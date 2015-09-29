using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GitList.Core.Entities.Dispatcher
{
    public static class DispatchInvoker
    {
        public static void InvokeBackground(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }
    }
}
