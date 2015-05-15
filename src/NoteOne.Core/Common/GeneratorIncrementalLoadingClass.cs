//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NoteOne_Utility.Helpers;

namespace NoteOne_Core.Common
{
    // This class implements IncrementalLoadingBase. 
    // To create your own Infinite List, you can create a class like this one that doesn't have 'generator' or 'maxcount', 
    //  and instead downloads items from a live data source in LoadMoreItemsOverrideAsync.
    public class GeneratorIncrementalLoadingClass<T> : IncrementalLoadingBase
    {
        public GeneratorIncrementalLoadingClass(uint maxCount, Func<int, Task<T>> generator)
        {
            _generator = new WeakFunc<int, Task<T>>(generator);
            _maxCount = maxCount;
        }

        protected override async Task<IList<object>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {
            uint toGenerate = Math.Min(count, _maxCount - _count);

            // This code simply generates
            IList<object> values = new List<object>();
            if (_generator != null && (_generator.IsStatic || _generator.IsAlive))
            {
                for (var j = (int) _count; j < (int) _count + (int) toGenerate; j++)
                {
                    var obj = (object) await _generator.Execute(j);
                    if (obj != null)
                        values.Add(obj);
                    else _isCompleted = true;
                }
            }
            _count += toGenerate;

            return values.ToArray();
        }

        protected override bool HasMoreItemsOverride()
        {
            return !_isCompleted && (_count < _maxCount);
        }

        #region State

        private readonly WeakFunc<int, Task<T>> _generator;
        private readonly uint _maxCount;
        private uint _count;
        private bool _isCompleted;

        #endregion
    }
}