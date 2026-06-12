using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;

namespace TheEngineer.TheEngineerCode.Vfx;

public partial class NOilSplashVfx : Node2D
{
    public const string ScenePath = "res://TheEngineer/scenes/vfx/oil_splash.tscn";

    [Export] public int LifetimeMs = 900;

    private CancellationTokenSource? _cts;

    public override void _ExitTree()
    {
        _cts?.Cancel();
    }

    public static Node2D? Create(Creature target)
    {
        if (!ResourceLoader.Exists(ScenePath))
        {
            GD.PushError($"[Engineer VFX] Missing scene: {ScenePath}");
            return null;
        }

        var scene = GD.Load<PackedScene>(ScenePath);

        if (scene == null)
        {
            GD.PushError($"[Engineer VFX] Failed to load scene: {ScenePath}");
            return null;
        }

        var vfx = scene.Instantiate<Node2D>();

        var creatureNode = target.GetCreatureNode();
        if (creatureNode != null)
            vfx.GlobalPosition = creatureNode.VfxSpawnPosition;

        return vfx;
    }

    public override void _Ready()
    {
        foreach (var particles in FindParticles(this))
        {
            particles.Restart();
            particles.Emitting = true;
        }

        _ = DeleteAfterComplete();
    }

    private static IEnumerable<GpuParticles2D> FindParticles(Node node)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is GpuParticles2D particles)
                yield return particles;

            foreach (var nested in FindParticles(child))
                yield return nested;
        }
    }

    private async Task DeleteAfterComplete()
    {
        _cts = new CancellationTokenSource();

        try
        {
            await Task.Delay(LifetimeMs, _cts.Token);
            this.QueueFreeSafely();
        }
        catch (TaskCanceledException)
        {
        }
    }
}