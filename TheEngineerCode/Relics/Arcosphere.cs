using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Relics;

[Pool(typeof(TheEngineerRelicPool))]
public class Arcosphere() : TheEngineerRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    private const int BASE_MATERIAL_REQS = 6;

    private int _materialReqs = BASE_MATERIAL_REQS;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(1),
        EngineerHoverTips.GetStaticHoverTip("THEENGINEER-STOCK")
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1),

        new DisplayVar<Arcosphere>(
            "MaterialAmount",
            relic => relic._materialReqs.ToString())
        {
            BaseValue = BASE_MATERIAL_REQS
        }
    ];

    public override Task BeforeCombatStart()
    {
        _materialReqs = BASE_MATERIAL_REQS;

        DynamicVars["MaterialAmount"].ResetToBase();

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return;

        int material = MaterialHelper.CountMaterial(
            Owner,
            MaterialSource.Stock);

        if (material < _materialReqs)
            return;

        Flash();

        await CommonActions.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature,
            this);

        _materialReqs++;

        DynamicVars["MaterialAmount"].PreviewValue = _materialReqs;
    }
}