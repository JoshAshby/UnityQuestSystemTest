using System;

[AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false)]
public class aaResponseAttribute : Attribute
{
    protected string _displayName = "";
    public string DisplayName
    {
        get { return _displayName; }
        set { _displayName = value; }
    }
}