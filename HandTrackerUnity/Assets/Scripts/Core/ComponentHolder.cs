using System.Collections;
using UnityEngine;

namespace Scripts.Core
{
    public class ComponentHolder : MonoBehaviour
    {
        public System.Type RelatedVisualController { get; protected set; }

        protected virtual void RegisterSelf() { }

        protected virtual IEnumerator AwaitServices()
        {
            yield return new WaitUntil(() => GameController.ServiceControllerInstance.ServicesInitialized);
            RegisterSelf();
        }
    }
}