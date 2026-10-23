using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Cards.Powers;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class Deathworld() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private const int BASE_SCIENCE = 2;
    private const int UPGRADE_SCIENCE = 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(BASE_SCIENCE)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        List<CardModel> cards = new List<CardModel>();
        for (int i = 0; i < DynamicVars.Cards.BaseValue; i++)
        {
            if (CombatState != null) cards.Add(CombatState.CreateCard<BlackScience>(Owner));
        }
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UPGRADE_SCIENCE);
    }
}