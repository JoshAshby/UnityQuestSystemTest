using System;

namespace Ashode
{
    namespace CoreExtensions
    {
        public static class EventExtensions
        {
            public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> handler, object sender, TEventArgs args) where TEventArgs : EventArgs
            {
                var e = handler;
                if (e != null)
                    e(sender, args);
            }

            public static void SafeInvoke(this Action handler)
            {
                var e = handler;
                if (e != null)
                    e();
            }
        }
    }
}