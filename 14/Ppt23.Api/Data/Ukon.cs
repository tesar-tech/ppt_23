
namespace Ppt23.Api.Data;
public class Ukon
{
    public Guid Id { get; set; }
    public string Detail { get; set; } = "";
    public string Kod { get; set; } = "";

    public DateTime DateTime { get; set; }

    public Guid VybaveniId { get; set; }
    public Vybaveni Vybaveni { get; set; } = null!;

    public Guid? PracovnikId { get; set; }
    public Pracovnik? Pracovnik { get; set; } 
}