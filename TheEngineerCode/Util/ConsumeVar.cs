using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheEngineer.TheEngineerCode.Util;

public sealed class ConsumeVar : DynamicVar
{
    public const string DefaultName = "Consume";

    public ConsumeVar(decimal amount) : base(DefaultName, amount)
    {
        this.WithTooltip();
    }

    public ConsumeVar(string name, decimal amount) : base(name, amount)
    {
        this.WithTooltip();
    }
}

public static class ConsumeVarDynamicVarSetExtensions
{
    public static DynamicVar Consume(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[ConsumeVar.DefaultName];
    }
}