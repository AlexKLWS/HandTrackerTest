using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scripts.UI;
using Scripts.Interop;
using Scripts.Core.Config;
using Scripts.Objects;

namespace Scripts.Core.Services
{

    public class MainService : ComponentLinkedService
    {

        private Coroutine _colorSampleAwait;

        public MainService(IDataController dataController) : base(dataController)
        {
            _controllers = new Dictionary<Type, ComponentLinkedController>
            {
                {typeof(ButtonController), new ButtonController()},
                {typeof(VideoFeedController), new VideoFeedController()},
                {typeof(MovableObjectController), new MovableObjectController()}
            };
            GameController.OnQuit += Release;
        }

        public IEnumerator IntiializeMainService()
        {
            HashSet<Func<Action, IEnumerator>> componentHolderWaiters = new HashSet<Func<Action, IEnumerator>>();
            foreach (ComponentLinkedController controller in _controllers.Values)
            {
                componentHolderWaiters.Add(controller.AwaitComponentHolder);
            }
            yield return GameController.CorountinePerformerInstance.AwaitParallelRoutines(componentHolderWaiters);
            foreach (ComponentLinkedController controller in _controllers.Values)
            {
                controller.Initialize(_dataController.GetConfig);
            }
        }

        public void StartSetup()
        {
            HandTrackerInterop.SetupForGettingColorSample();
        }

        public void StartColorSamplingAndTracking()
        {
            HandTrackerInterop.StartColorSampling();
            if (_colorSampleAwait != null)
            {
                GameController.CorountinePerformerInstance.StopCoroutine(_colorSampleAwait);
            }
            _colorSampleAwait = GameController.CorountinePerformerInstance.StartCoroutine(ColorSampleAwaitRoutine());
        }


        private IEnumerator ColorSampleAwaitRoutine()
        {
            float timer = 0.0f;
            while (true)
            {
                if (timer > GameConfig.ColorSampleAwaitTime)
                {
                    break;
                }

                timer += Time.unscaledDeltaTime;

                yield return null;
            }
            HandTrackerInterop.StartTracking();
            MovableObjectController cubeController = _controllers[typeof(MovableObjectController)] as MovableObjectController;
            cubeController.StartTracking();
        }

        private void Release()
        {
            if (_colorSampleAwait != null)
            {
                GameController.CorountinePerformerInstance.StopCoroutine(_colorSampleAwait);
            }
            foreach (ComponentLinkedController controller in _controllers.Values)
            {
                controller.Release();
            }
            HandTrackerInterop.Close();
        }
    }
}