using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TheEngineer.TheEngineerCode.Character;
using TheEngineer.TheEngineerCode.Powers;
using TheEngineer.TheEngineerCode.Relics;
using TheEngineer.TheEngineerCode.Util;

namespace TheEngineer.TheEngineerCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Material() : TheEngineerCard(
    -1,
    CardType.Skill,
    CardRarity.Token,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(3,ValueProp.Move)
    ];

    private bool HasTungstenCarbide =>
        Owner?.GetRelic<TungstenCarbide>() != null;
    protected override bool IsPlayable =>
        HasTungstenCarbide;

    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        TheEngineerKeyWords.Material
    ];
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);

        description.Add(
            "HasTungstenCarbide",
            HasTungstenCarbide);
    }

    public override void AfterCreated()
    {
        base.AfterCreated();
        RefreshTungstenCarbide();
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        TungstenCarbide? relic = Owner.GetRelic<TungstenCarbide>();

        if (relic == null)
            return;

        relic.Flash();

        await CommonActions.CardBlock(this, play);
    }

    protected override void OnUpgrade()
    {
    }
    
    public void RefreshTungstenCarbide()
    {
        if (Owner == null)
            return;

        EnergyCost.SetCustomBaseCost(
            Owner.GetRelic<TungstenCarbide>() != null
                ? 0
                : -1);
    }
}