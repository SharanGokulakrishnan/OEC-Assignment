using Microsoft.AspNetCore.Mvc;
using RL.Data;
using RL.Data.DataModels;
using Microsoft.EntityFrameworkCore;
namespace RL.Backend.Controllers;


[ApiController]
[Route("[controller]")]
public class PlanProcedureUserController : ControllerBase
{
    private readonly RLContext _context;

    public PlanProcedureUserController(RLContext context)
    {
        _context = context;
    }

    [HttpGet("{procedureId}")]
public async Task<IActionResult> GetAssignedUsers(int procedureId)
{
    try
    {
        var users = await _context.PlanProcedureUsers
    .Where(ppu => ppu.ProcedureId == procedureId && ppu.User != null)
    .Include(ppu => ppu.User)
    .Select(ppu => new { ppu.User.UserId, ppu.User.Name })
    .ToListAsync();

        return Ok(users);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex); // Or use ILogger

        return StatusCode(500, "Internal server error while fetching users.");
    }
}


    [HttpPost]
public IActionResult AssignUser([FromBody] RL.Data.DataModels.PlanProcedureUser model)
    {
        var exists = _context.PlanProcedureUsers.Any(x =>
            x.ProcedureId == model.ProcedureId &&
            x.UserId == model.UserId);

        if (!exists)
        {
            _context.PlanProcedureUsers.Add(model);
            _context.SaveChanges();
            return Ok(new { message = "Successfully added" });
        }

        return Ok();
    }

        [HttpDelete("{planId}/{procedureId}/{userId}")]
    public IActionResult RemoveUser(int planId, int procedureId, int userId)
    {
        var entity = _context.PlanProcedureUsers
            .FirstOrDefault(x => x.PlanId == planId && x.ProcedureId == procedureId && x.UserId == userId);

        if (entity != null)
        {
            _context.PlanProcedureUsers.Remove(entity);
            _context.SaveChanges();
        }

        return Ok();
    }

    [HttpDelete("{planId}/{procedureId}")]
    public IActionResult RemoveAllUsers(int planId, int procedureId)
    {
        var users = _context.PlanProcedureUsers
            .Where(x => x.PlanId == planId && x.ProcedureId == procedureId)
            .ToList();

        _context.PlanProcedureUsers.RemoveRange(users);
        _context.SaveChanges();

            return Ok(new { message = "Users removed" });

    }
}
