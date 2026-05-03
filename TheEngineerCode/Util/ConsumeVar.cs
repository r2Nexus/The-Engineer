using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheEngineer.TheEngineerCode.Util;

public sealed class ConsumeVar : DynamicVar
{
    public const string DefaultName = "Consume";

    public ConsumeVar(decimal amount) : base(DefaultName, amount)
    {
    }

    public ConsumeVar(string name, decimal amount) : base(name, amount)
    {
    }
}

public static class ConsumeVarDynamicVarSetExtensions
{
    public static DynamicVar Consume(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[ConsumeVar.DefaultName];
    }
}