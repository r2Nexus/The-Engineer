using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

[Pool(typeof(TheEngineerCardPool))]
public class RainingBullets() : TheEngineerCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override bool HasEnergyCostX => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int energy = ResolveEnergyXValue();

        if (energy <= 0)
            return;

        int consumeCount = IsUpgraded
            ? Math.Max(0, energy - 1)
            : energy;

        bool consumed = consumeCount <= 0 || await MaterialHelper.ConsumeMaterial(
            this,
            choiceContext,
            consumeCount,
            MaterialSource.Stock,
            play);

        if (!consumed)
            return;

        for (int i = 0; i < energy; i++)
        {
            await TurretHelper.FireAllTurrets(
                choiceContext,
                Owner);
        }
    }

    protected override void OnUpgrade()
    {
    }
}