using Microsoft.EntityFrameworkCore;

namespace flowerpass;

internal record Login(int Id)
{
    public string Website { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}

internal class LoginContext(DbContextOptions<LoginContext> options) : DbContext(options)
{
    public DbSet<Login> Logins { get; set; }   
}