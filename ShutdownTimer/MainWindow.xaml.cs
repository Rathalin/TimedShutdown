using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using System.Threading;
using System.Collections;
using System.Resources;
using System.Reflection;
using System.IO;

namespace ShutdownTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private bool closeMe;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;           
            Reset();

            // Default mode is Minutes
            Radio_Minutes.IsChecked = true;
        }

        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set
            {
                _time = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
            }
        }

        private TimeSpan _countdown;
        public TimeSpan Countdown
        {
            get { return _countdown; }
            set
            {
                _countdown = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Countdown"));
            }
        }

        private Thread Thread_Countdown;
        public event PropertyChangedEventHandler PropertyChanged;

        private int shutdownTimer = 0;

        private void Reset()
        {
            Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 2);
            Countdown = new TimeSpan(0, 0, 0);

            TBl_Time.Foreground = Brushes.Gray;
            TBl_TimeDesc.Foreground = Brushes.Gray;
            TBl_Countdown.Foreground = Brushes.Gray;
            TBl_CountdownDesc.Foreground = Brushes.Gray;
        }

        public void Command(string cmd)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + cmd;
            process.StartInfo = startInfo;
            process.Start();
        }

        private void Countdown_Worker()
        {
            bool timeToShutdown = false;
            while (!timeToShutdown)
            {
                TimeSpan timeLeft = Time - DateTime.Now;
                if (timeLeft > new TimeSpan(0, 0, 1))
                {
                    Countdown = timeLeft;
                    Thread.Sleep(1000);
                }
                else
                {
                    timeToShutdown = true;
                }
            }
            Countdown = new TimeSpan(0, 0, 0);
            Dispatcher.Invoke(delegate () { Application.Current.Shutdown(); });
        }

        private void StopShutdown()
        {
            if (Thread_Countdown != null)
            {
                Thread_Countdown.Abort();
                Command("shutdown /a");
            }
        }


        private void Button_SetTime_Click(object sender, RoutedEventArgs e)
        {
            StopShutdown();
            Button btn = (Button)sender;
            int multiplier = 1;
            if (Radio_Hours.IsChecked == true)
            {
                multiplier = 3600;
            }
            else if (Radio_Minutes.IsChecked == true)
            {
                multiplier = 60;
            }

            int tag = int.Parse((string)btn.Tag);

            // check if custom flag -1 in tag is set
            if (tag == -1)
            {
                if (int.TryParse(TB_CustomTime.Text, out tag) && tag > 0)
                {
                    TB_CustomTime.BorderBrush = Brushes.Gray;
                }
                else
                {
                    TB_CustomTime.BorderBrush = Brushes.Red;
                    return;
                }
            }

            shutdownTimer = tag * multiplier;
            Time = DateTime.Now.AddSeconds(shutdownTimer);
            Thread_Countdown = new Thread(new ThreadStart(Countdown_Worker));
            Thread_Countdown.IsBackground = true;
            Thread_Countdown.Start();

            Command("shutdown /s /t " + shutdownTimer);

            TBl_Time.Foreground = Brushes.Black;
            TBl_TimeDesc.Foreground = Brushes.Black;
            TBl_Countdown.Foreground = Brushes.Black;
            TBl_CountdownDesc.Foreground = Brushes.Black;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            StopShutdown();
            Reset();
        }

        private async void  MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            Grid_Overlay.Visibility = Visibility.Visible;

            if (e.Cancel) return;

            //for callback reentrance;
            e.Cancel = !this.closeMe;
            if (this.closeMe)
            {
                StopShutdown();
                return;
            }

            // Check if Countdown is running
            if (Countdown.CompareTo(new TimeSpan(0, 0, 0)) != 0)
            {
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Quit",
                    NegativeButtonText = "Cancel",
                    AnimateShow = true,
                    AnimateHide = false
                };

                var result = await this.ShowMessageAsync("Close Application?", "Do you really want to exit? The shutdown will be canceled.", MessageDialogStyle.AffirmativeAndNegative, mySettings);
                
                this.closeMe = result == MessageDialogResult.Affirmative;

                if (!this.closeMe)
                    Grid_Overlay.Visibility = Visibility.Collapsed;
            }

            if (this.closeMe)
                this.Close();
        }

        private void Button_Debug_Click(object sender, RoutedEventArgs e)
        {
            StopShutdown();
            Button btn = (Button)sender;
            shutdownTimer = 5;
            Command("shutdown /s /t " + shutdownTimer);

            Time = DateTime.Now.AddSeconds(shutdownTimer);

            Thread_Countdown = new Thread(new ThreadStart(Countdown_Worker));
            Thread_Countdown.IsBackground = true;
            Thread_Countdown.Start();

            TBl_Time.Foreground = Brushes.Black;
            TBl_TimeDesc.Foreground = Brushes.Black;
            TBl_Countdown.Foreground = Brushes.Black;
            TBl_CountdownDesc.Foreground = Brushes.Black;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Button_SetTime_Click(Btn1, new RoutedEventArgs());
            }
        }
    }
}
