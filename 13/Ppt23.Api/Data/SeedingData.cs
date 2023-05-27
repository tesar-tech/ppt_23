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
            var vybaveniLis = VybaveniVm.VratRandSeznam(10, isEmtpyId: false).Select(x => x.Adapt<Vybaveni>()).ToList();

            foreach (var vybaveni in vybaveniLis)
            {
                int numOfRevizes = Random.Shared.Next(0, 10);
                for (int i = 0; i < numOfRevizes; i++)
                {
                    Revize rev = new()
                    {
                        Name = RandomString(Random.Shared.Next(5, 15)),
                        DateTime = vybaveni.BoughtDateTime.AddDays(Random.Shared.Next(0, 3 * 365)),
                    };
                    vybaveni.Revizes.Add(rev);



                }

                foreach (var num in Enumerable.Range(0, Random.Shared.Next(0, 20)))
                {
                    Ukon ukon = new()
                    {
                        Kod = RandomString(Random.Shared.Next(5, 10)),
                        DateTime = vybaveni.BoughtDateTime.AddDays(Random.Shared.Next(0, 3 * 365)),
                        Detail = RandomString(Random.Shared.Next(5, 350))
                    };
                    vybaveni.Ukons.Add(ukon);
                }



                _db.Vybavenis.AddRange(vybaveniLis);
            }

            await _db.SaveChangesAsync();
        }
    }

        public static string RandomString(int length) =>
           new(Enumerable.Range(0, length).Select(_ =>
           Random.Shared.Next(0, 5) == 0 ? ' ' : //randomly add spaces
           (char)Random.Shared.Next('a', 'z')).ToArray());//add random chars

    }
