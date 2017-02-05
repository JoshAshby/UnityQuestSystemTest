using System;

[AttributeUsage(AttributeTargets.Field)]
public class InstallStringAttribute : Attribute
{
    private string m_id;

    public string Id {
        get { return m_id; }
    }

    public InstallStringAttribute(string id)
    {
        this.m_id = id;
    }
}