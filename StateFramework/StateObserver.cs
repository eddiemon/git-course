using System;

namespace StateFramework
{
    public class StateObserver<T> : IStateObserver
    {
        private readonly StateManager _stateManager;
        private readonly Action<T> _onStateValueChangedHandler;

        public StateObserver(StateManager stateManager, string statePath, Action<T> onChanged)
        {
#if DEBUG
            object value = stateManager.GetState<object>(statePath);
            if (value.GetType() != typeof(T))
            {
                System.Environment.FailFast($"Could not convert value from type {value.GetType()} to {typeof(T)}");
            }
#endif

            _stateManager = stateManager;
            _onStateValueChangedHandler = onChanged;

            _stateManager.AddObserver(this, statePath);
        }

        public void OnStateChanged(string statePath, object newValue)
        {
            if (newValue is T val)
            {
                _onStateValueChangedHandler(val);
            } else
            {
                throw new InvalidStateValueConversionException(newValue.GetType(), typeof(T));
            }
        }
    }
}
