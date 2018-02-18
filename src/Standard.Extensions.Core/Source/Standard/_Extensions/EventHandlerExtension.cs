using System;

namespace Standard
{
   public static class EventHandlerExtension
    {
        public static void Raise<T>(this EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            if (handler != null)
                handler(sender, e);
        }

       public static void Raise(this EventHandler handler, object sender)
       {
           if (handler != null)
               handler(sender, EventArgs.Empty);
       }
    }
}