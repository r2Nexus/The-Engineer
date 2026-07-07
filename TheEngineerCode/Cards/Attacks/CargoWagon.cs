using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Attacks;

[Pool(typeof(TheEngineerCardPool))]
public sealed class CargoWagon() : TheEngineerCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    private const decimal BASE_DAMAGE = 8m;
    private const decimal UPGRADE_DAMAGE = 0m;

    private const decimal BASE_PRODUCE = 1m;
    private const decimal UPGRADE_PRODUCE = 1m;
    
    protected override HashSet<CardTag> CanonicalTags => [TheEngineerCardTags.Wagon];

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(BASE_DAMAGE, ValueProp.Move),
        new ProduceVar(BASE_PRODUCE)
    };

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);

        int produceAmount = (int) DynamicVars.Produce().BaseValue;

        await MaterialHelper.ProduceMaterial(
            Owner,
            choiceContext,
            produceAmount,
            MaterialDestination.Hand,
            this);

        await MaterialHelper.ProduceMaterial(
            Owner,
            choiceContext,
            produceAmount,
            MaterialDestination.Discard,
            this);
    }

    protected override void OnUpgrade()
    {
        //DynamicVars.Damage.UpgradeValueBy(UPGRADE_DAMAGE);
        DynamicVars.Produce().UpgradeValueBy(UPGRADE_PRODUCE);
    }
}