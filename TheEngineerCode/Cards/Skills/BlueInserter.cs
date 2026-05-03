using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class BlueInserter() : TheEngineerCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private const int BASE_DRAW = 2;
    private const int UPGRADE_DRAW = 1;

    private const int BASE_DISCARD = 2;
    private const int UPGRADE_DISCARD = 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(BASE_DRAW),
        new IntVar("Discard",BASE_DISCARD)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CommonActions.Draw(this, choiceContext);
        await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, (int)DynamicVars["Discard"].BaseValue), null,  this));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UPGRADE_DRAW);
    }
}