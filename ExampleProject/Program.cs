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

            stateManager.AddProvider(stateProvider);

            System.Console.WriteLine(stateManager.GetState<string>("example.foo"));
            System.Console.WriteLine(stateManager.GetState<int>("test.bar"));
            System.Console.WriteLine(stateManager.GetState<bool>("test.bar.mux"));

            try {
                stateManager.SetState("example.foo", "trubbel");
            } catch (Exception e) {
                System.Console.WriteLine($"{e.ToString()}: {e.Message}");
            }

            // Should throw as the value is an int
            var _ = new StateObserver<int>(stateManager, "test.bar", s => Console.WriteLine("test.bar changed value to: " + s));

            stateManager.SetState("test.bar", 1337);

            System.Console.WriteLine(stateManager.GetState<string>("example.foo"));
            System.Console.WriteLine(stateManager.GetState<int>("test.bar"));

            var stateObserver = new StateObserver<string>(stateManager, "example.foo", s => Console.WriteLine(s));

            stateProvider.Tickle();

            Console.ReadLine();
        }
    }
}