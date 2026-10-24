using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

[Pool(typeof(TokenCardPool))]
public class RedScience() : TheEngineerCard(1,
    CardType.Power, CardRarity.Token,
    TargetType.Self)
{
    private const decimal BASE_POWER = 1;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-PRODUCEALL"),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL"),
        HoverTipFactory.FromCard<Material>()
    ];
    
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Science];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<RedSciencePower>(BASE_POWER)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        TheEngineerKeyWords.Material,
        TheEngineerKeyWords.Science
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<RedSciencePower>(this,DynamicVars.Power<RedSciencePower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}