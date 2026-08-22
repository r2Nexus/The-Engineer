using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Relics;

namespace TheEngineer.TheEngineerCode.Relics;

[Pool(typeof(TheEngineerRelicPool))]
public class TungstenCarbide() : TheEngineerRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;
    
    public override async Task AfterObtained()
    {
        await base.AfterObtained();

        foreach (Material material in Owner.Deck.Cards.OfType<Material>())
            material.RefreshTungstenCarbide();
    }
}