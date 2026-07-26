using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using TheEngineer.TheEngineerCode.Orbs.Vfx;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Util;
using MaterialCard = TheEngineer.TheEngineerCode.Cards.Material;

namespace TheEngineer.TheEngineerCode.Orbs;

public sealed class MinerOrb : CustomOrbModel
{
    public override Color DarkenedColor => new Color("7a6740");

    public override string? CustomIconPath => "res://TheEngineer/images/orbs/miner_orb.png";

    public override string? CustomPassiveSfx => "event:/sfx/characters/defect/defect_frost_passive";
    public override string? CustomEvokeSfx   => "event:/sfx/characters/defect/defect_frost_evoke";
    public override string? CustomChannelSfx => "event:/sfx/characters/defect/defect_frost_channel";

    public override decimal PassiveVal => 1m;
    public override decimal EvokeVal   => 2m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<MaterialCard>(),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-PRODUCEALL")
    ];
    public override Node2D? CreateCustomSprite()
    {
        PackedScene scene = PreloadManager.Cache.GetScene(
            "res://TheEngineer/scenes/model/orbs/miner_orb.tscn");

        NEngineerOrbVfx visual =
            scene.Instantiate<NEngineerOrbVfx>();

        visual.InitializeForOrb(this);

        return visual;
    }

    public override async Task AfterTurnStartOrbTrigger(PlayerChoiceContext choiceContext)
        => await Passive(choiceContext, null);

    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        PlayPassiveSfx();
        ActivatePassive();
        await MaterialHelper.ProduceMaterial(
            Owner,
            choiceContext,
            (int)PassiveVal,
            MaterialDestination.Hand,
            this);
    }

    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext choiceContext)
    {
        PlayEvokeSfx();
        ActivatePassive();
        await MaterialHelper.ProduceMaterial(
            Owner,
            choiceContext,
            (int)EvokeVal,
            MaterialDestination.Hand,
            this);

        return [];
    }
}