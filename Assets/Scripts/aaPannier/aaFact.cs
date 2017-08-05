using System;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class aaFact
{
    public enum PossibleTypes { Int, Bool, String };

    public string Key;
    public PossibleTypes ValueType;

    public object Value
    {
        get
        {
            switch (ValueType)
            {
                case PossibleTypes.Int:
                    return _intValue;
                case PossibleTypes.Bool:
                    return _boolValue;
                case PossibleTypes.String:
                    return _stringValue;
                default: return _intValue;
            }
        }

        set
        {
            switch (ValueType)
            {
                case PossibleTypes.Int:
                    _intValue = (int)value;
                    break;
                case PossibleTypes.Bool:
                    _boolValue = (bool)value;
                    break;
                case PossibleTypes.String:
                    _stringValue = (string)value;
                    break;
                default:
                    _intValue = (int)value;
                    break;
            }
        }
    }

    [XmlIgnore]
    public Type Type
    {
        get
        {
            switch (ValueType)
            {
                case PossibleTypes.Int:
                    return typeof(int);
                case PossibleTypes.Bool:
                    return typeof(bool);
                case PossibleTypes.String:
                    return typeof(string);
                default: return typeof(int);
            }
        }

        set
        {
            if (value == typeof(int))
                ValueType = PossibleTypes.Int;
            else if (value == typeof(bool))
                ValueType = PossibleTypes.Bool;
            else if (value == typeof(string))
                ValueType = PossibleTypes.String;
        }
    }

    [SerializeField]
    private int _intValue;
    [SerializeField]
    private bool _boolValue;
    [SerializeField]
    private string _stringValue;
}