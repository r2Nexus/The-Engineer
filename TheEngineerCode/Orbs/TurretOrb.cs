using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Orbs.Vfx;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Orbs;

public sealed class TurretOrb : CustomOrbModel
{
    public override Color DarkenedColor => new Color("6b5538");
    
    public override string? CustomIconPath => "res://TheEngineer/images/orbs/turret_orb.png";
    
    public override string? CustomPassiveSfx => "event:/sfx/characters/defect/defect_lightning_passive";
    public override string? CustomEvokeSfx => "event:/sfx/characters/defect/defect_lightning_evoke";
    public override string? CustomChannelSfx => "event:/sfx/characters/defect/defect_lightning_channel";

    private const decimal BASE_PASSIVE_DAMAGE = 7m;
    private const decimal BASE_EVOKE_DAMAGE = 9m;

    public override decimal PassiveVal =>
        ModifyOrbValue(BeltFedPower.ModifyTurretFireDamage(Owner, BASE_PASSIVE_DAMAGE));

    public override decimal EvokeVal =>
        ModifyOrbValue(BASE_EVOKE_DAMAGE);
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL")
    ];

    public override async Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)
        => await TriggerPassive(choiceContext, null);

    
    public override Node2D? CreateCustomSprite()
    {
        PackedScene scene = PreloadManager.Cache.GetScene(
            "res://TheEngineer/scenes/model/orbs/turret_orb.tscn");

        NEngineerOrbVfx visual =
            scene.Instantiate<NEngineerOrbVfx>();

        visual.InitializeForOrb(this);

        return visual;
    }
    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        bool paid = await MaterialHelper.ConsumeMaterial(
            Owner,
            choiceContext,
            1,
            MaterialSource.Hand);

        if (!paid)
            return;
        await Fire(choiceContext, target);
    }

    public async Task Fire(PlayerChoiceContext choiceContext, Creature? target)
    {
        int fireCount = BeltFedPower.GetTurretFireCount(Owner);

        for (int i = 0; i < fireCount; i++)
        {
            Creature? enemy = target ?? GetRandomEnemy();

            if (enemy == null || !enemy.IsHittable)
                return;

            ActivatePassive();
            VfxCmd.PlayOnCreature(enemy, "vfx/vfx_attack_lightning");
            PlayPassiveSfx();

            await CreatureCmd.Damage(
                choiceContext,
                new[] { enemy },
                PassiveVal,
                ValueProp.Unpowered,
                Owner.Creature);
        }
    }

    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext choiceContext)
    {
        Creature? enemy = GetRandomEnemy();
        if (enemy == null)
            return Array.Empty<Creature>();

        VfxCmd.PlayOnCreature(enemy, "vfx/vfx_attack_lightning");
        PlayEvokeSfx();

        ActivatePassive();
        await CreatureCmd.Damage(
            choiceContext,
            new[] { enemy },
            EvokeVal,
            ValueProp.Unpowered,
            Owner.Creature);

        return new[] { enemy };
    }

    private Creature? GetRandomEnemy()
    {
        List<Creature> enemies = CombatState
            .GetOpponentsOf(Owner.Creature)
            .Where(e => e.IsHittable)
            .ToList();

        return enemies.Count == 0
            ? null
            : Owner.RunState.Rng.CombatTargets.NextItem(enemies);
    }
}