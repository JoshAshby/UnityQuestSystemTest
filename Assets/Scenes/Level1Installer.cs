using UnityEngine;
using Zenject;

public class Level1Installer : MonoInstaller<Level1Installer> {
    public override void InstallBindings () {
        Container.BindAllInterfaces<GameManager>().To<GameManager>().AsSingle();
    }
}