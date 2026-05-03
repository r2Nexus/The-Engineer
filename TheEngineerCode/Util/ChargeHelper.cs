using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Util;

public static class ChargeHelper
{
    public static async Task OnConsumed(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialSource source,
        AbstractModel? causedBy)
    {
        AddChargeToAll(player, amount);

        await Task.CompletedTask;
    }

    public static bool HasCharge(CardModel card)
    {
        return card != null && card.Tags.Contains(TheEngineerCardTags.Charge);
    }

    public static decimal GetInitial(CardModel card)
        => card.DynamicVars.ChargeInitial().BaseValue;

    public static decimal GetCurrent(CardModel card)
        => card.DynamicVars.ChargeCurrent().BaseValue;

    public static decimal GetMax(CardModel card)
        => card.DynamicVars.ChargeMax().BaseValue;

    public static bool IsFull(CardModel card)
    {
        if (!HasCharge(card))
            return false;

        return GetCurrent(card) >= GetMax(card);
    }

    public static void Initialize(CardModel card)
    {
        if (!HasCharge(card))
            return;

        SetCurrent(card, GetInitial(card));
    }

    public static void ResetToInitial(CardModel card)
    {
        if (!HasCharge(card))
            return;

        SetCurrent(card, GetInitial(card));
    }

    public static void Empty(CardModel card)
    {
        if (!HasCharge(card))
            return;

        SetCurrent(card, 0m);
    }

    public static void AddCharge(CardModel card, decimal amount)
    {
        if (!HasCharge(card) || amount <= 0)
            return;

        decimal current = GetCurrent(card);
        decimal max = GetMax(card);
        decimal target = Math.Min(current + amount, max);

        SetCurrent(card, target);
    }

    public static bool TrySpendFullCharge(CardModel card)
    {
        if (!IsFull(card))
            return false;

        Empty(card);
        return true;
    }

    public static void SetCurrent(CardModel card, decimal value)
    {
        if (!HasCharge(card))
            return;

        decimal clamped = Math.Max(0m, Math.Min(value, GetMax(card)));
        var current = card.DynamicVars.ChargeCurrent();
        decimal delta = clamped - current.BaseValue;

        if (delta != 0m)
            current.UpgradeValueBy(delta);
    }

    public static void AddChargeToAll(Player owner, decimal amount)
    {
        if (owner == null || amount <= 0)
            return;

        foreach (CardModel card in GetAllRelevantCombatCards(owner))
            AddCharge(card, amount);
    }

    public static void InitializeAll(Player owner)
    {
        if (owner == null)
            return;

        foreach (CardModel card in GetAllRelevantCombatCards(owner))
            Initialize(card);
    }

    public static IEnumerable<CardModel> GetAllRelevantCombatCards(Player owner)
    {
        HashSet<CardModel> seen = new();

        foreach (CardModel card in ReadPile(PileType.Hand.GetPile(owner)))
            if (seen.Add(card))
                yield return card;

        foreach (CardModel card in ReadPile(PileType.Draw.GetPile(owner)))
            if (seen.Add(card))
                yield return card;

        foreach (CardModel card in ReadPile(PileType.Discard.GetPile(owner)))
            if (seen.Add(card))
                yield return card;
    }

    private static IEnumerable<CardModel> ReadPile(CardPile pile)
    {
        foreach (CardModel card in pile.Cards)
        {
            if (HasCharge(card))
                yield return card;
        }
    }
}