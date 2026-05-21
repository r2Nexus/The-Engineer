using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Hooks;

public interface IOnConsumed
{
    Task OnConsumed(PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialSource source,
        AbstractModel? causedBy, CardPlay? play);
}