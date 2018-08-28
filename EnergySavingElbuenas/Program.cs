using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EnergySavingElbuenas
{
    class Program
    {
        static void Main(string[] args)
        {
            var checkPoint = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 01, 20, 0);
            var longTimeBefore = new TimeSpan(0, 15, 0);
            var timeBeforeSleep = new TimeSpan(0, 2, 0);

            Console.WriteLine("Ctrl + C    ...para cancelar");

            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday
                || DateTime.Now.DayOfWeek == DayOfWeek.Saturday
                || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                return;
            }

            var sleep = 30000;
            var debeEntrarMsgConTiempo = true;

            while (true)
            {
                Console.WriteLine("ahora: " + DateTime.Now);
                Console.WriteLine("hiberna a: " + checkPoint);

                if (DateTime.Now.AddMinutes(longTimeBefore.Minutes) >= checkPoint && debeEntrarMsgConTiempo)
                {
                    AutoClosingMessageBox.Show("Cierra la applicacion para cancelar la hibernacion", 
                        "EnergySaving El Buenas", 50000);

                    debeEntrarMsgConTiempo = false;
                }

                if (DateTime.Now.AddMinutes(timeBeforeSleep.Minutes) >= checkPoint)
                {
                    AutoClosingMessageBox.Show("Cierra la applicacion para cancelar la hibernacion",
                       "EnergySaving El Buenas", 9000);

                    sleep = 10000;
                }

                if (DateTime.Now > checkPoint)
                {
                    Console.Clear();

                    Console.WriteLine("hibernando ahora...");

                    var stringCommand = "shutdown /h /f";
                    Console.WriteLine(stringCommand);

                    ProcessStartInfo procStartInfo = new ProcessStartInfo();
                    procStartInfo.CreateNoWindow = false;
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    procStartInfo.FileName = "shutdown.exe";
                    procStartInfo.Arguments = "/h /f";
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.RedirectStandardError = true;

                    Process p = Process.Start(procStartInfo);
                    string outstring = p.StandardOutput.ReadToEnd();
                    string errstring = p.StandardError.ReadToEnd();
                    p.WaitForExit();

                    break;
                }

                Thread.Sleep(sleep);
            }

            Console.ReadLine();
        }
    }


    public class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        string _caption;
        AutoClosingMessageBox(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            using (_timeoutTimer)
                MessageBox.Show(text, caption);
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}
