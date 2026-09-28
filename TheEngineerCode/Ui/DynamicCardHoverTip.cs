using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.Ui;

public sealed class DynamicCardHoverTip : IResolvingHoverTip
{
    private readonly Func<CardModel?> _cardResolver;

    private CardModel? _lastCard;
    private int _version;

    public DynamicCardHoverTip(Func<CardModel?> cardResolver)
    {
        _cardResolver = cardResolver;
    }

    public int ResolveVersion
    {
        get
        {
            CardModel? card = _cardResolver();

            if (!ReferenceEquals(card, _lastCard))
            {
                _lastCard = card;
                _version++;
            }

            return _version;
        }
    }

    public IHoverTip? ResolveHoverTip()
    {
        CardModel? card = _cardResolver();

        return card == null
            ? null
            : new ResolvedDynamicCardHoverTip(card, this);
    }

    public string Id => "THEENGINEER-DYNAMIC_CARD_HOVER_TIP";
    public bool IsSmart => false;
    public bool IsDebuff => false;
    public bool IsInstanced => true;
    public AbstractModel? CanonicalModel => null;
}