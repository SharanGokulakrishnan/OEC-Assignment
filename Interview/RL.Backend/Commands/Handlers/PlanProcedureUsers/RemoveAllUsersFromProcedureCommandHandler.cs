using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    public class RemoveAllUsersFromProcedureCommandHandler : IRequestHandler<RemoveAllUsersFromProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public RemoveAllUsersFromProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(RemoveAllUsersFromProcedureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid PlanId: {request.PlanId}. Must be greater than zero."));

                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid ProcedureId: {request.ProcedureId}. Must be greater than zero."));

                var list = await _context.PlanProcedureUsers
                    .Where(pu => pu.PlanId == request.PlanId && pu.ProcedureId == request.ProcedureId)
                    .ToListAsync(cancellationToken);

                _context.PlanProcedureUsers.RemoveRange(list);
                await _context.SaveChangesAsync(cancellationToken);
                return ApiResponse<Unit>.Succeed(Unit.Value);
            }
            catch (Exception e)
            {
                return ApiResponse<Unit>.Fail(e);
            }
        }
    }
}
