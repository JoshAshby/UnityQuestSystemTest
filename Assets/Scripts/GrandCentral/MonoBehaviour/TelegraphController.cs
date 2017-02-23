using GrandCentral.Telegraph;

namespace GrandCentral
{
    [Prefab("Telegraph Controller", true)]
    public class TelegraphController : Singleton<TelegraphController>
    {
        public IMessenger Bus { get; private set; }

        public void Awake()
        {
            Bus = new Messenger();
        }
    }
}