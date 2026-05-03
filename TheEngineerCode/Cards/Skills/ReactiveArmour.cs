using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Powers;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class ReactiveArmour() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_BLOCK = 4;
    private const decimal UPGRADE_BLOCK = 3;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(BASE_BLOCK, ValueProp.Move),
        new PowerVar<ReactiveArmourPower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await OrbCmd.Channel<LandMineOrb>(choiceContext, Owner);
        await CommonActions.ApplySelf<ReactiveArmourPower>(this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
    }
}