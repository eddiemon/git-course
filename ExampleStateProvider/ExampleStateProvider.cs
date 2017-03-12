using System;
using StateFramework;

namespace Example
{
    public class ExampleStateProvider
    {
        [StatePath("example.foo")]
        public string Foo { get; private set; } = "sad";

        [StatePath("test.bar")]
        public int Bar {get; set;} = 14;

        [StatePath("test.bar.mux", StatePath.PermissionModifier.Internal)]
        public bool Mux {get;} = true;
    }
}
