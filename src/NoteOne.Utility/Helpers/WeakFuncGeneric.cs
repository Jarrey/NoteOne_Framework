using System;

namespace NoteOne_Utility.Helpers
{
    /// <summary>
    ///     Stores a Func without causing a hard reference
    ///     to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="T">The type of the Func's parameter.</typeparam>
    ////[ClassInfo(typeof(Messenger))]
    public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult<TResult>
    {
        private readonly Func<T, TResult> _func;

        /// <summary>
        ///     Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The func's owner.</param>
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakFunc(object target, Func<T, TResult> func)
            : base(target, null)
        {
            _func = func;
        }

        public WeakFunc(Func<T, TResult> func)
            : this(func.Target, func)
        {
        }

        public new bool IsStatic
        {
            get { return _func != null && _func.Target == null; }
        }

        /// <summary>
        ///     Gets the Func associated to this instance.
        /// </summary>
        public new Func<T, TResult> Func
        {
            get { return _func; }
        }

        /// <summary>
        ///     Executes the func with a parameter of type object. This parameter
        ///     will be casted to T. This method implements <see cref="IExecuteWithObject.ExecuteWithObject" />
        ///     and can be useful if you store multiple WeakFunc{T} instances but don't know in advance
        ///     what type T represents.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter that will be passed to the action after
        ///     being casted to T.
        /// </param>
        public TResult ExecuteWithObject(object p1, object p2 = null)
        {
            var p1Casted = (T) p1;
            return Execute(p1Casted);
        }

        /// <summary>
        ///     Executes the func. This only happens if the func's owner
        ///     is still alive. The func's parameter is set to default(T).
        /// </summary>
        public new TResult Execute()
        {
            if (_func != null
                && (IsAlive || IsStatic))
            {
                return _func(default(T));
            }

            return default(TResult);
        }

        /// <summary>
        ///     Executes the func. This only happens if the func's owner
        ///     is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the func.</param>
        public TResult Execute(T parameter)
        {
            if (_func != null
                && (IsAlive || IsStatic))
            {
                return _func(parameter);
            }

            return default(TResult);
        }
    }

    public class WeakFunc<T1, T2, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult<TResult>
    {
        private readonly Func<T1, T2, TResult> _func;

        public WeakFunc(object target, Func<T1, T2, TResult> func)
            : base(target, null)
        {
            _func = func;
        }

        public WeakFunc(Func<T1, T2, TResult> func)
            : this(func.Target, func)
        {
        }

        public new bool IsStatic
        {
            get { return _func != null && _func.Target == null; }
        }

        public new Func<T1, T2, TResult> Func
        {
            get { return _func; }
        }

        public TResult ExecuteWithObject(object p1, object p2)
        {
            var p1Casted = (T1) p1;
            var p2Casted = (T2) p2;
            return Execute(p1Casted, p2Casted);
        }

        public new TResult Execute()
        {
            if (_func != null
                && (IsAlive || IsStatic))
            {
                return _func(default(T1), default(T2));
            }

            return default(TResult);
        }

        public TResult Execute(T1 t1, T2 t2)
        {
            if (_func != null
                && (IsAlive || IsStatic))
            {
                return _func(t1, t2);
            }

            return default(TResult);
        }
    }
}