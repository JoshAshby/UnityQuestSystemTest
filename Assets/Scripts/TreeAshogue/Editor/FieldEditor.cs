using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using TreeAshogue.Data;

static class FieldEditor
{
    private static Dictionary<Type, Action<object, PropertyInfo>> propertyLookup = new Dictionary<Type, Action<object, PropertyInfo>> {
        { typeof(bool),   BoolProperty },
        { typeof(float),  FloatProperty },
        { typeof(string), StringProperty }
    };

    public static void DeclaredFieldsEditor(object obj)
    {
        List<PropertyInfo> properties = obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly)
            .ToList();

        GUILayout.BeginVertical();
        foreach (PropertyInfo property in properties)
        {
            GUILayout.BeginHorizontal();
            propertyLookup[property.PropertyType](obj, property);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private static void BoolProperty(object obj, PropertyInfo property)
    {
        bool val = (bool)property.GetValue(obj, null);
        val = GUILayout.Toggle(val, property.Name);
        property.SetValue(obj, val, null);
    }

    private static void FloatProperty(object obj, PropertyInfo property)
    {
        GUILayout.Label(property.Name);
        float val = (float)property.GetValue(obj, null);
        val = EditorGUILayout.DelayedFloatField(val);
        property.SetValue(obj, val, null);
    }

    private static void StringProperty(object obj, PropertyInfo property)
    {
        GUILayout.Label(property.Name);
        string val = (string)property.GetValue(obj, null);
        val = EditorGUILayout.DelayedTextField(val);
        property.SetValue(obj, val, null);
    }
}