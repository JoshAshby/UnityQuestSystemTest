using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Zenject;

public class StateInstaller : MonoInstaller
{
    [InstallPrefab(typeof(IRootState))]
    [SerializeField]
    private RootState rootStatePrefab;

    [SerializeField]
    private string LoadingSceneName;

    public override void InstallBindings()
    {
        Container.Bind<string>().WithId("LoadingSceneName").FromInstance(LoadingSceneName);
        Container.BindAllInterfaces<GameManager>().To<GameManager>().AsSingle();

        RecursivelyBind(this);
    }

    private void RecursivelyBind(MonoBehaviour root)
    {
        foreach (FieldInfo field in GetFields<MonoBehaviour>(root))
        {
            MonoBehaviour obj = field.GetValue(root) as MonoBehaviour;
            InstallPrefabAttribute[] attributes = GetAttributes<InstallPrefabAttribute>(field);

            // If we don't have any InstallPrefabAttributes, assume we're not
            if (!attributes.Any())
                break;

            foreach (InstallPrefabAttribute attribute in attributes)
            {
                Container.Bind(attribute.type).FromPrefab(obj);
            }

            RecursivelyBind(obj);
        }
    }

    /// <summary>
    /// Returns all the fields within `obj` which are also MonoBehaviours
    /// </summary>
    private FieldInfo[] GetFields<T>(T obj)
    {
        Type type = obj.GetType();

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        return fields.Where(x => typeof(T).IsAssignableFrom(x.FieldType)).ToArray();
    }

    /// <summary>
    /// Returns all attributes of type T, returning them as an array
    /// </summary>
    private T[] GetAttributes<T>(FieldInfo field)
    {
        object[] attributes = field.GetCustomAttributes(typeof(T), false);

        return attributes.Select(x => (T)x).ToArray();
    }
}