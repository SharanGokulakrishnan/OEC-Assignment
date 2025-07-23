using MediatR;
using RL.Backend.Commands.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    public class RemoveUserFromProcedureCommandHandler : IRequestHandler<RemoveUserFromProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public RemoveUserFromProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(RemoveUserFromProcedureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid PlanId: {request.PlanId}. Must be greater than zero."));

                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid ProcedureId: {request.ProcedureId}. Must be greater than zero."));

                if (request.UserId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid UserId: {request.UserId}. Must be greater than zero."));

                var entity = await _context.PlanProcedureUsers.FindAsync(
                    new object[] { request.PlanId, request.ProcedureId, request.UserId },
                    cancellationToken);

                if (entity == null)
                    return ApiResponse<Unit>.Fail(new NotFoundException(
                        $"Assignment not found for PlanId {request.PlanId}, ProcedureId {request.ProcedureId}, UserId {request.UserId}"));

                _context.PlanProcedureUsers.Remove(entity);
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
