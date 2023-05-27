using Microsoft.EntityFrameworkCore;

namespace Ppt23.Api.Data;

public class PptDbContext : DbContext
{
    public PptDbContext(DbContextOptions<PptDbContext> options)
        : base(options)
    {

    }

    public DbSet<Vybaveni> Vybavenis => Set<Vybaveni>();
    public DbSet<Revize> Revizes => Set<Revize>();
}
