using System;

[AttributeUsage(AttributeTargets.Field)]
public class InstallPrefabAttribute : Attribute
{
    private Type m_type;
    private bool m_single = false;
    private bool m_nonLazy = true;

    public Type type {
        get { return m_type; }
    }

    public bool Single {
        get { return m_single; }
        set { m_single = value; }
    }

    public bool NonLazy {
        get { return m_nonLazy; }
        set { m_nonLazy = value; }
    }

    public InstallPrefabAttribute(Type type)
    {
        this.m_type = type;
    }
}