namespace Dryer_Server.Interfaces
{
    public interface IValueReceiver<T>
    {
        void ValueReceived(T v);
    }
}
