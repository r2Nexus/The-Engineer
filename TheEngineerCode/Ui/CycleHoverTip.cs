using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.HoverTips;

public interface IResolvingHoverTip : IHoverTip
{
    IHoverTip ResolveHoverTip();
    
    int ResolveVersion { get; }
}

public sealed class CycleHoverTip : IResolvingHoverTip
{
    private readonly IReadOnlyList<CardModel> _cards;
    private readonly double _secondsPerCard;
    private readonly bool _upgrade;

    public CycleHoverTip(
        IReadOnlyList<CardModel> cards,
        double secondsPerCard = 1.25,
        bool upgrade = false)
    {
        if (cards == null || cards.Count == 0)
            throw new ArgumentException("CycleHoverTip needs at least one card.", nameof(cards));

        _cards = cards;
        _secondsPerCard = Math.Max(0.25, secondsPerCard);
        _upgrade = upgrade;
    }

    private int CurrentIndex
    {
        get
        {
            double elapsedSeconds = Time.GetTicksMsec() / 1000.0;
            return (int)(Math.Floor(elapsedSeconds / _secondsPerCard) % _cards.Count);
        }
    }

    public int ResolveVersion => CurrentIndex;

    public IHoverTip ResolveHoverTip()
    {
        return HoverTipFactory.FromCard(_cards[CurrentIndex], _upgrade);
    }

    public string Id => "THEENGINEER-CYCLE_HOVER_TIP";
    public bool IsSmart => false;
    public bool IsDebuff => false;
    public bool IsInstanced => true;
    public AbstractModel? CanonicalModel => null;
}

public static class HoverTipResolver
{
    public static IHoverTip Resolve(IHoverTip tip)
    {
        return tip is IResolvingHoverTip resolving
            ? resolving.ResolveHoverTip()
            : tip;
    }

    public static List<IHoverTip> ResolveAll(IEnumerable<IHoverTip> tips)
    {
        return tips.Select(Resolve).ToList();
    }

    public static bool HasResolvingTip(IEnumerable<IHoverTip> tips)
    {
        return tips.Any(t => t is IResolvingHoverTip);
    }

    public static int GetVersionKey(IEnumerable<IHoverTip> tips)
    {
        unchecked
        {
            int hash = 17;

            foreach (IHoverTip tip in tips)
            {
                if (tip is IResolvingHoverTip resolving)
                    hash = hash * 31 + resolving.ResolveVersion;
            }

            return hash;
        }
    }
}