using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Orbs;

public sealed class LandMineOrb : CustomOrbModel
{
    public override Color DarkenedColor => new Color("7a5b2e");

    public override string? CustomIconPath => "res://TheEngineer/images/orbs/turret_orb.png";

    public override string? CustomPassiveSfx => "event:/sfx/characters/defect/defect_dark_passive";
    public override string? CustomEvokeSfx   => "event:/sfx/characters/defect/defect_dark_evoke";
    public override string? CustomChannelSfx => "event:/sfx/characters/defect/defect_dark_channel";

    public override decimal PassiveVal => ModifyOrbValue(3m);
    public override decimal EvokeVal   => ModifyOrbValue(3m);

    public override async Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)
        => await Passive(choiceContext, null);

    public override Node2D? CreateCustomSprite()
    {
        var container = new Node2D();

        string darkPath = SceneHelper.GetScenePath("orbs/orb_visuals/dark_orb");
        Node2D dark = PreloadManager.Cache.GetScene(darkPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);

        new MegaSprite(dark.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");

        dark.Modulate = new Color(0.65f, 0.55f, 0.2f, 1.0f);
        container.AddChild(dark);

        return container;
    }

    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (!AnyEnemyIntendsToAttack())
            return;
        
        Trigger();

        await OrbCmdHelper.EvokeSpecific(choiceContext, Owner, this);
    }

    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext choiceContext)
    {
        List<Creature> enemies = CombatState
            .GetOpponentsOf(Owner.Creature)
            .Where(e => e.IsHittable)
            .ToList();

        PlayEvokeSfx();

        await CreatureCmd.GainBlock(
            Owner.Creature,
            PassiveVal,
            ValueProp.Unpowered,
            (CardPlay)null);
        
        int corrodePower = Owner.Creature.Powers
            .OfType<CorrosiveMixturePower>()
            .FirstOrDefault()?.Amount ?? 0;
        
        int slowPower = Owner.Creature.Powers
            .OfType<SlowdownCapsulePower>()
            .FirstOrDefault()?.Amount ?? 0;

        foreach (Creature enemy in enemies)
        {
            await CommonActions.Apply<OilPower>(enemy, null, EvokeVal);
            
            if (corrodePower > 0)
            {
                await CommonActions.Apply<VulnerablePower>(enemy, null, corrodePower);
            }
            
            if (slowPower > 0)
            {
                await CommonActions.Apply<WeakPower>(enemy, null, slowPower);
            }
        }

        return enemies;
    }

    private bool AnyEnemyIntendsToAttack()
    {
        return CombatState
            .GetOpponentsOf(Owner.Creature)
            .Any(e => e.IsHittable && e.Monster != null && e.Monster.IntendsToAttack);
    }
}