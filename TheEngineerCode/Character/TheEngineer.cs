using BaseLib.Abstracts;
using BaseLib.Utils;
using BaseLib.Utils.NodeFactories;
using TheEngineer.TheEngineerCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using TheEngineer.TheEngineerCode.Cards.Attacks;
using TheEngineer.TheEngineerCode.Cards.Skills;
using TheEngineer.TheEngineerCode.Relics;
using TheEngineer.TheEngineerCode.Ui;
using MaterialCard = TheEngineer.TheEngineerCode.Cards.Material;

namespace TheEngineer.TheEngineerCode.Character;


public class TheEngineer : PlaceholderCharacterModel
{
    public const string CharacterId = "TheEngineer";

    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 70;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<MaterialCard>(),
        ModelDb.Card<BuildTurret>(),
        ModelDb.Card<ManualLabor>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<PocketTurret>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<TheEngineerCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<TheEngineerRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<TheEngineerPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override Color EnergyLabelOutlineColor =>
        new Color("#2b2418");
    public override string? CustomEnergyCounterPath =>
        "res://TheEngineer/scenes/engineer_energy_counter.tscn";
    
    public static readonly AddedNode<NEnergyCounter, EngineerMaterialCounter> MaterialCounterNode =
        new(parent =>
        {
            var counter = new EngineerMaterialCounter
            {
                Name = "EngineerMaterialCounter",
                MouseFilter = Control.MouseFilterEnum.Ignore
            };

            counter.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
            counter.Position = Vector2.Zero;
            counter.Size = new Vector2(128, 128);

            var visualScene = ResourceLoader.Load<PackedScene>(
                "res://TheEngineer/scenes/engineer_material_counter.tscn");

            var visual = visualScene.Instantiate<Control>();
            visual.Name = "Visual";
            visual.MouseFilter = Control.MouseFilterEnum.Ignore;

            counter.AddChild(visual);

            return counter;
        });

    public override int BaseOrbSlotCount => 3;
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    
    
}