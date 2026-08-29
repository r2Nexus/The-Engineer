using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
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
using TheEngineer.TheEngineerCode.Cards.Attacks;

namespace TheEngineer.TheEngineerCode.Powers;

public sealed class OilPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private  IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

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
        
        if(cardSource is Liquefy) await TriggerOil(choiceContext, dealer, cardSource, spendOil: false);
        
        else await TriggerOil(choiceContext, dealer, cardSource, spendOil: true);
    }
    
    public async Task<int> TriggerOil(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        CardModel? cardSource,
        bool spendOil = true)
    {
        int triggeredAmount = Amount;

        if (triggeredAmount <= 0)
            return 0;

        Flash();

        if (spendOil)
        {
            await PowerCmd.ModifyAmount(
                choiceContext,
                this,
                -triggeredAmount,
                null,
                cardSource);

            await PowerCmd.Apply<ResiduePower>(
                choiceContext,
                Owner,
                triggeredAmount,
                dealer,
                cardSource);
        }

        var room = NCombatRoom.Instance;

        room?.CombatVfxContainer.AddChildSafely(
            NGroundFireVfx.Create(Owner)!);

        await CreatureCmd.Damage(
            choiceContext,
            Owner,
            triggeredAmount,
            ValueProp.Unpowered | ValueProp.SkipHurtAnim,
            null,
            null);

        return spendOil ? triggeredAmount : 0;
    }
}