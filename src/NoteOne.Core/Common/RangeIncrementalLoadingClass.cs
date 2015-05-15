using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NoteOne_Utility.Helpers;

namespace NoteOne_Core.Common
{
    // This class implements IncrementalLoadingBase. 
    // To create your own Infinite List, you can create a class like this one that doesn't have 'generator' or 'maxcount', 
    //  and instead downloads items from a live data source in LoadMoreItemsOverrideAsync.
    public class RangeIncrementalLoadingClass<T> : IncrementalLoadingBase
    {
        public RangeIncrementalLoadingClass(uint maxCount, Func<uint, uint, Task<IList<T>>> generator)
        {
            _generator = new WeakFunc<uint, uint, Task<IList<T>>>(generator);
            _maxCount = maxCount;
        }

        protected override async Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {
            uint toGenerate = Math.Min(count, _maxCount - _count);

            if (_generator != null && (_generator.IsStatic || _generator.IsAlive))
            {
                IList<T> values = await _generator.Execute(_count, toGenerate);
                if (values != null)
                {
                    _count += Math.Min((uint)(values.Count), toGenerate);

                    return values as IList<object>;
                }
                else
                {
                    _isCompleted = true;
                    return null;
                }
            }
            else
            {
                _isCompleted = true;
                return null;
            }
        }

        protected override bool HasMoreItemsOverride()
        {
            return !_isCompleted && (_count < _maxCount);
        }

        #region State

        private readonly WeakFunc<uint, uint, Task<IList<T>>> _generator;
        private readonly uint _maxCount;
        private uint _count;
        private bool _isCompleted;

        #endregion
    }
}