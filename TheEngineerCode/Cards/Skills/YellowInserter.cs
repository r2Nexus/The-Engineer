using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class YellowInserter() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    
    private const int BASE_DURATION = 2;
    private const int UPGRADE_DURATION = 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<YellowInserterPower>(BASE_DURATION),
        new ProduceVar(1),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CommonActions.Draw(this, choiceContext);
        
        await MaterialHelper.ProduceMaterial(
            Owner,
            choiceContext,
            (int)DynamicVars.Produce().BaseValue,
            MaterialDestination.Hand,
            this);
        
        CommonActions.ApplySelf<YellowInserterPower>(this );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<YellowInserterPower>().UpgradeValueBy(UPGRADE_DURATION);
    }
}