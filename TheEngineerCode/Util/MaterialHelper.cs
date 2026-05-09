using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Powers;

namespace TheEngineer.TheEngineerCode.Util;

public enum MaterialSource
{
    Hand,
    Draw,
    Discard,
    Stock
}

public enum MaterialDestination
{
    Hand,
    Draw,
    Discard
}

public readonly record struct MaterialRef(CardPile Pile, CardModel Card);

public static class MaterialHelper
{
    public static int CountMaterial(CardModel sourceCard, MaterialSource source)
        => CountMaterial(sourceCard.Owner, source);

    public static int CountMaterial(Player? owner, MaterialSource source)
    {
        if (owner == null)
            return 0;

        return GetMaterials(owner, source).Count;
    }

    public static bool CanConsumeMaterial(
        CardModel sourceCard,
        int amount,
        MaterialSource source)
        => CanConsumeMaterial(sourceCard.Owner, amount, source);

    public static bool CanConsumeMaterial(
        Player? owner,
        int amount,
        MaterialSource source)
    {
        if (owner == null || amount <= 0)
            return false;

        if (HasFreeConsume(owner))
            return true;

        return GetMaterials(owner, source).Count >= amount;
    }

    public static Task<bool> ConsumeMaterial(
        CardModel sourceCard,
        PlayerChoiceContext choiceContext,
        int amount,
        MaterialSource source)
        => ConsumeMaterial(
            sourceCard.Owner,
            choiceContext,
            amount,
            source,
            sourceCard);

    public static async Task<bool> ConsumeMaterial(
        Player? owner,
        PlayerChoiceContext choiceContext,
        int amount,
        MaterialSource source,
        AbstractModel? causedBy = null)
    {
        if (owner == null || amount <= 0)
            return false;

        if (await TrySpendFreeConsume(owner, choiceContext, causedBy))
            return true;

        var materials = GetMaterials(owner, source);

        if (materials.Count < amount)
            return false;

        for (var i = 0; i < amount; i++)
            await ExhaustMaterial(choiceContext, materials[i].Card);

        await EngineerHooks.OnConsumed(
            owner.Creature.CombatState,
            choiceContext,
            owner,
            amount,
            source,
            causedBy);

        return true;
    }

    public static Task<int> ConsumeAllMaterial(
        CardModel sourceCard,
        PlayerChoiceContext choiceContext,
        MaterialSource source)
        => ConsumeAllMaterial(
            sourceCard.Owner,
            choiceContext,
            source,
            sourceCard);

    public static async Task<int> ConsumeAllMaterial(
        Player? owner,
        PlayerChoiceContext choiceContext,
        MaterialSource source,
        AbstractModel? causedBy = null)
    {
        if (owner == null)
            return 0;

        var materials = GetMaterials(owner, source);
        var amount = materials.Count;

        if (amount <= 0)
            return 0;

        for (int i = 0; i < amount; i++)
            await ExhaustMaterial(choiceContext, materials[i].Card);

        await EngineerHooks.OnConsumed(
            owner.Creature.CombatState,
            choiceContext,
            owner,
            amount,
            source,
            causedBy);

        return amount;
    }

    public static Task<int> ProduceMaterial(
        CardModel sourceCard,
        PlayerChoiceContext choiceContext,
        int amount,
        MaterialDestination destination)
        => ProduceMaterial(
            sourceCard.Owner,
            choiceContext,
            amount,
            destination,
            sourceCard);

    public static async Task<int> ProduceMaterial(
        Player? owner,
        PlayerChoiceContext choiceContext,
        int amount,
        MaterialDestination destination,
        AbstractModel? causedBy = null)
    {
        if (owner == null || amount <= 0)
            return 0;

        PileType pileType = destination switch
        {
            MaterialDestination.Hand => PileType.Hand,
            MaterialDestination.Draw => PileType.Draw,
            MaterialDestination.Discard => PileType.Discard,
            _ => throw new ArgumentOutOfRangeException(nameof(destination))
        };

        var combatState = owner.Creature.CombatState;

        if (combatState == null)
            return 0;

        var produced = 0;

        for (var i = 0; i < amount; i++)
        {
            var material = combatState.CreateCard<Material>(owner);

            await CardPileCmd.AddGeneratedCardToCombat(
                material,
                pileType,
                owner);

            produced++;
        }

        if (produced > 0)
        {
            await EngineerHooks.OnProduced(
                combatState,
                choiceContext,
                owner,
                produced,
                destination,
                causedBy);
        }

        return produced;
    }

    private static bool HasFreeConsume(Player owner)
    {
        return GetFreeConsumePower(owner) != null;
    }

    private static FreeConsumePower? GetFreeConsumePower(Player owner)
    {
        return owner.Creature
            .Powers
            .OfType<FreeConsumePower>()
            .FirstOrDefault(power => power.Amount > 0);
    }

    private static async Task<bool> TrySpendFreeConsume(
        Player owner,
        PlayerChoiceContext choiceContext,
        AbstractModel? causedBy = null)
    {
        FreeConsumePower? freeConsume = GetFreeConsumePower(owner);

        if (freeConsume == null)
            return false;

        freeConsume.Flash();

        await CommonActions.Apply<FreeConsumePower>(
            choiceContext,
            owner.Creature,
            causedBy as CardModel,
            -1m);

        return true;
    }

    private static List<MaterialRef> GetMaterials(Player owner, MaterialSource source)
    {
        var result = source switch
        {
            MaterialSource.Hand =>
                ReadPile(PileType.Hand.GetPile(owner)),

            MaterialSource.Draw =>
                ReadPile(PileType.Draw.GetPile(owner)),

            MaterialSource.Discard =>
                ReadPile(PileType.Discard.GetPile(owner)),

            MaterialSource.Stock =>
                ReadPile(PileType.Hand.GetPile(owner))
                    .Concat(ReadPile(PileType.Draw.GetPile(owner)))
                    .Concat(ReadPile(PileType.Discard.GetPile(owner))),

            _ => Enumerable.Empty<MaterialRef>()
        };

        return result.ToList();
    }

    private static IEnumerable<MaterialRef> ReadPile(CardPile pile)
    {
        foreach (var card in pile.Cards)
        {
            if (card is Material)
                yield return new MaterialRef(pile, card);
        }
    }

    private static async Task ExhaustMaterial(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal = false)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        var combatState = card.CombatState ?? card.Owner.Creature.CombatState;

        await CardPileCmd.Add(card, PileType.Exhaust);

        if (combatState != null)
        {
            CombatManager.Instance.History.CardExhausted(combatState, card);

            await Hook.AfterCardExhausted(
                combatState,
                choiceContext,
                card,
                causedByEthereal);
        }
    }
}