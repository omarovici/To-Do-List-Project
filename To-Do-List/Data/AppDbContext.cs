using Microsoft.EntityFrameworkCore;
using To_Do_List_Project.Models;

namespace To_Do_List_Project.Data;
using Task = To_Do_List_Project.Models.Task;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Task> Tasks { get; set; }
}