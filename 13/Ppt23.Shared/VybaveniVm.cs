using System.ComponentModel.DataAnnotations;

namespace Ppt23.Shared;

public class VybaveniVm
{
    [Required,MinLength(5,ErrorMessage ="Alespoň 5 písmen!")]
    public string Name { get; set; } = "";
    public Guid Id { get; set; }
    public bool IsRevisionNeeded { get => DateTime.Now.AddYears(-2) < LastRevisionDateTime ; }
    public DateTime BoughtDateTime { get; set; }
    public DateTime? LastRevisionDateTime { get; set; } 

    public int PriceCzk { get; set; }


    public static List<VybaveniVm> VratRandSeznam(int pocet, bool isEmtpyId = false)
    {
        List<VybaveniVm> list = new();
        for (int i = 0; i < pocet; i++)
        {
            VybaveniVm model = new()
            {
                
                Id = isEmtpyId? Guid.Empty :Guid.NewGuid(),
                Name = RandomString(Random.Shared.Next(5, 25)),
                BoughtDateTime = DateTime.Now.AddDays(-Random.Shared.Next(3 * 365, 20 * 365)),
                LastRevisionDateTime = DateTime.Now.AddDays(-Random.Shared.Next(0, 3 * 365)),
            };
            list.Add(model);
        }
        return list;
    }
    public static string RandomString(int length) =>
           new(Enumerable.Range(0, length).Select(_ => (char)Random.Shared.Next('a', 'z')).ToArray());
}

public class VybaveniSrevizemaVm
{
    public string Name { get; set; } = "";
    public Guid Id { get; set; }
    //public bool IsRevisionNeeded { get => DateTime.Now.AddYears(-2) < LastRevisionDateTime; }
    public DateTime BoughtDateTime { get; set; }
    public List<RevizeVm> Revizes { get; set; } = new();
}