namespace Ppt23.Shared;

public class UkonVm
{
    public Guid Id { get; set; }
    public string Detail { get; set; } = "";
    public string Kod { get; set; } = "";
    public DateTime DateTime { get; set; } = DateTime.Now;
    public Guid VybaveniId { get; set; }
}
