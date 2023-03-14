namespace Ppt23.Client.ViewModels;

public class VybaveniVm
{
    public string Name { get; set; } = "";
    public bool IsRevisionNeeded { get => DateTime.Now.AddYears(-2) < LastRevisionDateTime ; }
    public DateTime BoughtDateTime { get; set; }
    public DateTime LastRevisionDateTime { get; set; }

    public bool IsInEditMode { get; set; }

    public static List<VybaveniVm> VratRandSeznam(int pocet)
    {
        List<VybaveniVm> list = new();
        for (int i = 0; i < pocet; i++)
        {
            VybaveniVm model = new()
            {
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
