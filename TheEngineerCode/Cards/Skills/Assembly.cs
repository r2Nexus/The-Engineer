using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;


[Pool(typeof(TheEngineerCardPool))]
public sealed class Assembly() : TheEngineerCard(
    0,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    private const decimal BASE_PRODUCE = 1m;
    private const decimal UPGRADE_PRODUCE = 1m;

    private const decimal POWER_AMOUNT = 1m;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AssemblyPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromCard<Material>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ProduceVar(BASE_PRODUCE),
        new PowerVar<AssemblyPower>(POWER_AMOUNT)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MaterialHelper.ProduceMaterial(
            this,
            choiceContext,
            (int)DynamicVars.Produce().BaseValue,
            MaterialDestination.Hand);

        await CommonActions.ApplySelf<AssemblyPower>(
            this,
            DynamicVars.Power<AssemblyPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Produce().UpgradeValueBy(UPGRADE_PRODUCE);
    }
}