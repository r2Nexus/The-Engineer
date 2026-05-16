using Godot;

namespace TheEngineer.TheEngineerCode.Ui;

[GlobalClass]
public partial class EngineerChargeSegmentOverlay : Control
{
    [Export] public Color LineColor { get; set; } = new(0f, 0f, 0f, 0.25f);
    [Export] public float LineWidthPx { get; set; } = 1.2f;

    private int _segmentCount = 1;

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public void SetSegmentCount(int segmentCount)
    {
        segmentCount = Mathf.Max(1, segmentCount);

        if (_segmentCount == segmentCount)
            return;

        _segmentCount = segmentCount;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_segmentCount <= 1)
            return;
        
        float step = Size.X / _segmentCount;

        for (int i = 1; i < _segmentCount; i++)
        {
            float x = Mathf.Round(step * i) + 0.5f;

            DrawLine(
                new Vector2(x, 0),
                new Vector2(x, Size.Y),
                LineColor,
                LineWidthPx,
                antialiased: false);
        }
    }
}