namespace Ppt23.Client.ViewModels;

public class VybaveniVm
{
    public string Name { get; set; } = "";
    //public bool IsNeedRevision { get => ;  }

    public bool kdkdkdkdk{ get=> Name=="Jack";  }

    public static List<VybaveniVm> VratRandSeznam(int pocet)
    { 
        return new List<VybaveniVm>() { new VybaveniVm() { Name="dkdk"}, new VybaveniVm() { Name="KDIEI"} };
    }
}
