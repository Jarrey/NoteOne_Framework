﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace NoteOne_Utility.Helpers
{
    public class AsyncLock
    {
        private readonly Task<Releaser> m_releaser;
        private readonly AsyncSemaphore m_semaphore;

        public AsyncLock()
        {
            m_semaphore = new AsyncSemaphore(1);
            m_releaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            Task wait = m_semaphore.WaitAsync();
            return wait.IsCompleted
                       ? m_releaser
                       : wait.ContinueWith((_, state) => new Releaser((AsyncLock) state),
                                           this, CancellationToken.None,
                                           TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;

            internal Releaser(AsyncLock toRelease)
            {
                m_toRelease = toRelease;
            }

            public void Dispose()
            {
                if (m_toRelease != null)
                    m_toRelease.m_semaphore.Release();
            }
        }
    }
}