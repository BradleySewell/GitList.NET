using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GitList.Client.Startup
{
    public class Splash : IDisposable
    {
        private GitListMainWindow form;
        private SplashScreen splashScreen;

        public Splash(GitListMainWindow Form)
        {
            form = Form;
            Show();
        }

        private void Show()
        {
            form.Hide();
            splashScreen = new SplashScreen(@"Images\Splash.png");
            splashScreen.Show(false);
        }

        private void Hide()
        {
            splashScreen.Close(TimeSpan.FromSeconds(1));
            Thread.Sleep(TimeSpan.FromSeconds(1));
            form.Show();
        }

        public void Dispose()
        {
            Hide();
        }
    }
}
