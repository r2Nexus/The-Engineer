using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

[Pool(typeof(TheEngineerCardPool))]
public class Landfill() : TheEngineerCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_ORBS= 2m;
    private const decimal UPGRADE_ORBS = 1m;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new RepeatVar((int)BASE_ORBS)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await OrbCmd.AddSlots(Owner, DynamicVars.Repeat.IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy((int)UPGRADE_ORBS);
    }
}