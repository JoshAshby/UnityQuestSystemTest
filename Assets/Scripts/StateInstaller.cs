using System;
using System.Reflection;
using UnityEngine;
using Zenject;

public class StateInstaller : MonoInstaller
{
    [InstallPrefab(typeof(IRootState))]
    public RootState rootStatePrefab;

    [SerializeField]
    private string LoadingSceneName;

    public override void InstallBindings()
    {
        Container.Bind<string>().WithId("LoadingSceneName").FromInstance(LoadingSceneName);

        RecursivelyBind(this);
    }

    private void RecursivelyBind(MonoBehaviour root)
    {
        if (root == null)
            return;

        foreach (FieldInfo field in GetMonoBehaviorFields(root))
        {
            var installPrefabAsType = GetInstallPrefabType(field);
            if (installPrefabAsType != null)
                Container.Bind(installPrefabAsType).FromPrefab(field.GetValue(root) as MonoBehaviour);

            RecursivelyBind(field.GetValue(root) as MonoBehaviour);
        }
    }

    private FieldInfo[] GetMonoBehaviorFields(MonoBehaviour obj)
    {
        if(obj == null)
            return null;

        Type type = obj.GetType();

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
}