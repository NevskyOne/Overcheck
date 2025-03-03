using Zenject;

public class BindingsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<JsonSaver>().AsSingle();

        Container.Bind<EventBus>().AsSingle();
        Container.Bind<NPCDataBaseService>().AsSingle();
        
        Container.Bind<VisualEffects>().AsSingle();
        Container.Bind<DialogSystem>().AsSingle(); 
        Container.Bind<CameraManager>().AsSingle();
        Container.Bind<Player>().AsSingle();
        Container.Bind<MainUI>().AsSingle();
    }
}