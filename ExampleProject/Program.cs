using System;
using StateFramework;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var stateManager = new StateManager();
            var stateProvider = new ExampleStateProvider();

            stateManager.AddStore(stateProvider);

            System.Console.WriteLine(stateManager.GetState<string>("example.foo"));
            System.Console.WriteLine(stateManager.GetState<int>("test.bar"));
            System.Console.WriteLine(stateManager.GetState<bool>("test.bar.mux"));

            try {
                stateManager.SetState("example.foo", "trubbel");
            } catch (Exception e) {
                System.Console.WriteLine($"{e.ToString()}: {e.Message}");
            }

            stateManager.SetState("test.bar", 1337);

            System.Console.WriteLine(stateManager.GetState<string>("example.foo"));
            System.Console.WriteLine(stateManager.GetState<int>("test.bar"));
        }
    }
}