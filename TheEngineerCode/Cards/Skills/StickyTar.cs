using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class StickyTar() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    private const decimal BASE_OIL = 6m;
    private const decimal UPGRADE_OIL = 2m;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OilPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OilPower>(BASE_OIL)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await CommonActions.Apply<OilPower>(
            play.Target,
            this,
            DynamicVars.Power<OilPower>().BaseValue);

        CardSelectorPrefs prefs = new CardSelectorPrefs(
            SelectionScreenPrompt,
            1);

        CardModel? selectedCard = (await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                prefs,
                RetainFilter,
                this))
            .FirstOrDefault();

        if (selectedCard == null)
            return;

        CardCmd.ApplyKeyword(
            selectedCard,
            CardKeyword.Retain);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<OilPower>().UpgradeValueBy(UPGRADE_OIL);
    }

    private static bool RetainFilter(CardModel card)
    {
        return !card.Keywords.Contains(CardKeyword.Retain);
    }
}