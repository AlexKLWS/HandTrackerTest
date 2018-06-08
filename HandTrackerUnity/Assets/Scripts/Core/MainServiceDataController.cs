using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Core.Config;
using Scripts.Core.Services;
using Scripts.Interop;
using UnityEngine;

namespace Scripts.Core
{
	public class MainServiceDataController : IServiceController, IDataController
	{
		public bool ServicesInitialized { get; private set; }

		public GameConfig GetConfig { get { return _currentGameConfig; } }

		private GameConfig _currentGameConfig;

		private Dictionary<Type, object> _services;


		public void Initialization()
		{
			_currentGameConfig = GameConfig.GetConfig;

			GameController.CorountinePerformerInstance.StartCoroutine(InitializeSelf());
		}

		private IEnumerator InitializeSelf()
		{
            int camWidth = 0, camHeight = 0;
            int result = HandTrackerInterop.Init(ref camWidth, ref camHeight);
            if (result < 0)
            {
                if (result == 1)
                {
                    Debug.LogWarningFormat("[{0}] Failed to open camera stream.", GetType());
                }

                yield break;
            }

            _currentGameConfig.CameraWidth = camWidth;
            _currentGameConfig.CameraHeight = camHeight;

			_services = new Dictionary<Type, object>
			{
                {typeof(MainService), new MainService(this)},
			};

            MainService mainService = _services[typeof(MainService)] as MainService;
			
			ServicesInitialized = true;

            yield return GameController.CorountinePerformerInstance.StartCoroutine(mainService.IntiializeMainService());
		}


		public TService GetService<TService>() where TService : class
		{
			if (_services != null && _services.ContainsKey(typeof(TService)))
			{
				return _services[typeof(TService)] as TService;
			}
			else
			{
				foreach (object obj in _services.Values)
				{
					TService castObject = obj as TService;
					if (castObject != null)
					{
						return castObject;
					}
				}
			}
			Debug.LogErrorFormat("No {0} is available!", typeof(TService));
			return null;
		}
	}
}