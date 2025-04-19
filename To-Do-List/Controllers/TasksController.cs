using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using To_Do_List_Project.Data;
using To_Do_List_Project.Models;
using Task = To_Do_List_Project.Models.Task;

namespace To_Do_List_Project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async System.Threading.Tasks.Task<ActionResult<IEnumerable<Task>>> GetTasks(
        [FromQuery] bool? completed,
        [FromQuery] string? dueDate,
        [FromQuery] string? priority)
    {
        var query = _context.Tasks.AsQueryable();

        if (completed.HasValue)
            query = query.Where(t => t.IsCompleted == completed);

        if (!string.IsNullOrEmpty(dueDate))
            query = query.Where(t => t.DueDate.Date == DateTime.Parse(dueDate).Date);

        if (!string.IsNullOrEmpty(priority))
            query = query.Where(t => t.Priority == priority.ToLower());

        return await query.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Task>> GetTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        return task;
    }

    [HttpPost]
    public async Task<ActionResult<Task>> CreateTask(Task task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, Task task)
    {
        if (id != task.Id)
            return BadRequest();

        _context.Entry(task).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TaskExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> MarkComplete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        task.IsCompleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}/incomplete")]
    public async Task<IActionResult> MarkIncomplete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        task.IsCompleted = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}/priority")]
    public async Task<IActionResult> UpdatePriority(int id, [FromBody] string priority)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        task.Priority = priority.ToLower();
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TaskExists(int id)
    {
        return _context.Tasks.Any(e => e.Id == id);
    }
}