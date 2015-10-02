using PluginCore.Fundamentals;
using System;

namespace PluginCore.Classes
{
    public class Disposable : IDisposable
    {
        protected Action _OnDispose;
        public Disposable() { }
        public Disposable(Action onDispose)
        {
            _OnDispose = onDispose;
        }
        private bool _IsDisposed;
        /// <summary>
        /// Returns true if the object has been disposed
        /// </summary>
        public bool IsDisposed
        {
            get { return _IsDisposed; }
        }
        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (this.IsDisposed)
                return;

            _IsDisposed = true;

            if (_OnDispose != null)
                _OnDispose.Invoke();

            this.OnDispose();
        }
        /// <summary>
        /// Can be overriden by the descendant class to perform actions on object's disposal.
        /// </summary>
        protected virtual void OnDispose()
        {
        }
        ~Disposable()
        {
            Dispose(false);
        }
        /// <summary>
        ///  disposes the given instance if it is an IDisposable and not null
        /// </summary>
        /// <param name="instance">instance to dispose</param>
        public static void Dispose(object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }

    public class DisposeOnce : Disposable
    {
        /// <summary>
        ///  creates a disposable which takes ownership of the given inner disposable
        /// </summary>
        /// <typeparam name="T">disposable type</typeparam>
        /// <param name="innerFactory">inner disposable factory</param>
        /// <returns>a disposable which takes ownership of the given inner disposable</returns>
        public static DisposeOnce<T> Create<T>(Func<T> innerFactory) where T : class, IDisposable
        {
            return new DisposeOnce<T>(innerFactory());
        }
        protected DisposeOnce(IDisposable inner)
        {
            this.Inner = inner;
        }
        protected IDisposable Inner { get; private set; }
        bool _DisposableOwnershipIsTakenAway;
        protected override void OnDispose()
        {
            base.OnDispose();
            if (!_DisposableOwnershipIsTakenAway && this.Inner != null)
            {
                this.Inner.Dispose();
            }
        }
        /// <summary>
        ///  signals that the inner disposable ownership has been lost (passed to another disposable)
        /// </summary>
        public void DisposableOwnershipIsTakenAway()
        {
            _DisposableOwnershipIsTakenAway = true;
        }
    }

    /// <summary>
    ///  used in scenarios where ownership of an disposable is passed to another disposable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DisposeOnce<T> : DisposeOnce
        where T : class, IDisposable
    {
        internal DisposeOnce(T inner) : base(inner) { }
        /// <summary>
        ///  disposable instance passed in the constructor
        /// </summary>
        new public T Inner
        {
            get
            {
                var res = base.Inner;
                Check.NotNull(res, "Inner");
                return (T)res;
            }
        }
    }
}
