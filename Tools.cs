using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.CSharp.RuntimeBinder;

namespace Utils
{
    class Tools
    {
        public static Process NewProcessWithPsi(ProcessStartInfo PSI, bool CreateNoWindow = true,
            bool UseShellExecute = false)
        {
            Process process = new Process();
            PSI.CreateNoWindow = CreateNoWindow;
            PSI.UseShellExecute = UseShellExecute;
            process.StartInfo = PSI;

            return process;
        }
    }

    class ColorChanger
    {
        private Thread thread;
        private volatile static bool Changing = false;
        public ColorChanger(Brush bg, Dispatcher dispatcher)
        {
            Color defaultColor = ((SolidColorBrush) bg).Color;
            thread = new Thread(()=>ThreadSys(bg, dispatcher, defaultColor));
        }

        private static void ThreadSys(Brush bg, Dispatcher dispatcher, Color defaultColor)
        {
            
            while (Changing)
            {
                dispatcher.Invoke(() =>
                {
                    Random random = new Random();
                    ColorAnimation animation = new ColorAnimation();
                    animation.From = ((SolidColorBrush)bg).Color;
                    animation.To = Color.FromRgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                    animation.Duration = new Duration(TimeSpan.FromSeconds(1));
                    bg.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                });
                Thread.Sleep(1000);
            }
            if (Changing == false)
            {
                dispatcher.Invoke(() =>
                {
                    Random random = new Random();
                    ColorAnimation animation = new ColorAnimation();
                    animation.From = ((SolidColorBrush)bg).Color;
                    animation.To = defaultColor;
                    animation.Duration = new Duration(TimeSpan.FromSeconds(1));
                    bg.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                });
            }
        }

        public void Start()
        {
            Changing = true;
            thread.Start();
        }

        public void Stop()
        {
            Changing = false;
        }
    }

    class WindowUtil
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool CloseWindow(IntPtr hWnd);

        private IntPtr hWnd = IntPtr.Zero;

        public enum WindowSize
        {
            Normal = 1,
            Minimized = 2,
            Maximized = 3
        }

        public WindowUtil(string Title)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process pList in processes)
            {
                if (pList.MainWindowTitle.Contains(Title))
                {
                    hWnd = pList.MainWindowHandle;
                }
            }
            if (hWnd == IntPtr.Zero)
            {
                throw new Exception("Not Found");
            }
        }

        public void setVisibility(WindowSize size)
        {
            bool success = ShowWindowAsync(hWnd, (int) size);
            if (!success)
            {
                throw new Exception("Win32API said that there was an error!");
            }
        }

        public void Close()
        {
            bool success = CloseWindow(hWnd);
            if (!success)
            {
                throw new Exception("Win32API said that there was an error!");
            }
            
        }
    
}

    static class Kiosk
    {
        private static bool isEnabledInternal = false;

        public static bool isEnabled()
        {
            return isEnabledInternal;
        }

        public static void EnableKiosk()
        {
            int killedCount = 0;
            if (!isEnabled())
            {
                Process[] explorerInstances = Process.GetProcessesByName("explorer.exe");
                foreach (Process explorer in explorerInstances)
                {
                    explorer.Kill();
                    killedCount++;
                }
                if (killedCount <= 0)
                {
                    //Using alternative metheod!
                    ProcessStartInfo cfg = new ProcessStartInfo("taskkill.exe", "/F /IM \"explorer.exe\"");
                    cfg.CreateNoWindow = true;
                    cfg.UseShellExecute = true;
                    Process alternativeKiller = new Process();
                    alternativeKiller.StartInfo = cfg;
                    alternativeKiller.Start();

                }
                isEnabledInternal = true;
            }
            
        }

        public static void DisableKiosk()
        {
            
            if (isEnabled())
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe"); ;
                
                process.StartInfo = startInfo;
                process.Start();
                isEnabledInternal = false;
            }
        }


    }

}
