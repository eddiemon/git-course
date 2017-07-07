using System;

namespace StateFramework
{
    public interface IStateObserver
    {
        void OnStateChanged(string statePath, object newValue);
    }

    public class StateObserver<T> : IStateObserver
    {
        private readonly StateManager _stateManager;
        private readonly Action<T> _onStateValueChangedHandler;

        public StateObserver(StateManager stateManager, string statePath, Action<T> onChanged)
        {
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

    public class InvalidStateValueConversionException : Exception
    {
        public InvalidStateValueConversionException(Type fromType, Type toType): base($"Could not convert value from {fromType} to {toType}")
        {
            
        }
    }
}
