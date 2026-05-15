using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
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

    public override Task BeforeCombatStart()
    {
        SetTurnsRemaining(ACTIVE_TURNS);
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner)
            return;

        if (_turnsRemaining <= 0)
            return;

        await OrbCmd.Channel<TurretOrb>(
            new BlockingPlayerChoiceContext(),
            Owner);

        await MaterialHelper.ProduceMaterial(
            Owner,
            choiceContext,
            1,
            MaterialDestination.Hand);

        SetTurnsRemaining(_turnsRemaining - 1);

        if (_turnsRemaining <= 0)
            Status = RelicStatus.Normal;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        SetTurnsRemaining(0);
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    private void SetTurnsRemaining(int value)
    {
        AssertMutable();

        _turnsRemaining = Math.Max(0, value);

        InvokeDisplayAmountChanged();
    }
}