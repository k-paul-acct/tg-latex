using LatexView.Bot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LatexView.Bot.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<CompileRequest> CompileRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Username).HasColumnName("username");
            entity.Property(x => x.Firstname).HasColumnName("firstname");
            entity.Property(x => x.Lastname).HasColumnName("lastname");
            entity.Property(x => x.CreatedTimestamp).HasColumnName("created_timestamp");
        });

        modelBuilder.Entity<CompileRequest>(entity =>
        {
            entity.ToTable("compile_request");

            entity.HasKey(x => new { x.UserId, x.MessageId });

            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.MessageId).HasColumnName("message_id");
            entity.Property(x => x.ResultMessageId).HasColumnName("result_message_id");
            entity.Property(x => x.CreatedTimestamp).HasColumnName("created_timestamp");
            entity.Property(x => x.UpdatedTimestamp).HasColumnName("updated_timestamp");
            entity.ComplexProperty(x => x.Options, x => x.ToJson("options"));

            entity.HasOne(x => x.User).WithMany(x => x.CompileRequests).HasForeignKey(x => x.UserId);
        });
    }
}

internal sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(args[0]);
        return new AppDbContext(optionsBuilder.Options);
    }
}
