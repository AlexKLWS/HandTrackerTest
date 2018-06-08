using UnityEngine;
using Scripts.Interop;

namespace Scripts.Core
{
    public partial class GameController : MonoBehaviour
    {

        public delegate void UpdateEvent();
        public static event UpdateEvent OnUpdate;

        public delegate void QuitEvent();
        public static event QuitEvent OnQuit;

        public static IServiceController ServiceControllerInstance
        {
            get { return _serviceControllerInstance; }
        }

        public static ICorountinePerformer CorountinePerformerInstance
        {
            get { return _coroutinePerformerInstance; }
        }

        private static IServiceController _serviceControllerInstance;
        private static ICorountinePerformer _coroutinePerformerInstance;

        private void Awake()
        {
            Application.targetFrameRate = 30;
            _serviceControllerInstance = new MainServiceDataController();

            _coroutinePerformerInstance = this;
            _serviceControllerInstance.Initialization();
        }

        //Originally, I intended to make an update analog method, using a coroutine, but
        //since we can't use unsafe code inside enumerators, I had to try current approach
        //I'm not a big fan of events, but decided to show that I am familiar with the concept
        private void Update()
        {
            if (!_serviceControllerInstance.ServicesInitialized)
            {
                return;
            }

            if (OnUpdate != null)
            {
                OnUpdate();
            }
        }

        private void OnApplicationQuit()
        {
            if (OnQuit != null)
            {
                OnQuit();
            }
        }
    }
}