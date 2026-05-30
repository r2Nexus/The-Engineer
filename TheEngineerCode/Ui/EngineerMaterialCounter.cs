using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Ui;

public partial class EngineerMaterialCounter : Control
{
	private Label? _handLabel;
	private Label? _drawLabel;
	private Label? _discardLabel;

	private Control? _handArm;
	private Control? _drawArm;
	private Control? _discardArm;

	private NEnergyCounter? _energyCounter;
	private Player? _player;

	private int _lastHand = -1;
	private int _lastDraw = -1;
	private int _lastDiscard = -1;

	private bool _wasMouseOverMaterial;
	private bool _tooltipShown;

	public override void _Ready()
	{
		SetMouseFilterRecursive(this, MouseFilterEnum.Ignore);

		_handArm = FindArm("HandArm");
		_drawArm = FindArm("DrawArm");
		_discardArm = FindArm("DiscardArm");

		_handLabel = FindCounterLabel("HandArm/HandCounter");
		_drawLabel = FindCounterLabel("DrawArm/DrawCounter");
		_discardLabel = FindCounterLabel("DiscardArm/DiscardCounter");

		if (GetParent() is NEnergyCounter energyCounter)
		{
			_energyCounter = energyCounter;
			_player = energyCounter._player;
		}

		if (!ShouldShowForPlayer(_player))
		{
			HideMaterialTooltip();
			Visible = false;
			SetProcess(false);
			return;
		}

		Visible = true;
		SetProcess(true);

		GD.Print(
			$"[TheEngineer] MaterialCounter ready. " +
			$"Player: {_player != null}, " +
			$"Engineer: {ShouldShowForPlayer(_player)}, " +
			$"HandArm: {_handArm != null}, " +
			$"DrawArm: {_drawArm != null}, " +
			$"DiscardArm: {_discardArm != null}, " +
			$"HandLabel: {_handLabel != null}, " +
			$"DrawLabel: {_drawLabel != null}, " +
			$"DiscardLabel: {_discardLabel != null}");

		Refresh(force: true);
	}

	public override void _ExitTree()
	{
		HideMaterialTooltip();
	}

	public override void _Process(double delta)
	{
		if (!ShouldShowForPlayer(_player))
		{
			HideMaterialTooltip();
			Visible = false;
			SetProcess(false);
			return;
		}

		Refresh(force: false);
		UpdateMaterialHover();
	}

	private static bool ShouldShowForPlayer(Player? player)
	{
		if (player == null)
			return false;

		return player.Character is Character.TheEngineer;
	}

	private static void SetMouseFilterRecursive(Node node, MouseFilterEnum mouseFilter)
	{
		if (node is Control control)
			control.MouseFilter = mouseFilter;

		foreach (Node child in node.GetChildren())
			SetMouseFilterRecursive(child, mouseFilter);
	}

	private Control? FindArm(string name)
	{
		return GetNodeOrNull<Control>(name)
			   ?? GetNodeOrNull<Control>($"Visual/{name}");
	}

	private Label? FindCounterLabel(string path)
	{
		return GetNodeOrNull<Label>(path)
			   ?? GetNodeOrNull<Label>($"Visual/{path}");
	}

	private void Refresh(bool force)
	{
		if (_player == null)
			return;

		int hand = MaterialHelper.CountMaterial(_player, MaterialSource.Hand);
		int draw = MaterialHelper.CountMaterial(_player, MaterialSource.Draw);
		int discard = MaterialHelper.CountMaterial(_player, MaterialSource.Discard);

		UpdateLabel(_handLabel, hand, ref _lastHand, force);
		UpdateLabel(_drawLabel, draw, ref _lastDraw, force);
		UpdateLabel(_discardLabel, discard, ref _lastDiscard, force);
	}

	private static void UpdateLabel(Label? label, int amount, ref int lastAmount, bool force)
	{
		if (label == null)
			return;

		if (!force && amount == lastAmount)
			return;

		lastAmount = amount;
		label.Text = amount.ToString();
	}

	private void UpdateMaterialHover()
	{
		bool isMouseOverMaterial = IsMouseOverMaterialCounter();

		if (isMouseOverMaterial == _wasMouseOverMaterial)
			return;

		_wasMouseOverMaterial = isMouseOverMaterial;

		if (isMouseOverMaterial)
			ShowMaterialTooltip();
		else
			HideMaterialTooltip();
	}

	public bool IsMouseOverMaterialCounter()
	{
		Vector2 mouse = GetGlobalMousePosition();

		return IsMouseOverArm(_handArm, mouse)
			   || IsMouseOverArm(_drawArm, mouse)
			   || IsMouseOverArm(_discardArm, mouse);
	}

	private static bool IsMouseOverArm(Control? arm, Vector2 mouse)
	{
		if (arm == null || !GodotObject.IsInstanceValid(arm))
			return false;

		return arm.GetGlobalRect().Grow(4f).HasPoint(mouse);
	}

	private void ShowMaterialTooltip()
	{
		if (_player == null)
			return;

		if (!ShouldShowForPlayer(_player))
			return;

		if (_tooltipShown)
			return;

		_tooltipShown = true;

		// Important:
		// Clean stale entry for this owner before showing.
		NHoverTipSet.Remove(this);

		// Also remove the energy tooltip, because the material UI overlaps it.
		if (_energyCounter != null && GodotObject.IsInstanceValid(_energyCounter))
			NHoverTipSet.Remove(_energyCounter);

		HoverTip hoverTip = BuildHoverTip();

		NHoverTipSet.CreateAndShow(this, hoverTip)
			?.SetGlobalPosition(GlobalPosition + new Vector2(-70f, -200f));
	}

	private void HideMaterialTooltip()
	{
		_tooltipShown = false;

		NHoverTipSet.Remove(this);

		if (_energyCounter == null || !GodotObject.IsInstanceValid(_energyCounter))
			return;

		if (!IsMouseOverEnergyCounter())
			return;

		// Let Material fully unregister first, then let Energy try to show again.
		_energyCounter.CallDeferred(nameof(NEnergyCounter.OnHovered));
	}

	private bool IsMouseOverEnergyCounter()
	{
		if (_energyCounter == null || !GodotObject.IsInstanceValid(_energyCounter))
			return false;

		return _energyCounter.GetGlobalRect().Grow(4f).HasPoint(GetGlobalMousePosition());
	}

	private HoverTip BuildHoverTip()
	{
		int hand = MaterialHelper.CountMaterial(_player, MaterialSource.Hand);
		int draw = MaterialHelper.CountMaterial(_player, MaterialSource.Draw);
		int discard = MaterialHelper.CountMaterial(_player, MaterialSource.Discard);

		LocString title = new(
			"static_hover_tips",
			"THEENGINEER_MATERIAL_COUNTER.title");

		LocString description = new(
			"static_hover_tips",
			"THEENGINEER_MATERIAL_COUNTER.description");

		description.Add("hand", hand.ToString());
		description.Add("draw", draw.ToString());
		description.Add("discard", discard.ToString());

		return new HoverTip(title, description);
	}
}
