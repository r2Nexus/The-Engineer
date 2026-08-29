using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class SteamBattery() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    private const decimal BASE_BLOCK = 9m;
    private const decimal UPGRADE_BLOCK = 3m;
    private const decimal CONSUME = 1;

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(BASE_BLOCK, ValueProp.Move),
        new ConsumeVar(CONSUME),
        new EnergyVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, DynamicVars.Block, play);
        
        var consumed = await MaterialHelper.ConsumeMaterial(this, choiceContext, (int)DynamicVars.Consume().BaseValue, MaterialSource.Hand, play);
        if(consumed) await CommonActions.ApplySelf<EnergyNextTurnPower>(this, DynamicVars.Energy.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
    }
}