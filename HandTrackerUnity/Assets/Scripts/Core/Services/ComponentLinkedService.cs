using System;
using Scripts.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.Services
{
	public class ComponentLinkedService : BaseService
	{

		protected Dictionary<Type, ComponentLinkedController> _controllers;

        protected ComponentLinkedService(IDataController dataController) : base(dataController)
		{
		}

		public void RemoveController(Type controllerType)
		{
			if (_controllers == null || !_controllers.ContainsKey(controllerType))
			{
				return;
			}
			_controllers.Remove(controllerType);
		}

		public void RegisterComponentHolder(ComponentHolder menuComponentHolder)
		{
			if (_controllers == null || !_controllers.ContainsKey(menuComponentHolder.RelatedVisualController))
			{
				Debug.LogErrorFormat("No {0} is available to register component holder!", menuComponentHolder.RelatedVisualController);
				return;
			}

			_controllers[menuComponentHolder.RelatedVisualController].AssignComponentHolder(menuComponentHolder);
		}

		public virtual void Reset()
		{
			foreach (ComponentLinkedController controller in _controllers.Values)
			{
				controller.Release();
			}
			_controllers.Clear();
		}
	}
}