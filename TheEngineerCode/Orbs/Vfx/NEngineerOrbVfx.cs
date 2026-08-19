using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Orbs;

namespace TheEngineer.TheEngineerCode.Orbs.Vfx;

public partial class NEngineerOrbVfx : NOrbVfx
{
	private AnimationPlayer? _animationPlayer;
	private bool _evokeHandled;

	public void InitializeForOrb(OrbModel orbModel)
	{
		if (_orbModel != null)
			return;

		Initialize(orbModel);

		GD.Print(
			$"NEngineerOrbVfx initialized for {orbModel.Id.Entry}");
	}

	public override void _Ready()
	{
		base._Ready();

		GD.Print(
			$"NEngineerOrbVfx ready. Runtime type: {GetType().FullName}");

		_animationPlayer =
			GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

		if (_animationPlayer == null)
		{
			GD.PushWarning(
				"NEngineerOrbVfx could not find AnimationPlayer.");
			return;
		}

		GD.Print(
			$"Orb animations: {string.Join(", ", _animationPlayer.GetAnimationList())}");

		PlayAnimation("idle_loop");
	}

	public override void OnPassiveActivated(
		decimal passiveVal,
		decimal evokeVal)
	{
		base.OnPassiveActivated(passiveVal, evokeVal);

		GD.Print("NEngineerOrbVfx passive activated");

		RestartParticles("Passive");
		PlayAnimation("Passive", queueIdle: true);
	}

	protected override void OnEvokeInternal(
		Vector2 targetVfxSpawnPosition)
	{
		base.OnEvokeInternal(targetVfxSpawnPosition);

		if (_evokeHandled)
			return;

		_evokeHandled = true;

		GD.Print("NEngineerOrbVfx evoke activated");

		SpawnDetachedParticles("Evoke");
		PlayAnimation("Evoke");

		Callable
			.From(() => _evokeHandled = false)
			.CallDeferred();
	}

	private void RestartParticles(string nodeName)
	{
		Node? node = FindChild(
			nodeName,
			recursive: true,
			owned: false);

		switch (node)
		{
			case GpuParticles2D gpu:
				gpu.Restart();
				gpu.Emitting = true;
				break;

			case CpuParticles2D cpu:
				cpu.Restart();
				cpu.Emitting = true;
				break;
		}
	}

	private void PlayAnimation(
		string animationName,
		bool queueIdle = false)
	{
		if (_animationPlayer == null)
			return;

		StringName animation = new(animationName);

		if (!_animationPlayer.HasAnimation(animation))
		{
			GD.PushWarning(
				$"Missing orb animation: {animationName}");
			return;
		}

		_animationPlayer.Play(animation);

		if (queueIdle &&
			_animationPlayer.HasAnimation("idle_loop"))
		{
			_animationPlayer.Queue("idle_loop");
		}
	}
	
	private void SpawnDetachedParticles(string nodeName)
{
	Node? source = FindChild(
		nodeName,
		recursive: true,
		owned: false);

	if (source == null)
	{
		GD.PushWarning(
			$"Orb particle node '{nodeName}' was not found.");
		return;
	}

	GD.Print(
		$"Found particle node '{source.GetPath()}' " +
		$"with type {source.GetType().FullName}");

	switch (source)
	{
		case GpuParticles2D gpuSource:
			SpawnDetachedGpuParticles(gpuSource);
			break;

		case CpuParticles2D cpuSource:
			SpawnDetachedCpuParticles(cpuSource);
			break;

		default:
			GD.PushWarning(
				$"Node '{nodeName}' is not a 2D particle node. " +
				$"Actual type: {source.GetType().FullName}");
			break;
	}
}

private void SpawnDetachedGpuParticles(
	GpuParticles2D source)
{
	if (VfxContainer == null)
		return;

	Transform2D originalTransform =
		source.GlobalTransform;

	GpuParticles2D particles =
		(GpuParticles2D)source.Duplicate();

	particles.Name =
		$"{source.Name}_Detached";

	particles.Emitting = false;
	particles.OneShot = true;
	particles.Visible = true;

	VfxContainer.AddChild(particles);

	particles.GlobalTransform =
		originalTransform;
	
	particles.Restart();

	QueueParticleCleanup(
		particles,
		particles.Lifetime + 0.5);
}

private void SpawnDetachedCpuParticles(
	CpuParticles2D source)
{
	if (VfxContainer == null)
		return;

	Transform2D originalTransform =
		source.GlobalTransform;

	CpuParticles2D particles =
		(CpuParticles2D)source.Duplicate();

	particles.Name =
		$"{source.Name}_Detached";

	particles.Emitting = false;
	particles.OneShot = true;
	particles.Visible = true;

	VfxContainer.AddChild(particles);

	particles.GlobalTransform =
		originalTransform;

	particles.Restart();

	QueueParticleCleanup(
		particles,
		particles.Lifetime + 0.5);
}

private void QueueParticleCleanup(
	Node particles,
	double delay)
{
	SceneTreeTimer timer =
		GetTree().CreateTimer(delay);

	timer.Timeout += () =>
	{
		if (GodotObject.IsInstanceValid(particles))
			particles.QueueFree();
	};
}
}
