using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class Pipeline() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        int oilAmount = cardPlay.Target.Powers
            .OfType<OilPower>()
            .FirstOrDefault()?.Amount ?? 0;

        if (oilAmount <= 0)
            return;

        List<Creature> enemies = CombatState.HittableEnemies
            .Where(enemy => IsUpgraded || enemy != cardPlay.Target)
            .ToList();

        foreach (Creature enemy in enemies)
        {
            await CommonActions.Apply<OilPower>(
                enemy,
                this,
                oilAmount);
        }

        await MaterialHelper.ProduceMaterial(this, choiceContext, 1, MaterialDestination.Hand);
    }

    protected override void OnUpgrade()
    {
    }
}