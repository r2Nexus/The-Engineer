using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Relics;

[Pool(typeof(TheEngineerRelicPool))]
public class PocketTurret : TheEngineerRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<WartimeProduction>();
    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        ICombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber > 1)
            return;

        Flash();
        
        await OrbCmd.Channel<TurretOrb>(
            new BlockingPlayerChoiceContext(),
            Owner);
        await MaterialHelper.ProduceMaterial(Owner, choiceContext, 1, MaterialDestination.Hand);
    }
}