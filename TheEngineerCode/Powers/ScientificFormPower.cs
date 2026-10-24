using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using TheEngineer.TheEngineerCode.Cards.Skills;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Powers;

public class ScientificFormPower :TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner.Player)
            return;

        for (int i = 0; i < Amount; ++i)
        {
            
            IEnumerable<CardModel> scienceCards = ModelDb.CardPool<TokenCardPool>()
                .GetUnlockedCards(
                    Owner.Player.UnlockState,
                    Owner.Player.RunState.CardMultiplayerConstraint)
                .Where(card => card.Tags.Contains(TheEngineerCardTags.Science));

            IEnumerable<CardModel> card = CardFactory.GetDistinctForCombat(
                Owner.Player,
                scienceCards,
                1,
                Owner.Player.RunState.Rng.CombatCardGeneration);

            await CardPileCmd.AddGeneratedCardToCombat(
                card.First(),
                PileType.Hand,
                Owner.Player);
        }
    }
}