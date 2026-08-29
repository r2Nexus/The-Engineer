using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public sealed class ExpansionPolicy() : TheEngineerCard(
    0,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const decimal BASE_BLOCK = 4m;
    private const decimal UPGRADE_BLOCK = 1m;

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(BASE_BLOCK, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Channeling)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int x = ResolveEnergyXValue();

        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);

        for (int i = 0; i < x; i++)
        {
            await CommonActions.CardBlock(this, DynamicVars.Block, play);
        }

        if (Owner.PlayerCombatState != null)
        {
            OrbModel? leftMostOrb = Owner.PlayerCombatState
                .OrbQueue
                .Orbs
                .FirstOrDefault();

            if (leftMostOrb == null)
                return;
            
            var orbId = leftMostOrb.Id;

            for (int i = 0; i < x; i++)
            {
                OrbModel orb = ModelDb
                    .GetById<OrbModel>(orbId)
                    .ToMutable();

                await OrbCmd.Channel(
                    choiceContext,
                    orb,
                    Owner);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
    }
}