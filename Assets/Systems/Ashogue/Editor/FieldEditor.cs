using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

static class FieldEditor
{
    private static Dictionary<Type, Action<object, FieldInfo>> typeLookup = new Dictionary<Type, Action<object, FieldInfo>> {
        { typeof(bool),   BoolField },
        { typeof(float),  FloatField },
        { typeof(string), StringField }
    };

    public static void DeclaredFieldsEditor(object obj)
    {
        List<FieldInfo> fields = obj.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).ToList();
        if (!fields.Any())
            return;

        GUILayout.BeginVertical();
        foreach (FieldInfo field in fields)
        {
            GUILayout.BeginHorizontal();
            typeLookup[field.FieldType](obj, field);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private static void BoolField(object obj, FieldInfo field)
    {
        bool val = (bool)field.GetValue(obj);
        val = GUILayout.Toggle(val, field.Name);
        field.SetValue(obj, val);
    }

    private static void FloatField(object obj, FieldInfo field)
    {
        GUILayout.Label(field.Name);
        float val = (float)field.GetValue(obj);
        val = EditorGUILayout.DelayedFloatField(val);
        field.SetValue(obj, val);
    }

    private static void StringField(object obj, FieldInfo field)
    {
        GUILayout.Label(field.Name);
        string val = (string)field.GetValue(obj);
        val = EditorGUILayout.DelayedTextField(val);
        field.SetValue(obj, val);
    }
}