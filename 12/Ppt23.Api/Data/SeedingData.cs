using Mapster;
using Microsoft.AspNetCore.Identity;
using Ppt23.Shared;

namespace Ppt23.Api.Data;

public class SeedingData
{
    private readonly PptDbContext _db;

    public SeedingData(PptDbContext db)
    {
        _db = db;
    }
    public async Task SeedData()
    {
        if (!_db.Vybavenis.Any())
        { //není žádné vybavení - nějaké přidáme
           var vybaveniLis = VybaveniVm.VratRandSeznam(10,isEmtpyId:false).Select(x => x.Adapt<Vybaveni>());
            _db.Vybavenis.AddRange(vybaveniLis);
        }

        await _db.SaveChangesAsync();
    }

}
