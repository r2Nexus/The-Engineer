using BaseLib.Abstracts;
using BaseLib.Utils;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Potions;

[Pool(typeof(TheEngineerPotionPool))]
public abstract class TheEngineerPotion : CustomPotionModel;