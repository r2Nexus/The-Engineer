using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

[Pool(typeof(TheEngineerCardPool))]
public sealed class SpaceScience() : TheEngineerCard(
    2,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    private const decimal BASE_POWER = 1m;
    
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Science];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SpaceSciencePower>(BASE_POWER)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<SpaceSciencePower>(
            this,
            DynamicVars.Power<SpaceSciencePower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}