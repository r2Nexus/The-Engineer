using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class Flamethrower : TheEngineerCard
{
    private const decimal BASE_DAMAGE = 4m;
    private const decimal BASE_OIL = 5m;

    private const int BASE_REPLAY = 2;
    private const int UPGRADE_REPLAY = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OilPower>()
    ];
    public Flamethrower() : base(
        2,
        CardType.Attack,
        CardRarity.Uncommon,
        TargetType.AnyEnemy)
    {
        _baseReplayCount = BASE_REPLAY;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BASE_DAMAGE, ValueProp.Move),
        new PowerVar<OilPower>(BASE_OIL),
        new ConsumeVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        
        await CommonActions.CardAttack(this, play.Target)
            .Execute(choiceContext);

        bool consumed = await MaterialHelper.ConsumeMaterial(this, choiceContext, 1, MaterialSource.Hand);

        if (consumed)
        {
            await CommonActions.Apply<OilPower>(play.Target,this,DynamicVars.Power<OilPower>().BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        BaseReplayCount += UPGRADE_REPLAY;
    }
}