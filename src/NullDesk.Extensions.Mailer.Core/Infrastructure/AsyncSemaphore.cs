// Based on code from Stephen Toub
//   https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-5-asyncsemaphore/
//   https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-6-asynclock/


using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     Class AsyncSemaphore.
    /// </summary>
    public class AsyncSemaphore
    {
        private static readonly Task SCompleted = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> _mWaiters = new Queue<TaskCompletionSource<bool>>();
        private int _mCurrentCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AsyncSemaphore" /> class.
        /// </summary>
        /// <param name="initialCount">The initial count.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">initialCount</exception>
        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCount));
            }

            _mCurrentCount = initialCount;
        }

        /// <summary>
        ///     Async wait.
        /// </summary>
        /// <returns>Task.</returns>
        public Task WaitAsync()
        {
            lock (_mWaiters)
            {
                if (_mCurrentCount > 0)
                {
                    --_mCurrentCount;
                    return SCompleted;
                }

                var waiter = new TaskCompletionSource<bool>();
                _mWaiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        /// <summary>
        ///     Releases this instance.
        /// </summary>
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (_mWaiters)
            {
                if (_mWaiters.Count > 0)
                {
                    toRelease = _mWaiters.Dequeue();
                }
                else
                {
                    ++_mCurrentCount;
                }
            }

            toRelease?.SetResult(true);
        }
    }
}