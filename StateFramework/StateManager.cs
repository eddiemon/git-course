using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Linq;

namespace StateFramework
{
    public class StateManager
    {
        ConcurrentDictionary<string, StatePath> _providers = new ConcurrentDictionary<string, StatePath>();
        ConcurrentDictionary<string, ConcurrentBag<IStateObserver>> _observers = new ConcurrentDictionary<string, ConcurrentBag<IStateObserver>>();

        public void AddProvider(object stateProvider)
        {
            var t = stateProvider.GetType();
            var stateHandlers =
                t.GetRuntimeProperties()
                .Where(m => m.CustomAttributes.Where(a => a.AttributeType.Equals(typeof(StatePathAttribute))).Any())
                .Select(m => new { statePath = (StatePathAttribute)m.GetCustomAttribute(typeof(StatePathAttribute)), prop = m })
                .ToList();

            foreach (var stateHandler in stateHandlers)
            {
                var path = stateHandler.statePath.Path;
                var permissionModifier = stateHandler.statePath.Permission;

                var getter = new Func<object>(() =>
                {
                    return stateHandler.prop.GetValue(stateProvider);
                });

                Action<object> setter = new Action<object>(_ => throw new StatePathReadOnlyException(path));
                if (stateHandler.prop.CanWrite && stateHandler.prop.SetMethod.Attributes.HasFlag(MethodAttributes.Public))
                    setter = new Action<object>(value =>
                    {
                        stateHandler.prop.SetValue(stateProvider, value);
                    });

                var statePath = StatePath.Create(path, permissionModifier, getter, setter);
                _providers.AddOrUpdate(path, statePath, (_, __) => throw new MultipleStatePathDeclarationsException(path));
            }

            if (stateProvider is IStateSetter stateSetter)
            {
                stateSetter.StateChanged += StateSetter_StateChanged;
            }
        }

        public void AddObserver(IStateObserver stateObserver, string observedStatePath)
        {
            var observersForStatePath = _observers.GetOrAdd(observedStatePath, new ConcurrentBag<IStateObserver>());
            observersForStatePath.Add(stateObserver);
        }

        private void StateSetter_StateChanged(string statePath, object newValue)
        {
            if (_observers.TryGetValue(statePath, out var observers))
            {
                foreach (var observer in observers)
                {
                    observer.OnStateChanged(statePath, newValue);
                }
            }
        }

        public T GetState<T>(string path)
        {
            if (_providers.TryGetValue(path, out var statePath))
            {
                return statePath.GetValue<T>();
            }
            throw new UndefinedStatePathException(path);
        }
        
        /// <summary>
        /// Sets a state value.
        /// </summary>
        /// <param name="path">The path to set.</param>
        /// <param name="value">The value to set.</param>
        /// <exception>StatePathReadOnlyException</exception>
        /// <exception>UndefinedStatePathException</exception>
        public void SetState<T>(string path, T value)
        {
            if (_providers.TryGetValue(path, out var statePath))
            {
                statePath.SetValue<T>(value);
                return;
            }
            throw new UndefinedStatePathException(path);
        }
    }

    public class StatePathReadOnlyException : Exception
    {
        public StatePathReadOnlyException(string path) : base(path) { }
    }

    #region Exceptions
    public class MultipleStatePathDeclarationsException : Exception
    {
        public MultipleStatePathDeclarationsException(string path) : base(path) { }
    }

    public class UndefinedStatePathException : Exception
    {
        public UndefinedStatePathException(string path) : base(path) { }
    }
    #endregion
}
