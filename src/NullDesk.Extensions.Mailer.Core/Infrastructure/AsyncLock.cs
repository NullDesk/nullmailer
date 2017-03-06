// Based on code from Stephen Toub
//   https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-5-asyncsemaphore/
//   https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-6-asynclock/

using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace NullDesk.Extensions.Mailer.Core
{
    /// <summary>
    ///     AsyncLock.
    /// </summary>
    public class AsyncLock
    {
        private readonly Task<Releaser> _mReleaser;
        private readonly AsyncSemaphore _mSemaphore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AsyncLock" /> class.
        /// </summary>
        public AsyncLock()
        {
            _mSemaphore = new AsyncSemaphore(1);
            _mReleaser = Task.FromResult(new Releaser(this));
        }

        /// <summary>
        ///     Create an async lock.
        /// </summary>
        /// <returns>Task&lt;Releaser&gt;.</returns>
        public Task<Releaser> LockAsync()
        {
            var wait = _mSemaphore.WaitAsync();
            return wait.IsCompleted
                ? _mReleaser
                : wait.ContinueWith((_, state) => new Releaser((AsyncLock) state),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        /// <summary>
        ///     Struct Releaser
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _mToRelease;

            internal Releaser(AsyncLock toRelease)
            {
                _mToRelease = toRelease;
            }

            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _mToRelease?._mSemaphore.Release();
            }
        }
    }
}