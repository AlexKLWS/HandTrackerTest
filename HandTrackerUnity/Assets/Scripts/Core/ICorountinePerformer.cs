using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core
{
    public interface ICorountinePerformer
    {
        /// <summary>
        /// Adds coroutine to the queue of the executing coroutines. If queue is empty, performs coroutine next frame (IMPORTANT!).
        /// </summary>
        /// <param name="routine">Coroutine.</param>
        void EnqueueCoroutine(IEnumerator routine);
        /// <summary>
        /// Allow to wait until multiple coroutines are performed in parallel. Awaits until ALL coroutines are performed.
        /// </summary>
        /// <returns>Exits after all coroutines performed</returns>
        /// <param name="enumerators">Accepts a batch of FUNCTIONS, that accept callback and return IEnumerator</param>
        IEnumerator AwaitParallelRoutines(IEnumerable<Func<Action, IEnumerator>> enumerators);
        /// <summary>
        /// Starts a coroutine.
        /// </summary>
        /// <returns>The coroutine.</returns>
        /// <param name="routine">Coroutine.</param>
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine coroutine);
    }
}