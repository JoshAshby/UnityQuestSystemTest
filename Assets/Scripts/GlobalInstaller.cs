using Zenject;

public class GlobalInstaller : MonoInstaller<GlobalInstaller>
{
    public override void InstallBindings()
    {
        Container.BindAllInterfaces<GameManager>().To<GameManager>().AsSingle();
    }
}