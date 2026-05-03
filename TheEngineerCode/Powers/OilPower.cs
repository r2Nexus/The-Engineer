using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheEngineer.TheEngineerCode.Powers;

public sealed class OilPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    public override async Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner)
            return;
        
        if (!props.IsPoweredAttack())
            return;
        
        if (amount <= 0)
            return;

        var spent = Amount;
        if (Amount <= 0)
            return;

        Flash();
        
        await PowerCmd.ModifyAmount(choiceContext,this, -spent, null, null);
        
        var room = NCombatRoom.Instance;
        room?.CombatVfxContainer.AddChildSafely(
            (Node)NGroundFireVfx.Create(Owner)!);
        
        await CreatureCmd.Damage(
            choiceContext,
            Owner,
            spent,
            ValueProp.Unpowered | ValueProp.SkipHurtAnim,
            null,
            null);
    }
}