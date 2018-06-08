using System;
using System.Collections;
using UnityEngine;
using Scripts.Core.Config;

namespace Scripts.Core
{

    public abstract class ComponentLinkedController
    {

        protected virtual ComponentHolder ComponentHolder { get { return _; } }

        //Just a placeholder, to allow for a polymorhic property, and for component holder await
#pragma warning disable 0649
        private ComponentHolder _;
#pragma warning restore 0649


        public IEnumerator AwaitComponentHolder(Action callback)
        {
            yield return new WaitUntil(() => ComponentHolder != null);
            callback();
        }

        public virtual void AssignComponentHolder(ComponentHolder componentHolder)
        {

        }

        public virtual void Initialize(GameConfig gameConfig)
        {

        }

        public virtual void Release()
        {

        }
    }
}