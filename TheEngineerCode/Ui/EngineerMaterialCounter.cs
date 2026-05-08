using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Ui;

public partial class EngineerMaterialCounter : Control
{
	private Label? _handLabel;
	private Label? _drawLabel;
	private Label? _discardLabel;

	private Player? _player;

	private int _lastHand = -1;
	private int _lastDraw = -1;
	private int _lastDiscard = -1;

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Ignore;

		_handLabel = FindCounterLabel("HandArm/HandCounter");
		_drawLabel = FindCounterLabel("DrawArm/DrawCounter");
		_discardLabel = FindCounterLabel("DiscardArm/DiscardCounter");

		if (GetParent() is NEnergyCounter energyCounter)
			_player = energyCounter._player;

		GD.Print(
			$"[TheEngineer] MaterialCounter ready. " +
			$"Player: {_player != null}, " +
			$"Hand: {_handLabel != null}, " +
			$"Draw: {_drawLabel != null}, " +
			$"Discard: {_discardLabel != null}");

		Refresh(force: true);
	}

	public override void _Process(double delta)
	{
		Refresh(force: false);
	}

	private Label? FindCounterLabel(string path)
	{
		return GetNodeOrNull<Label>($"Visual/{path}")
			   ?? GetNodeOrNull<Label>(path);
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
}
