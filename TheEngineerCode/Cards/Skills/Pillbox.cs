using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Orbs;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

public class Pillbox() : TheEngineerCard(2,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_PLATING = 5m;
    private const decimal UPGRADE_PLATING = 2m;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<PlatingPower>(BASE_PLATING)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<PlatingPower>(choiceContext, this);
        await OrbCmd.Channel<TurretOrb>(choiceContext, Owner);
        await OrbCmd.Channel<TurretOrb>(choiceContext, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<PlatingPower>().UpgradeValueBy(UPGRADE_PLATING);
    }
}