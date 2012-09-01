using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace SimpleDotImage
{
    public class BackgroundDispatcher
    {
        private Dispatcher _dispatcher;

        public BackgroundDispatcher(string name)
        {
            AutoResetEvent are = new AutoResetEvent(false);

            Thread thread = new Thread((ThreadStart)delegate
            {
                _dispatcher = Dispatcher.CurrentDispatcher;
                _dispatcher.UnhandledException +=
                delegate(
                     object sender,
                     DispatcherUnhandledExceptionEventArgs e)
                {
                    e.Handled = true;
                };
                are.Set();
                Dispatcher.Run();
            });

            thread.Name = string.Format("BackgroundStaDispatcher({0})", name);
            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Start();

            are.WaitOne();
            are.Close();
            are.Dispose();
        }

        public void Invoke(Action action)
        {
            _dispatcher.Invoke(action);
        }

        public void BeginInvoke(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }
    }
}
