using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Core.Services;
using Scripts.Core.Config;
using UnityEngine;

namespace Scripts.UI
{
    public class ButtonController : ComponentLinkedController
    {
        protected override ComponentHolder ComponentHolder { get { return _componentHolder; } }

        private ButtonComponentHolder _componentHolder;


        public override void AssignComponentHolder(ComponentHolder componentHolder)
        {
            _componentHolder = componentHolder as ButtonComponentHolder;
        }

        public override void Initialize(GameConfig gameConfig)
        {
            _componentHolder.StartSetupButton.onClick.AddListener(StartSetup);
            _componentHolder.StartTrackingButton.onClick.AddListener(StartTracking);
            _componentHolder.StartTrackingButton.gameObject.SetActive(false);
        }

        private void StartSetup()
        {
            _componentHolder.StartSetupButton.gameObject.SetActive(false);
            _componentHolder.StartTrackingButton.gameObject.SetActive(true);
            GameController.ServiceControllerInstance.GetService<MainService>().StartSetup();
        }

        private void StartTracking()
        {
            _componentHolder.StartTrackingButton.gameObject.SetActive(false);
            GameController.ServiceControllerInstance.GetService<MainService>().StartColorSamplingAndTracking();
        }

    }
}