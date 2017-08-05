using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class aaCriterion : ScriptableObject
{
    public enum Comparisons
    {
        Is, IsNot,             // bool
        StringEq, StringNotEq, // string
        Eq, NotEq,             // int
        Lt, Lte,               // int
        Gt, Gte                // int
    }

    public string Hint = "global";
    public string FactKey = "";

    private object _blackboardValue
    {
        get { return aaPannier.Instance.BlackboardDatabase.GetFact(Hint, FactKey); }
    }

    public object BlackboardValue
    {
        get
        {
            if (Comparitor == Comparisons.Is || Comparitor == Comparisons.IsNot)
                return (_blackboardValue as bool?) ?? default(bool);

            if (Comparitor == Comparisons.StringEq || Comparitor == Comparisons.StringNotEq)
                return (_blackboardValue as string) != null ? (_blackboardValue as string) : default(string);

            return (_blackboardValue as int?) ?? default(int);
        }
    }

    public Comparisons Comparitor = Comparisons.Is;

    [SerializeField]
    private object _comparisonValue;
    public object ComparisonValue
    {
        get
        {
            if (Comparitor == Comparisons.Is || Comparitor == Comparisons.IsNot)
                return (_comparisonValue as bool?) ?? default(bool);

            if (Comparitor == Comparisons.StringEq || Comparitor == Comparisons.StringNotEq)
                return (_comparisonValue as string) != null ? (_comparisonValue as string) : default(string);

            return (_comparisonValue as int?) ?? default(int);
        }
        set { _comparisonValue = value; }
    }

    public Type ComparisonType
    {
        get
        {
            if (Comparitor == Comparisons.Is || Comparitor == Comparisons.IsNot)
                return typeof(bool);

            if (Comparitor == Comparisons.StringEq || Comparitor == Comparisons.StringNotEq)
                return typeof(string);

            return typeof(int);
        }
    }

    public bool Check(aaEvent @event)
    {
        switch (Comparitor)
        {
            default: return false;
            case Comparisons.Is:
                return (bool)ComparisonValue == (bool)BlackboardValue;
            case Comparisons.IsNot:
                return (bool)ComparisonValue != (bool)BlackboardValue;
            case Comparisons.StringEq:
                return (string)ComparisonValue == (string)BlackboardValue;
            case Comparisons.StringNotEq:
                return (string)ComparisonValue != (string)BlackboardValue;
            case Comparisons.Eq:
                return (int)ComparisonValue == (int)BlackboardValue;
            case Comparisons.NotEq:
                return (int)ComparisonValue != (int)BlackboardValue;
            case Comparisons.Lt:
                return (int)ComparisonValue < (int)BlackboardValue;
            case Comparisons.Lte:
                return (int)ComparisonValue <= (int)BlackboardValue;
            case Comparisons.Gt:
                return (int)ComparisonValue > (int)BlackboardValue;
            case Comparisons.Gte:
                return (int)ComparisonValue >= (int)BlackboardValue;
        }
    }

    public void OnCustomGUI()
    {
#if UNITY_EDITOR
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        Hint = EditorGUILayout.TextField(GUIContent.none, Hint);
        FactKey = EditorGUILayout.TextField(GUIContent.none, FactKey);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        Comparitor = (Comparisons)EditorGUILayout.EnumPopup(GUIContent.none, Comparitor);

        if (ComparisonType == typeof(bool))
            ComparisonValue = GUILayout.Toggle((bool)ComparisonValue, "");
        if (ComparisonType == typeof(int))
            ComparisonValue = EditorGUILayout.IntField(GUIContent.none, (int)ComparisonValue);
        if (ComparisonType == typeof(string))
            ComparisonValue = EditorGUILayout.TextField(GUIContent.none, (string)ComparisonValue);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
#endif
    }
}