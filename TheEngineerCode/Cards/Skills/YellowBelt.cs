using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Ui;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class YellowBelt() : TheEngineerCard(
    0,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    private const decimal BASE_PRODUCE = 1m;
    private const int EXHAUSTIVE = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ProduceVar(BASE_PRODUCE),
        new ExhaustiveVar(EXHAUSTIVE)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new DynamicCardHoverTip(() =>
        {
            if (!IsMutable)
                return null;

            Player? owner = Owner;

            if (owner is null)
                return null;

            return PileType.Discard
                .GetPile(owner)
                .Cards
                .LastOrDefault();
        })
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MaterialHelper.ProduceMaterial(
            Owner,
            choiceContext,
            (int)DynamicVars.Produce().BaseValue,
            MaterialDestination.Hand,
            this);

        CardPile discardPile = PileType.Discard.GetPile(Owner);
        CardModel? card = discardPile.Cards.LastOrDefault();

        if (card != null)
            await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Exhaustive"].UpgradeValueBy(1);
    }
}