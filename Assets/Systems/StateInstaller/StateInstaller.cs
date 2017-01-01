using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Zenject;

public class StateInstaller : MonoInstaller
{
    [SerializeField]
    [InstallPrefab(typeof(IRootState), Single=true, NonLazy=true)]
    private RootState rootStatePrefab;

    public override void InstallBindings()
    {
        Container.BindAllInterfaces<GameManager>().To<GameManager>().AsSingle();

        RecursivelyBind(this);
    }

    private void RecursivelyBind(MonoBehaviour root)
    {
        HandleInstaller<InstallStringAttribute, string>(root, (attribute, val) =>
        {
            Container.Bind<string>().WithId(attribute.Id).FromInstance(val);
        });

        HandleInstaller<InstallScriptableAssetAttribute, ScriptableObject>(root, (attribute, obj) =>
        {
            Container.BindInstance(obj);
        });

        HandleInstaller<InstallPrefabAttribute, MonoBehaviour>(root, (attribute, obj) =>
        {
            GameObjectNameGroupNameScopeArgBinder binder = Container.Bind(attribute.type).FromPrefab(obj);

            if(attribute.Single)
                binder.AsSingle();

            if(attribute.NonLazy)
                binder.NonLazy();
        }).ToList().ForEach(x => RecursivelyBind(x));
    }

    /// <summary>
    /// Executes handler for every T attribute found, passing in the U type value
    /// for every field in root
    /// </summary>
    private U[] HandleInstaller<T, U>(MonoBehaviour root, Action<T, U> handler)
    {
        FieldInfo[] fields = GetFields<U>(root);

        List<U> objects = new List<U>();

        foreach (FieldInfo field in fields)
        {
            T[] attributes = GetAttributes<T>(field);

            if (!attributes.Any())
                break;

            U val = (U)field.GetValue(root);

            foreach (T attribute in attributes)
            {
                handler(attribute, val);
            }

            objects.Add(val);
        }

        return objects.ToArray();
    }

    /// <summary>
    /// Returns all the fields within `obj` which are also MonoBehaviours
    /// </summary>
    private FieldInfo[] GetFields<T>(object obj)
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