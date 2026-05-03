using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using TheEngineer.TheEngineerCode.Cards;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Orbs;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards.Skills;


[Pool(typeof(TheEngineerCardPool))]
public class BuildLandMine() : TheEngineerCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.None)
{
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await OrbCmd.Channel<LandMineOrb>(choiceContext, Owner);

        bool consumed = await MaterialHelper.ConsumeMaterial(
            this,
            choiceContext,
            1,
            MaterialSource.Hand);

        if (consumed)
        {
            await OrbCmd.Channel<LandMineOrb>(choiceContext, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}