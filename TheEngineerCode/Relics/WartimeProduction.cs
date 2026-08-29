using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rooms;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Relics;

[Pool(typeof(TheEngineerRelicPool))]
public class WartimeProduction : TheEngineerRelic
{
    private const int ACTIVE_TURNS = 3;

    private int _turnsRemaining;

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => _turnsRemaining;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-PRODUCEALL"),
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<TurretOrb>(),
        HoverTipFactory.FromOrb<MinerOrb>(),
        HoverTipFactory.FromCard<Material>()
    ];

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber > 1)
            return;

        Flash();
        
        await OrbCmd.Channel<TurretOrb>(
            new BlockingPlayerChoiceContext(),
            Owner);
        await OrbCmd.Channel<TurretOrb>(
            new BlockingPlayerChoiceContext(),
            Owner);
        await OrbCmd.Channel<MinerOrb>(
            new BlockingPlayerChoiceContext(),
            Owner);
        await MaterialHelper.ProduceMaterial(Owner, choiceContext, 1, MaterialDestination.Hand);
    }
}