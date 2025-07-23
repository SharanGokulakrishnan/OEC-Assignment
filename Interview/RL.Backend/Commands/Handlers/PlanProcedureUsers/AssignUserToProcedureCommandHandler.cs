using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    public class AssignUserToProcedureCommandHandler : IRequestHandler<AssignUserToProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public AssignUserToProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(AssignUserToProcedureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid PlanId: {request.PlanId}. Must be greater than zero."));

                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid ProcedureId: {request.ProcedureId}. Must be greater than zero."));

                if (request.UserId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid UserId: {request.UserId}. Must be greater than zero."));

                var exists = await _context.PlanProcedureUsers.AnyAsync(p =>
                    p.PlanId == request.PlanId &&
                    p.ProcedureId == request.ProcedureId &&
                    p.UserId == request.UserId,
                    cancellationToken);

                if (!exists)
                {
                    _context.PlanProcedureUsers.Add(new PlanProcedureUser
                    {
                        PlanId = request.PlanId,
                        ProcedureId = request.ProcedureId,
                        UserId = request.UserId
                    });

                    await _context.SaveChangesAsync(cancellationToken);
                }

                return ApiResponse<Unit>.Succeed(Unit.Value);
            }
            catch (Exception e)
            {
                return ApiResponse<Unit>.Fail(e);
            }
        }
    }
}
