using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class CorrosiveMixture() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_CORRODE = 1;
    private const decimal UPGRADE_CORRODE = 1;
    
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ConsumeVar(2),
        new PowerVar<CorrosiveMixturePower>(BASE_CORRODE)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await OrbCmd.Channel<LandMineOrb>(choiceContext, Owner);

        bool consumed = await MaterialHelper.ConsumeMaterial(
            this, 
            choiceContext, 
            (int)DynamicVars.Consume().BaseValue,
            MaterialSource.Hand);
        if (consumed)
        {
            await CommonActions.ApplySelf<CorrosiveMixturePower>(this, DynamicVars.Power<CorrosiveMixturePower>().BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<CorrosiveMixturePower>().UpgradeValueBy(UPGRADE_CORRODE);
    }
}