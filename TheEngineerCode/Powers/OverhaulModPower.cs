using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Powers;

public class OverhaulModPower : TheEngineerPower
{
    public CardModel CreateMaterialReplacement(Player owner)
    {
        IEnumerable<CardModel> defectCards = ModelDb.CardPool<DefectCardPool>()
            .GetUnlockedCards(
                owner.UnlockState,
                owner.RunState.CardMultiplayerConstraint);

        CardModel card = CardFactory.GetDistinctForCombat(
                owner,
                defectCards,
                1,
                owner.RunState.Rng.CombatCardGeneration)
            .First();

        EnsureMaterialKeyword(card);

        return card;
    }

    private static void EnsureMaterialKeyword(CardModel card)
    {
        if (!card.Keywords.Contains(TheEngineerKeyWords.Material))
            card.AddKeyword(TheEngineerKeyWords.Material);
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}