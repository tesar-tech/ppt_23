namespace Ppt23.Api.Data; 

public class Pracovnik
{
    public string Name { get; set; } = "";
    public Guid Id { get; set; }
    public List<Ukon> Ukons { get; set; } = new();

}
