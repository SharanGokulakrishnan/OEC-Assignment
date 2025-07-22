// File: RL.Data/DataModels/PlanProcedureUser.cs
using RL.Data.DataModels;
public class PlanProcedureUser
{
    public int PlanId { get; set; }
    public int ProcedureId { get; set; }
    public int UserId { get; set; }

    public virtual Plan Plan { get; set; }
    public virtual Procedure Procedure { get; set; }
    public virtual User User { get; set; }
}
