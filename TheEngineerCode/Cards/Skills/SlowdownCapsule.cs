using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;


[Pool(typeof(TheEngineerCardPool))]
public class SlowdownCapsule() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_SLOW = 1;
    private const decimal UPGRADE_CORRODE = 0;

    private const int BASE_CONSUME = 2;
    private const int UPGRADE_CONSUME = -1;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<LandMineOrb>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ConsumeVar(BASE_CONSUME),
        new PowerVar<SlowdownCapsulePower>(BASE_SLOW)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await OrbCmd.Channel<LandMineOrb>(choiceContext, Owner);

        bool consumed = await MaterialHelper.ConsumeMaterial(
            this, 
            choiceContext, 
            (int)DynamicVars.Consume().BaseValue,
            MaterialSource.Hand,
            play);
        if (consumed)
        {
            await CommonActions.ApplySelf<SlowdownCapsulePower>(this, DynamicVars.Power<SlowdownCapsulePower>().BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Consume().UpgradeValueBy(UPGRADE_CONSUME);
    }
}