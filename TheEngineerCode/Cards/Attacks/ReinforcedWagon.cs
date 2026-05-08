using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public class ReinforcedWagon() : TheEngineerCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{

    private const decimal BASE_DAMAGE = 8;
    private const decimal BASE_BLOCK = 3;
    private const decimal UPGRADE_BLOCK = 1;
    
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Wagon];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BASE_DAMAGE,ValueProp.Move),
        new BlockVar(BASE_BLOCK, ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        int material = MaterialHelper.CountMaterial(this, MaterialSource.Hand);
        await CreatureCmd.GainBlock(
            Owner.Creature,
            material * DynamicVars.Block.BaseValue,
            ValueProp.Unpowered,
            play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(UPGRADE_BLOCK);
    }
}