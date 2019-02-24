using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intel.Overclocking.SDK.Monitoring;
using Intel.Overclocking.SDK.Tuning;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows.Threading;
using MySettingDLL;

namespace Rev.Center2_Tuner
{
    class PerformanceSetter: ApplicationContext
    {
        string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        const string userRoot = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Rev.Center";
        const string keyName = userRoot + "\\" + "Rev.Center2.0";

        private int performance_mode;
        private int throttlestop=0;
        private MySetting MySetting = new MySetting();

        string temp = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public PerformanceSetter()
        {
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            InitializeComponent();
            }

        private void InitializeComponent()
        {
            SetPerf();
        }
        private void SetPerf()
        {
            performance_mode = Convert.ToInt32(Registry.GetValue(keyName, "PerformanceMode", 0));
            switch (performance_mode)
            {
                case 1:
                    object data = (object)0;
                    WMIEC.WMIReadECRAM(1115UL, ref data);
                    if (((long)Convert.ToUInt64(data) & 1L) != 1L)
                        MySetting.SilentModeOnOff();
                    System.IO.File.Copy(path + @"\Throttlestop\Basic.ini", path + @"\Throttlestop\ThrottleStop.ini",true);
                    break;
                case 2:
                    data = (object)0;
                    WMIEC.WMIReadECRAM(1115UL, ref data);
                    if (((long)Convert.ToUInt64(data) & 1L) == 1L)
                        MySetting.SilentModeOnOff();
                    System.IO.File.Copy(path + @"\Throttlestop\Basic.ini", path + @"\Throttlestop\ThrottleStop.ini", true);

                    break;
                case 3:
                    data = (object)0;
                    WMIEC.WMIReadECRAM(1115UL, ref data);
                    if (((long)Convert.ToUInt64(data) & 1L) == 1L)
                        MySetting.SilentModeOnOff();
                    System.IO.File.Copy(path + @"\Throttlestop\Performance.ini", path + @"\Throttlestop\ThrottleStop.ini", true);
                    break;
            }
            if ((throttlestop = Convert.ToInt32(Registry.GetValue(keyName, "ThrottleStop", 1))) == 1)
            {
                foreach (var process in Process.GetProcessesByName("ThrottleStop"))
                {
                    process.Kill();
                }
                Process.Start(path + "/Throttlestop/ThrottleStop.exe");
            }
            Environment.Exit(1);

        }


        private void OnApplicationExit(object sender, EventArgs e)
        {
            //Cleanup so that the icon will be removed when the application is closed
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            //Here you can do stuff if the tray icon is doubleclicked
        }

        
    }
}
