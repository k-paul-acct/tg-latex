using LatexView.Bot.Data;
using LatexView.Bot.Models;

internal sealed class UserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnsureCreated(long id, string? username, string? firstname, string? lastname)
    {
        var existing = await _dbContext.Users.FindAsync(id);
        if (existing is null)
        {
            var newUser = new User
            {
                Id = id,
                Username = username,
                Firstname = firstname,
                Lastname = lastname,
                CreatedTimestamp = DateTimeOffset.UtcNow,
            };
            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();
        }
    }
}
