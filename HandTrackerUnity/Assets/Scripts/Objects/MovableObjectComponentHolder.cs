using Scripts.Core.Services;
using Scripts.Core;
using UnityEngine;

namespace Scripts.Objects
{

    public class MovableObjectComponentHolder : ComponentHolder
    {

#pragma warning disable 0649
        [SerializeField] private Camera _currentCamera;
        [SerializeField] private Transform _movableObjectTransform;
#pragma warning restore 0649

        public Camera CurrentCamera { get { return _currentCamera; } }
        public Transform MovableObjectTransform { get { return _movableObjectTransform; } }

        private void Start()
        {
            RelatedVisualController = typeof(MovableObjectController);
            //Even though this is a MonoBehaviour, for the sake of consistency
            //I start a coroutine on GameController
            GameController.CorountinePerformerInstance.StartCoroutine(AwaitServices());
        }

        protected override void RegisterSelf()
        {
            GameController.ServiceControllerInstance.GetService<MainService>().RegisterComponentHolder(this);
        }
    }
}