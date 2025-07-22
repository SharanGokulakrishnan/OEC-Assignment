import React, { useEffect, useState } from "react";
import ReactSelect from "react-select";
import {
  getAssignedUsers,
  assignUserToProcedure,
  removeUserFromProcedure,
  removeAllUsersFromProcedure,
} from "../../../api/api"; // Adjust path if needed

const PlanProcedureItem = ({ planId, procedure, users }) => {
  const [selectedUsers, setSelectedUsers] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const procedureId = procedure?.procedureId;

  // Load assigned users when plan or procedure changes
  useEffect(() => {
    if (!procedureId) return;

    const fetchAssignedUsers = async () => {
      setIsLoading(true);
      try {
        const data = await getAssignedUsers(procedureId);

        // Format the result to match ReactSelect
        const formatted = Array.isArray(data)
          ? data.map((u) => ({
              label: u.name,
              value: u.userId,
            }))
          : [];

        setSelectedUsers(formatted);
      } catch (err) {
        console.error("Failed to load assigned users", err);
        setSelectedUsers([]); // Fallback to empty list
      } finally {
        setIsLoading(false);
      }
    };

    fetchAssignedUsers();
  }, [planId, procedureId]);

  const handleAssignUsers = async (newSelected) => {
    const newlyAdded =
      newSelected?.filter(
        (user) => !selectedUsers.some((u) => u.value === user.value)
      ) || [];

    const removed =
      selectedUsers?.filter(
        (user) => !newSelected?.some((u) => u.value === user.value)
      ) || [];

    try {
      for (const user of newlyAdded) {
        await assignUserToProcedure(planId, procedureId, user.value);
      }

      if (
        removed.length === selectedUsers.length &&
        newSelected?.length === 0
      ) {
        await removeAllUsersFromProcedure(planId, procedureId);
      } else {
        for (const user of removed) {
          await removeUserFromProcedure(planId, procedureId, user.value);
        }
      }

      setSelectedUsers(newSelected || []);
    } catch (err) {
      console.error("Error updating user assignments", err);
    }
  };

  return (
    <div className="py-4 border-b">
      <div className="font-bold mb-2">{procedure.procedureTitle}</div>

      <ReactSelect
        isMulti
        isLoading={isLoading}
        options={users}
        value={selectedUsers}
        onChange={handleAssignUsers}
        placeholder="Select User to Assign"
      />
    </div>
  );
};

export default PlanProcedureItem;
