using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.Ui;

public interface IResolvingHoverTip : IHoverTip
{
    IHoverTip? ResolveHoverTip();

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
            throw new ArgumentException(
                "CycleHoverTip needs at least one card.",
                nameof(cards));

        _cards = cards;
        _secondsPerCard = Math.Max(0.25, secondsPerCard);
        _upgrade = upgrade;
    }

    private int CurrentIndex
    {
        get
        {
            double elapsedSeconds = Time.GetTicksMsec() / 1000.0;

            return (int)(
                Math.Floor(elapsedSeconds / _secondsPerCard)
                % _cards.Count);
        }
    }

    public int ResolveVersion => CurrentIndex;

    public IHoverTip ResolveHoverTip()
    {
        CardModel card = _cards[CurrentIndex];

        if (_upgrade)
        {
            card = (CardModel)card.MutableClone();
            card.UpgradeInternal();
            card.FinalizeUpgradeInternal();
        }

        return new ResolvedDynamicCardHoverTip(card, this);
    }

    public string Id => "THEENGINEER-CYCLE_HOVER_TIP";
    public bool IsSmart => false;
    public bool IsDebuff => false;
    public bool IsInstanced => true;
    public AbstractModel? CanonicalModel => null;
}

public sealed class ResolvedDynamicCardHoverTip(
    CardModel card,
    IResolvingHoverTip source)
    : CardHoverTip(card)
{
    public IResolvingHoverTip Source { get; } = source;
}