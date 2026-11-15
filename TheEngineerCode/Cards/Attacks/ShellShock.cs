using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class ShellShock() : TheEngineerCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 10m;

    private const int BASE_SHELLS = 1;
    private const decimal UPGRADE_SHELLS = 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BASE_DAMAGE,ValueProp.Move),
        new CardsVar(BASE_SHELLS)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<ArtilleryShell>(),
    ];
    
    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play)
            .Execute(choiceContext);
        
        foreach (Creature ally in (IEnumerable<Creature>)CombatState.PlayerCreatures
                     .Where(c => c.IsAlive).ToList())
        {
            for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
            {
                ArtilleryShell shell = CombatState.CreateCard<ArtilleryShell>(ally.Player);
            
                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.AddGeneratedCardToCombat(
                        shell,
                        PileType.Draw,
                        ally.Player,
                        CardPilePosition.Random)
                );
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(UPGRADE_SHELLS);
    }
}