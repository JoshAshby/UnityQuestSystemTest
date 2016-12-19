using System;

[AttributeUsage(AttributeTargets.Field)]
public class InstallPrefabAttribute : Attribute
{
    private Type mtype;

    public Type type {
        get { return mtype; }
    }

    public InstallPrefabAttribute(Type type)
    {
        this.mtype = type;
    }
}