using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Hooks;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Character;

public class TheEngineerSingleton()
    : CustomSingletonModel(true, true), IOnConsumed
{
    public Task OnConsumed(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialSource source,
        AbstractModel? causedBy)
        => ChargeHelper.OnConsumed(
            choiceContext,
            player,
            amount,
            source,
            causedBy);
}