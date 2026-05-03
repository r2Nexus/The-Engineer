using BaseLib.Abstracts;
using TheEngineer.TheEngineerCode.Extensions;
using Godot;

namespace TheEngineer.TheEngineerCode.Character;

public class TheEngineerRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => TheEngineer.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}