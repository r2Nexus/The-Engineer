using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Ui;

[GlobalClass]
public partial class EngineerChargeMeter : Control
{
    private NCard? _cardNode;

    private Label? _label;
    private ProgressBar? _bar;

    private EngineerChargeSegmentOverlay? _segmentOverlay;

    private CardModel? _lastCard;
    private decimal _lastCurrent = -1;
    private decimal _lastMax = -1;

    public void SetCardNode(NCard cardNode)
    {
        _cardNode = cardNode;
    }

    public override void _Ready()
    {
        SetMouseFilterRecursive(this, MouseFilterEnum.Ignore);

        _label = GetNodeOrNull<Label>("Label");
        
        _bar = GetNodeOrNull<ProgressBar>("Bounds/Bar");

        _segmentOverlay = GetNodeOrNull<EngineerChargeSegmentOverlay>("Bounds/SegmentOverlay");

        Refresh(force: true);
    }

    public override void _Process(double delta)
    {
        Refresh(force: false);
    }

    private void Refresh(bool force)
    {
        CardModel? card = _cardNode?.Model;

        if (card == null || !ChargeHelper.HasCharge(card))
        {
            Visible = false;
            _lastCard = null;
            return;
        }

        decimal current = ChargeHelper.GetCurrent(card);
        decimal max = ChargeHelper.GetMax(card);

        if (max <= 0)
        {
            Visible = false;
            _lastCard = null;
            return;
        }

        if (!force && card == _lastCard && current == _lastCurrent && max == _lastMax)
            return;

        _lastCard = card;
        _lastCurrent = current;
        _lastMax = max;

        Visible = true;

        if (_label != null)
            _label.Text = $"{current:0}/{max:0}";

        if (_bar != null)
        {
            _bar.MinValue = 0;
            _bar.MaxValue = (double)max;
            _bar.Value = (double)current;
        }

        SetSegments(max);
    }

    private void SetSegments(decimal max)
    {
        _segmentOverlay?.SetSegmentCount(Mathf.Max(1, (int)max));
    }

    private static void SetMouseFilterRecursive(Node node, MouseFilterEnum mouseFilter)
    {
        if (node is Control control)
            control.MouseFilter = mouseFilter;

        foreach (Node child in node.GetChildren())
            SetMouseFilterRecursive(child, mouseFilter);
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard._Ready))]
public static class NCardReadyChargeMeterPatch
{
    private const string ScenePath =
        "res://TheEngineer/scenes/EngineerChargeMeter.tscn";

    [HarmonyPostfix]
    public static void Postfix(NCard __instance)
    {
        if (__instance.GetNodeOrNull<EngineerChargeMeter>(
                nameof(EngineerChargeMeter)) != null)
        {
            return;
        }

        PackedScene scene = GD.Load<PackedScene>(ScenePath);
        EngineerChargeMeter meter =
            scene.Instantiate<EngineerChargeMeter>();

        meter.Name = nameof(EngineerChargeMeter);
        meter.SetCardNode(__instance);

        meter.MouseFilter = Control.MouseFilterEnum.Ignore;
        meter.Position = new Vector2(110, -235);
        meter.ZIndex = 0;
        meter.ZAsRelative = true;

        __instance.AddChild(meter);
    }
}