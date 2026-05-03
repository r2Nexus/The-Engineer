using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheEngineer.TheEngineerCode.Util;

public sealed class ProduceVar : DynamicVar
{
    public const string DefaultName = "Produce";

    public ProduceVar(decimal amount) : base(DefaultName, amount)
    {
    }

    public ProduceVar(string name, decimal amount) : base(name, amount)
    {
    }
}

public static class ProduceVarDynamicVarSetExtensions
{
    public static DynamicVar Produce(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[ProduceVar.DefaultName];
    }
}