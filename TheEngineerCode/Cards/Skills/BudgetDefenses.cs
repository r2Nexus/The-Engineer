using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class BudgetDefenses() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    private const decimal BASE_BLOCK = 8m;
    private const decimal UPGRADE_BLOCK = 2m;

    private const int BASE_PRODUCE = 1;
    private const int UPGRADE_PRODUCE = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(BASE_BLOCK, ValueProp.Move),
        new ProduceVar(BASE_PRODUCE)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, DynamicVars.Block, play);

        await MaterialHelper.ProduceMaterial(
            Owner,
            choiceContext,
            (int)DynamicVars.Produce().BaseValue,
            MaterialDestination.Hand,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
        DynamicVars.Produce().UpgradeValueBy(UPGRADE_PRODUCE);
    }
}