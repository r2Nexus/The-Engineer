using BaseLib.Abstracts;
using BaseLib.Extensions;
using TheEngineer.TheEngineerCode.Extensions;
using Godot;

namespace TheEngineer.TheEngineerCode.Powers;

public abstract class TheEngineerPower : CustomPowerModel
{
    //Loads from TheEngineer/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}