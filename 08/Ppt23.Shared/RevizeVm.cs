namespace Ppt23.Shared;

public class RevizeVm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    public static List<RevizeVm> VratRandSeznam(int pocet)
    {
        List<RevizeVm> list = new();
        for (int i = 0; i < pocet; i++)
        {
            RevizeVm model = new()
            {
                Id = Guid.NewGuid(),
                Name = RandomString(Random.Shared.Next(5, 25)),
            };
            list.Add(model);
        }
        return list;
    }
    public static string RandomString(int length) =>
           new(Enumerable.Range(0, length).Select(_ => (char)Random.Shared.Next('a', 'z')).ToArray());
}
