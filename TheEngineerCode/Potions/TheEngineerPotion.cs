using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Extensions;

namespace TheEngineer.TheEngineerCode.Potions;

[Pool(typeof(TheEngineerPotionPool))]
public abstract class TheEngineerPotion : CustomPotionModel
{
    public override string? CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png"
            .PotionImagePath();

    public override string? CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png"
            .PotionOutlineImagePath();
}