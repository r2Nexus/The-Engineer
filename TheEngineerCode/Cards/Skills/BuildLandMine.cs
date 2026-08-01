using BaseLib.Cards.Variables;
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
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;


[Pool(typeof(TheEngineerCardPool))]
public class BuildLandMine() : TheEngineerCard(0,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<LandMineOrb>()
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ConsumeVar(1),
        new ExhaustiveVar(2)
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<LandMineOrb>(choiceContext, Owner);

        bool consumed = await MaterialHelper.ConsumeMaterial(
            this,
            choiceContext,
            1,
            MaterialSource.Hand,
            play);

        if (consumed)
        {
            await OrbCmd.Channel<LandMineOrb>(choiceContext, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Exhaustive"]
            .UpgradeValueBy(1);
    }
}