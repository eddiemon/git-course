using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Linq;

namespace StateFramework
{
    public class StateManager
    {
        ConcurrentDictionary<string, StatePath> _stores = new ConcurrentDictionary<string, StatePath>();

        public void AddStore(object stateStore)
        {
            var t = stateStore.GetType();
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
                    return stateHandler.prop.GetValue(stateStore);
                });

                Action<object> setter = new Action<object>(_ => throw new StatePathReadOnlyException(path));
                if (stateHandler.prop.CanWrite && stateHandler.prop.SetMethod.Attributes.HasFlag(MethodAttributes.Public))
                    setter = new Action<object>(value =>
                    {
                        stateHandler.prop.SetValue(stateStore, value);
                    });

                var statePath = StatePath.Create(path, permissionModifier, getter, setter);
                _stores.AddOrUpdate(path, statePath, (_, __) => throw new MultipleStatePathDeclarationsException(path));
            }
        }

        public T GetState<T>(string path)
        {
            if (_stores.TryGetValue(path, out var statePath))
            {
                return statePath.GetValue<T>();
            }
            throw new UndefinedStatePathException(path);
        }
        
        public void SetState<T>(string path, T value)
        {
            if (_stores.TryGetValue(path, out var statePath))
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
