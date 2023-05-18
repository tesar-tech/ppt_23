using System.ComponentModel.DataAnnotations;

namespace Ppt23.Api.Data;

public class Revize
{
    //[Key] //už ví z názvu Id, že je to primární klíč. 
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    public DateTime DateTime { get; set; }

    public Guid VybaveniId { get; set; }
    public Vybaveni Vybaveni { get; set; } = null!;


}
