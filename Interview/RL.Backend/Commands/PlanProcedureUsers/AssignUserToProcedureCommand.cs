﻿using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands.PlanProcedureUsers
{
    public class AssignUserToProcedureCommand : IRequest<ApiResponse<Unit>>
    {
        public int PlanId { get; set; }
        public int ProcedureId { get; set; }
        public int UserId { get; set; }
    }
}
