using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;

namespace TheEngineer.TheEngineerCode.Cards.Powers;

[Pool(typeof(TheEngineerCardPool))]
public class StorageChest() : TheEngineerCard(2,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_RETAIN = 2m;
    private const decimal UPGRADE_RETAIN = 1m;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StorageChestPower>(BASE_RETAIN)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<StorageChestPower>(this, DynamicVars.Power<StorageChestPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<StorageChestPower>().UpgradeValueBy(UPGRADE_RETAIN);
    }
}