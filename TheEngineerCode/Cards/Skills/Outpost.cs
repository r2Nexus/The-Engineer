using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class Outpost() : TheEngineerCard(
    1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    private const decimal BASE_BLOCK = 5m;
    private const decimal UPGRADE_BLOCK = 2m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling),
        HoverTipFactory.FromOrb<MinerOrb>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(BASE_BLOCK, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<MinerOrb>(choiceContext, Owner);

        int minerCount = TurretHelper.GetOrbs<MinerOrb>(Owner).Count;

        for (int i = 0; i < minerCount; i++)
            await CommonActions.CardBlock(this, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
    }
}