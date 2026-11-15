using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class ImprovisedStrike : TheEngineerCard, IOnConsumed, IAncientAwakenedCard
{
    public ImprovisedStrike() : base(
        1,
        CardType.Attack,
        CardRarity.Token,
        TargetType.AnyEnemy)
    {
        //this.AddPerfectedCardForCustomCharacters(ModelDb.Card<Strike>());
    }
    public override CardPoolModel VisualCardPool => AncientsAwakenedInterop.PerfectedCardPool(Pool);
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(9m),
        new ExtraDamageVar(2m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(static (card, _) =>
                ((ImprovisedStrike)card)._consumeCount)
    ];
    protected override HashSet<CardTag> CanonicalTags => [
        CardTag.Strike
    ];

    private int _consumeCount = 0;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        _consumeCount = 0;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3);
    }

    public async Task OnConsumed(PlayerChoiceContext choiceContext, Player player, int amount, MaterialSource source,
        AbstractModel? causedBy, CardPlay? play)
    {
        if(player != Owner) return;
        if(amount <= 0) return;
        
        _consumeCount += 1;
    }
}