using System;

namespace StateFramework
{
    public class StatePath : IEquatable<StatePath>
    {
        public enum PermissionModifier
        {
            Public,
            Internal,
            Custom,
        }

        public string Path { get; protected set; }
        public PermissionModifier Permission { get; protected set; }
        private Func<object> _valueGetter;
        private Action<object> _valueSetter;

        public T GetValue<T>()
        {
            if (_valueGetter == null) throw new NotImplementedException(Path);
            var value = _valueGetter.Invoke();
            if (value is T t) return t;
            throw new InvalidCastException($"Target type is {typeof(T)}, source type is {value.GetType().ToString()}");
        }

        public void SetValue<T>(T value) {
            _valueSetter.Invoke(value);
        }

        internal static StatePath Create(string path, PermissionModifier permission, Func<object> valueGetter, Action<object> valueSetter)
        {
            return new StatePath
            {
                Path = path,
                Permission = permission,
                _valueGetter = valueGetter,
                _valueSetter = valueSetter,
            };
        }

        bool IEquatable<StatePath>.Equals(StatePath other)
        {
            return this.Path.Equals(other.Path);
        }
    }
}