using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;


[Pool(typeof(TheEngineerCardPool))]
public class YellowBelt() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    private const int BASE_DRAW = 2;
    private const int UPGRADE_DRAW = 0;

    private const decimal BASE_PRODUCE = 1m;
    private const decimal UPGRADE_PRODUCE = 1m;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(BASE_DRAW),
        new ProduceVar(BASE_PRODUCE)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner);

        await MaterialHelper.ProduceMaterial(
            Owner,
            choiceContext,
            (int)DynamicVars.Produce().BaseValue,
            MaterialDestination.Hand,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UPGRADE_DRAW);
        DynamicVars.Produce().UpgradeValueBy(UPGRADE_PRODUCE);
    }
}