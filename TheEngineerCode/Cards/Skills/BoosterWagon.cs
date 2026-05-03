using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class BoosterWagon() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const int BASE_CARDS = 2;
    private const int UPGRADE_CARDS = 0;

    private const int BASE_ENERGY = 2;
    private const int UPGRADE_ENERGY = 0;

    private const decimal BASE_CHARGE_INITIAL = 2m;
    private const decimal UPGRADE_CHARGE_INITIAL = 0m;

    private const decimal BASE_CHARGE_MAX = 7m;
    private const decimal UPGRADE_CHARGE_MAX = -2m;
    
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Charge, TheEngineerCardTags.Wagon];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(BASE_CARDS),
        new EnergyVar(BASE_ENERGY),
        new ChargeInitialVar(BASE_CHARGE_INITIAL),
        new ChargeCurrentVar(BASE_CHARGE_INITIAL),
        new ChargeMaxVar(BASE_CHARGE_MAX)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CommonActions.Draw(this,choiceContext);

        if (ChargeHelper.TrySpendFullCharge(this))
        {
            await PlayerCmd.GainEnergy(
                DynamicVars.Energy.BaseValue,
                Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UPGRADE_CARDS);
        DynamicVars.Energy.UpgradeValueBy(UPGRADE_ENERGY);
        DynamicVars.ChargeInitial().UpgradeValueBy(UPGRADE_CHARGE_INITIAL);
        DynamicVars.ChargeCurrent().UpgradeValueBy(UPGRADE_CHARGE_INITIAL);
        DynamicVars.ChargeMax().UpgradeValueBy(UPGRADE_CHARGE_MAX);
    }
}