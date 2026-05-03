using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using TheEngineer.TheEngineerCode.Cards.Skills;

namespace TheEngineer.TheEngineerCode.Powers;

public class PumpjackPower :TheEngineerPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner.Player)
            return;

        for (int i = 0; i < Amount; ++i)
        {
            CardModel card = combatState.CreateCard<OilBarrel>(Owner.Player);

            await CardPileCmd.AddGeneratedCardToCombat(
                card,
                PileType.Hand,
                Owner.Player);
        }
    }
}