namespace StateFramework
{
    public delegate void StateChangedEventHandler(string statePath, object newValue);

    public interface IStateSetter
    {
        event StateChangedEventHandler StateChanged;
    }
}
