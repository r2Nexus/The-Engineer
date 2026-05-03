using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Hooks;

public interface IOnProduced
{
    Task OnProduced(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialDestination destination,
        AbstractModel? causedBy);
}