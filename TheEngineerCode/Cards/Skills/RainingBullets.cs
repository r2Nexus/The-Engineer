using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;

public class RainingBullets() : TheEngineerCard(0,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 4;
    
    protected override bool HasEnergyCostX => true;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BASE_DAMAGE, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int energy = ResolveEnergyXValue();
        int turretFireCount = Mathf.Max(0,energy - 1);

        for (int i = 0; i < energy; i++)
        {
            await CommonActions.CardAttack(this, play)
                .Execute(choiceContext);
        }

        for (int i = 0; i < turretFireCount; i++)
        {
            await TurretHelper.FireAllTurrets(
                choiceContext,
                Owner);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}