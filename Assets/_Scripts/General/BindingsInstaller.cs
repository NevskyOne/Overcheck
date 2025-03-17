using Zenject;

public class BindingsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<JsonSaver>().AsSingle();

        Container.Bind<EventBus>().AsSingle();
        Container.Bind<NPCDataBaseService>().AsSingle();
        
        Container.Bind<DocumentControlService>().FromComponentInHierarchy().AsSingle();
        Container.Bind<NPCService>().FromComponentInHierarchy().AsSingle();
        Container.Bind<VisualEffects>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DialogSystem>().FromComponentInHierarchy().AsSingle(); 
        Container.Bind<CameraManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Player>().FromComponentInHierarchy().AsSingle();
        Container.Bind<MainUI>().FromComponentInHierarchy().AsSingle();
    }
}