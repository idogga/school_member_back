using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class MemberDbContext : DbContext
{
    private readonly ILoggerFactory loggerFactory;

    public MemberDbContext(DbContextOptions<MemberDbContext> options, ILoggerFactory loggerFactory)
            : base(options)
    {
        this.loggerFactory = loggerFactory;
    }
    public DbSet<User> Users => Set<User>();
    
    public DbSet<Pupil> Teachers => Set<Pupil>();
}