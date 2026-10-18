namespace MWSMan.Core.Model.Events
{
    public class CenterEvent 
    {
        public delegate void ServerEventHandler();
        public event ServerEventHandler MyCustomEvent;
        public CenterEvent()
        {
            
        }
        public void ServerEventCaller()
        {
            this.MyCustomEvent.Invoke();
        }
    }
}
