using Scripts.Core.Services;
using Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{

    public class ButtonComponentHolder : ComponentHolder
    {

#pragma warning disable 0649
        [SerializeField] private Button _startSetupButton;
        [SerializeField] private Button _startTrackingButton;
#pragma warning restore 0649

        public Button StartSetupButton { get { return _startSetupButton; } }
        public Button StartTrackingButton { get { return _startTrackingButton; } }

        private void Start()
        {
            RelatedVisualController = typeof(ButtonController);
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