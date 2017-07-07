using StateFramework;

namespace Example
{
    public class ExampleStateProvider : IStateSetter
    {
        string _foo = "asd";
        int _bar = 14;
        bool _mux = true;

        [StatePath("example.foo")]
        public string Foo { get { return _foo; } private set { _foo = value; StateChanged?.Invoke("example.foo", _foo); } }

        [StatePath("test.bar")]
        public int Bar { get { return _bar; } set { _bar = value; StateChanged?.Invoke("test.bar", _bar); } }

        [StatePath("test.bar.mux", StatePath.PermissionModifier.Internal)]
        public bool Mux { get { return _mux; } }

        public event StateChangedEventHandler StateChanged;

        public void Tickle()
        {
            Foo = "hwoo it tickles!";
        }
    }
}
