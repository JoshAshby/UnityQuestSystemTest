using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StateInstaller : MonoInstaller
{
    [InstallPrefab(typeof(IRootState))]
    public RootState rootStatePrefab;

    private Dictionary<Type, Action<FieldInfo, object, MonoBehaviour>> InstallerMap;

    public override void InstallBindings()
    {
        InstallerMap = new Dictionary<Type, Action<FieldInfo, object, MonoBehaviour>> {
          { typeof(InstallPrefabAttribute), HandlePrefabInstall },
          { typeof(InstallStringAttribute), HandleStringInstall }
        };

        RecursivelyBind(this);
    }

    private void RecursivelyBind(MonoBehaviour root)
    {
        if (root == null)
            return;

        foreach (FieldInfo field in GetMonoBehaviorFields(root))
        {
            HandleAttributes(field, root);
            // var installPrefabAsType = GetInstallPrefabType(field);
            // if (installPrefabAsType != null)
            //     Container.Bind(installPrefabAsType).FromPrefab(field.GetValue(root) as MonoBehaviour);

            RecursivelyBind(field.GetValue(root) as MonoBehaviour);
        }
    }

    private FieldInfo[] GetMonoBehaviorFields(MonoBehaviour obj)
    {
        if(obj == null)
            return null;

        Type type = obj.GetType();

        /// <todo>
        /// Ensure this only searches through things for MonoBehaviour's and not just everything
        /// Since its only looking for State prefabs
        /// </todo>
        return type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    }

    private Type GetInstallPrefabType(FieldInfo field)
    {
        object[] attributes = field.GetCustomAttributes(typeof(InstallPrefabAttribute), false);

        foreach (object attribute in attributes)
        {
            if (attribute is InstallPrefabAttribute)
                return (attribute as InstallPrefabAttribute).type;
        }

        return null;
    }

    private void HandleAttributes(FieldInfo field, MonoBehaviour root)
    {

        object[] attributes = field.GetCustomAttributes(false);

        foreach (object attribute in attributes)
        {
            Type type = attribute.GetType();

            if (InstallerMap.ContainsKey(type))
                InstallerMap[attribute.GetType()](field, attribute, root);
        }
    }

    private void HandlePrefabInstall(FieldInfo field, object attribute, MonoBehaviour root)
    {
        Container.Bind((attribute as InstallPrefabAttribute).type).FromPrefab(field.GetValue(root) as MonoBehaviour);
    }

    private void HandleStringInstall(FieldInfo field, object attribute, MonoBehaviour root)
    {
        Container.Bind(typeof(string)).WithId((attribute as InstallStringAttribute).Id).FromInstance(field.GetValue(root) as string);
    }
}