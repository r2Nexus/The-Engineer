using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheEngineer.TheEngineerCode.Hooks;

public interface IOnChargeSpent
{
    Task OnChargeSpent(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel? spentCard,
        decimal amount,
        AbstractModel? causedBy);
}