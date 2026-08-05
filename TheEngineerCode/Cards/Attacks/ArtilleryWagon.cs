using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class ArtilleryWagon() : TheEngineerCard(
    2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AllEnemies)
{
    private const decimal BASE_DAMAGE = 8m;
    private const decimal UPGRADE_DAMAGE = 3m;

    protected override HashSet<CardTag> CanonicalTags =>
    [
        TheEngineerCardTags.Wagon
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BASE_DAMAGE, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<ArtilleryShell>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        List<CardModel> materials = PileType.Hand
            .GetPile(Owner)
            .Cards
            .Concat(PileType.Draw.GetPile(Owner).Cards)
            .Where(card =>
                card.IsTransformable &&
                card.Keywords.Contains(TheEngineerKeyWords.Material))
            .ToList();

        List<CardTransformation> transformations = [];

        foreach (CardModel material in materials)
        {
            if (CombatState != null)
            {
                CardModel shell =
                    CombatState.CreateCard<ArtilleryShell>(Owner);

                transformations.Add(
                    new CardTransformation(material, shell));
            }
        }

        if (transformations.Count > 0)
        {
            await CardCmd.Transform(
                transformations,
                null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
    }
}