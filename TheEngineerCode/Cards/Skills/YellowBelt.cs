using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Cards.Variables;
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
    0,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    private const int BASE_DRAW = 1;
    private const int UPGRADE_DRAW = 0;

    private const decimal BASE_PRODUCE = 1m;

    private const int EXHAUSTIVE = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(BASE_DRAW),
        new ProduceVar(BASE_PRODUCE),
        new ExhaustiveVar(EXHAUSTIVE)
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

    protected override CardLocation GetResultLocationForCardPlay()
    {
        CardLocation locationForCardPlay = base.GetResultLocationForCardPlay();
        if (locationForCardPlay.pileType == PileType.Discard)
            locationForCardPlay.pileType = PileType.Hand;
        return locationForCardPlay;
    }
    protected override void OnUpgrade()
    {
        DynamicVars["Exhaustive"].UpgradeValueBy(1);
    }
}