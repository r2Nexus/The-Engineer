using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Powers;
public sealed class RailSignalPower : TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(Amount, ValueProp.Move)
    ];

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (play.Card.Owner != Owner.Player)
            return;

        if (!play.Card.Tags.Contains(TheEngineerCardTags.Wagon))
            return;

        Flash();

        await CreatureCmd.GainBlock(
            Owner,
            Amount,
            ValueProp.Unpowered,
            play);

        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -Amount,
            null,
            null);
    }
}