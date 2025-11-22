using System;
using System.Windows;

namespace BetterTriggers.Utility
{
    public static class UiDispatch
    {
        public static void Invoke(Action action)
        {
            var app = Application.Current;
            var dispatcher = app?.Dispatcher;
            if (dispatcher != null)
                dispatcher.Invoke(action);
            else
                action();
        }
    }
}
