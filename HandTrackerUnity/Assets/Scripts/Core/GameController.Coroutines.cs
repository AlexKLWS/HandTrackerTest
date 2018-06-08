using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core
{
    public partial class GameController : ICorountinePerformer
    {
        private Queue<IEnumerator> _coroutineQueue;

        public void EnqueueCoroutine(IEnumerator routine)
        {
            if (_coroutineQueue == null)
            {
                _coroutineQueue = new Queue<IEnumerator>();
            }
            _coroutineQueue.Enqueue(routine);
            //If this is first coroutine added, start executer
            if (_coroutineQueue.Count == 1)
            {
                StartCoroutine(OrderedCoroutineExecuter());
            }
        }

        private IEnumerator OrderedCoroutineExecuter()
        {
            yield return null;
            while (_coroutineQueue.Count != 0)
            {
                yield return StartCoroutine(_coroutineQueue.Dequeue());
            }
        }

        public IEnumerator AwaitParallelRoutines(IEnumerable<Func<Action, IEnumerator>> enumerators)
        {
            if (enumerators == null)
            {
                yield break;
            }
            int parallelRoutineCounter = enumerators.Count();
            foreach (Func<Action, IEnumerator> enumerator in enumerators)
            {
                StartCoroutine(enumerator(() => parallelRoutineCounter--));
            }
            yield return new WaitUntil(() => parallelRoutineCounter == 0);
        }
    }
}