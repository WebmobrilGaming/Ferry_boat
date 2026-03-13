using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace GF
{
    public class AsyncUtils
    {
        /// <summary>
        /// Waits asynchronously until the specified condition becomes true.
        /// </summary>
        /// <param name="condition">The condition to wait for.</param>
        /// <param name="timeoutMilliseconds">Optional timeout in milliseconds.</param>
        /// <returns>A task that completes when the condition becomes true.</returns>
        public static async Task WaitUntilAsync(Func<bool> condition, int timeoutMilliseconds = Timeout.Infinite)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition), "Condition cannot be null.");

            var startTime = Time.realtimeSinceStartup;

            while (!condition.Invoke())
            {
                if (timeoutMilliseconds != Timeout.Infinite && Time.realtimeSinceStartup - startTime > timeoutMilliseconds / 1000f)
                {
                    throw new TimeoutException("WaitUntilAsync timed out.");
                }
                await Task.Yield();
            }
        }
    }
}