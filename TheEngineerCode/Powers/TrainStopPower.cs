using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;

public class TrainStopPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner.Player)
            return;

        IEnumerable<CardModel> wagonCards = player.Character.CardPool
            .GetUnlockedCards(
                player.UnlockState,
                player.RunState.CardMultiplayerConstraint)
            .Where(card => card.Tags.Contains(TheEngineerCardTags.Wagon));

        List<CardModel> cards = CardFactory.GetDistinctForCombat(
                player,
                wagonCards,
                (int)Amount,
                player.RunState.Rng.CombatCardGeneration)
            .ToList();

        if (cards.Count <= 0)
            return;

        Flash();

        foreach (CardModel card in cards)
        {
            CardCmd.ApplyKeyword(
                card,
                CardKeyword.Ethereal);

            ChargeHelper.Initialize(card);

            await CardPileCmd.AddGeneratedCardToCombat(
                card,
                PileType.Hand,
                player);
        }
    }
}