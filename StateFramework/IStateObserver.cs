using System;

namespace StateFramework
{
    public interface IStateObserver
    {
        void OnStateChanged(string statePath, object newValue);
    }

    public class InvalidStateValueConversionException : Exception
    {
        public InvalidStateValueConversionException(Type fromType, Type toType): base($"Could not convert value from {fromType} to {toType}")
        {
            
        }
    }
}
