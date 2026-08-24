using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Relics;

public sealed class ProductivityModule : TheEngineerRelic
{
    private bool _triggeredThisTurn;

    public override RelicRarity Rarity => RelicRarity.Rare;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-CONSUMEALL"),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-PRODUCEALL")
    ];

    public async Task OnConsumed(
        PlayerChoiceContext choiceContext,
        Player player,
        int amount,
        MaterialSource source,
        AbstractModel? causedBy)
    {
        if (player != Owner)
            return;

        if (_triggeredThisTurn)
            return;

        if (amount <= 0)
            return;

        _triggeredThisTurn = true;

        Flash();

        await MaterialHelper.ProduceMaterial(
            player,
            choiceContext,
            1,
            MaterialDestination.Discard,
            this);
    }

    public override Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player == Owner)
            _triggeredThisTurn = false;

        return Task.CompletedTask;
    }
}