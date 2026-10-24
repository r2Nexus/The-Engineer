using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Cards.Powers;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class GlebaSpecial() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
        EngineerCycleHoverTips.ForKeyword(TheEngineerKeyWords.Science)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        List<CardModel> hand = PileType.Hand
            .GetPile(Owner)
            .Cards
            .Where(card =>
                !card.Keywords.Contains(CardKeyword.Ethereal))
            .ToList();

        for (int i = 0; i < 2 ; i++)
        {
            CardModel? selected =
                Owner.RunState.Rng.CombatCardSelection.NextItem(hand);
            
            if (selected == null) break;
            
            hand.Remove(selected);
            CardCmd.ApplyKeyword(selected, CardKeyword.Ethereal);
        }
        
        IEnumerable<CardModel> scienceCards = ModelDb.CardPool<TokenCardPool>()
            .GetUnlockedCards(
                Owner.UnlockState,
                Owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.Tags.Contains(TheEngineerCardTags.Science));

        List<CardModel> cards = CardFactory.GetDistinctForCombat(
                Owner,
                scienceCards,
                2,
                Owner.RunState.Rng.CombatCardGeneration)
            .ToList();

        foreach (CardModel card in cards)
        {
            await CardPileCmd.AddGeneratedCardToCombat(
                card,
                PileType.Hand,
                Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}