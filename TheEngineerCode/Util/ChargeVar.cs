using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheEngineer.TheEngineerCode.Util;

public sealed class ChargeInitialVar : DynamicVar
{
    public const string DefaultName = "ChargeInitial";

    public ChargeInitialVar(decimal amount) : base(DefaultName, amount)
    {
    }

    public ChargeInitialVar(string name, decimal amount) : base(name, amount)
    {
    }
}

public sealed class ChargeCurrentVar : DynamicVar
{
    public const string DefaultName = "ChargeCurrent";

    public ChargeCurrentVar(decimal amount) : base(DefaultName, amount)
    {
    }

    public ChargeCurrentVar(string name, decimal amount) : base(name, amount)
    {
    }
}

public sealed class ChargeMaxVar : DynamicVar
{
    public const string DefaultName = "ChargeMax";

    public ChargeMaxVar(decimal amount) : base(DefaultName, amount)
    {
    }

    public ChargeMaxVar(string name, decimal amount) : base(name, amount)
    {
    }
}

public static class ChargeDynamicVarSetExtensions
{
    public static DynamicVar ChargeInitial(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[ChargeInitialVar.DefaultName];
    }

    public static DynamicVar ChargeCurrent(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[ChargeCurrentVar.DefaultName];
    }

    public static DynamicVar ChargeMax(this DynamicVarSet dynamicVars)
    {
        return dynamicVars[ChargeMaxVar.DefaultName];
    }
}