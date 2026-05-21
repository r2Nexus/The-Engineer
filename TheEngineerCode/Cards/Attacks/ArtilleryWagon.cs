using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class ArtilleryWagon : TheEngineerCard
{
    private const decimal BASE_DAMAGE = 18;
    private const decimal UPGRADE_DAMAGE = 0;

    private const decimal BASE_CONSUME = 3;
    private const decimal BASE_REPLAY = 2;
    private const decimal UPPGRADE_REPLAY = 1;

    public ArtilleryWagon() : base(2,
        CardType.Attack, CardRarity.Rare,
        TargetType.AllEnemies)
    {
        _baseReplayCount = (int)BASE_REPLAY;
    }
    
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Wagon];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-STOCK")
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BASE_DAMAGE,ValueProp.Move),
        new ConsumeVar(BASE_CONSUME)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        bool consumed = await MaterialHelper.ConsumeMaterial(this, choiceContext, (int)DynamicVars.Consume().BaseValue,
            MaterialSource.Stock, play);
        if (consumed) await CommonActions.CardAttack(this, play).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        //DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
        BaseReplayCount += (int)BASE_REPLAY;
    }
}