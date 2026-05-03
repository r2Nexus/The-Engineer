using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

public class BlueBelt : TheEngineerCard
{
    private const int BASE_REPLAY = 1;
    private const int UPGRADE_REPLAY = 1;
    
    public BlueBelt() : base(0,
        CardType.Skill, CardRarity.Rare,
        TargetType.Self)

    {
        _baseReplayCount = BASE_REPLAY;
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ConsumeVar(1),
        new CardsVar(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        bool consumed = await MaterialHelper.ConsumeMaterial(this, choiceContext, (int)DynamicVars.Consume().BaseValue,
            MaterialSource.Hand);
        if (consumed) await CommonActions.Draw(this,choiceContext);
    }

    protected override void OnUpgrade()
    {
        _baseReplayCount += UPGRADE_REPLAY;
    }
}