using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

public class Derailment() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.AnyAlly)
{

    private const int BASE_WAGONING = 2;
    private const int UPGRADE_WAGONING = 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(BASE_WAGONING)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheEngineerKeyWords.Wagon)
    ];
    
    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if(play.Target == null) return;
        if(play.Target.Player == null) return;
        
        List<CardModel> hand = PileType.Hand
            .GetPile(play.Target.Player)
            .Cards
            .Where(card =>
                !card.Keywords.Contains(TheEngineerKeyWords.Wagon))
            .ToList();
        
        for (int i = 0; i < DynamicVars.Cards.IntValue ; i++)
        {
            CardModel? selected =
                Owner.RunState.Rng.CombatCardSelection.NextItem(hand);
            
            if (selected == null) break;
            
            hand.Remove(selected);
            CardCmd.ApplyKeyword(selected, TheEngineerKeyWords.Wagon);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UPGRADE_WAGONING);
    }
}