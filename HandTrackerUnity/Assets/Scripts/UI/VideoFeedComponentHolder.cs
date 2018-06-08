using UnityEngine;
using Scripts.Core;
using Scripts.Core.Services;

namespace Scripts.UI
{

    public class VideoFeedComponentHolder : ComponentHolder
    {

#pragma warning disable 0649
        [SerializeField] private Material _videoMaterial;
        [SerializeField] private Transform _videoTransform;
#pragma warning restore 0649

        public Material VideoMaterial { get { return _videoMaterial; } }
        public Transform VideoTransform { get { return _videoTransform; } }

        private void Start()
        {
            RelatedVisualController = typeof(VideoFeedController);
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