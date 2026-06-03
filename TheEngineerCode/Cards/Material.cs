using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Material() : TheEngineerCard(-1,
    CardType.Skill, CardRarity.Token,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override bool IsPlayable => false;
    public override int MaxUpgradeLevel => 0;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        TheEngineerKeyWords.Material
    ];
    
    protected override Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {

    }
}