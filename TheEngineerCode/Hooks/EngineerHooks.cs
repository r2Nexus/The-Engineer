using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Hooks;

public static class EngineerHooks
{
    private static async Task Dispatch<T>(
        ICombatState combatState,
        PlayerChoiceContext choiceContext,
        Func<T, Task> action)
        where T : class
    {
        foreach (T listener in combatState.IterateHookListeners().OfType<T>())
        {
            AbstractModel model = (AbstractModel)(object)listener;

            choiceContext.PushModel(model);
            await action(listener);
            choiceContext.PopModel(model);
        }
    }

    public static Task OnConsumed(
        ICombatState? combatState,
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialSource source,
        AbstractModel? causedBy,
        CardPlay? play = null)
    {
        return Dispatch<IOnConsumed>(
            combatState,
            choiceContext,
            listener => listener.OnConsumed(
                choiceContext,
                player,
                amount,
                source,
                causedBy, 
                play));
    }
    
    public static Task OnProduced(
        ICombatState? combatState,
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialDestination destination,
        AbstractModel? causedBy)
    {
        if (combatState == null || amount <= 0)
            return Task.CompletedTask;

        return Dispatch<IOnProduced>(
            combatState,
            choiceContext,
            listener => listener.OnProduced(
                choiceContext,
                player,
                amount,
                destination,
                causedBy));
    }
    
    public static Task OnChargeSpent(
        ICombatState? combatState,
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel? spentCard,
        decimal amount,
        AbstractModel? causedBy)
    {
        if (combatState == null || amount <= 0)
            return Task.CompletedTask;

        return Dispatch<IOnChargeSpent>(
            combatState,
            choiceContext,
            listener => listener.OnChargeSpent(
                choiceContext,
                player,
                spentCard,
                amount,
                causedBy));
    }
}