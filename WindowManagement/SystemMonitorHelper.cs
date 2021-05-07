using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

public class SystemMonitorHelper
{

    public static List<DisplayInfo> _monitors = new List<DisplayInfo>();

    public static List<DisplayInfo> Monitors
    {
        get
        {
            if(_monitors == null || _monitors.Count == 0)
            {
                Init();
            }

            return _monitors;
        }
    }

    public static void Init()
    {
        _monitors = QueryDisplays();
    }

    public static DisplayInfo GetMonitorByScreenPos(Vector3 screenPos)
    {

        screenPos.y = Screen.height - screenPos.y;  //  Unity To Windows Coord Space

        return Monitors.FirstOrDefault(e => e.MonitorArea.Contains(screenPos));
    }

    public static Vector3[] GetMonitorConors(Vector3 screenPos, Camera mappingCam, out bool hasMonitor)
    {
        hasMonitor = false;
        DisplayInfo di = GetMonitorByScreenPos(screenPos);

        if (di != null )
        {

            Vector3[] corners = new Vector3[] {
            new Vector3(di.MonitorLeft, Screen.height - di.MonitorTop),
            new Vector3(di.MonitorLeft+di.ScreenWidth, Screen.height - di.MonitorTop),
            new Vector3(di.MonitorLeft+di.ScreenWidth, Screen.height - di.MonitorTop - di.ScreenHeight),
            new Vector3(di.MonitorLeft, Screen.height - di.MonitorTop - di.ScreenHeight)};

            for (int i = 0; i != corners.Length; i++)
            {
                corners[i] = mappingCam.ScreenToWorldPoint(corners[i]);
            }
            hasMonitor = true;
            return corners;
        }

        return null;
    }

    public static float MaxMonitorHeight()
    {
        return Monitors.Max(e => e.ScreenHeight);
    }

    public static float TotalMonitorWidth()
    {
        return Monitors.Sum(e => e.ScreenWidth);
    }

    public static float TotalMonitorHeight()
    {
        return Monitors.Sum(e => e.ScreenHeight);
    }


    #region Multi-Display Detection
    private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MonitorInfo
    {
        public uint Size;
        public RectNative Monitor;
        public RectNative WorkArea;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RectNative
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hmon, ref MonitorInfo monitorinfo);
    #endregion

    /// <summary>
    /// The struct that contains the display information
    /// </summary>
    public class DisplayInfo
    {
        public string Availability { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }
        public Rect MonitorArea { get; set; }
        public int MonitorTop { get; set; }
        public int MonitorLeft { get; set; }
        public string DeviceName { get; set; }
        public string FriendlyName { get; set; }
        public string VendorsName { get; set; }
    }

    /// <summary>
    /// Returns the number of Displays using the Win32 functions.
    /// </summary>
    /// <returns>A collection of DisplayInfo with information about each monitor.</returns>
    public static List<DisplayInfo> QueryDisplays()
    {
        var Monitors = new List<DisplayInfo>();

        // Get the all Display Monitors.
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
            {
                MonitorInfo monitor = new MonitorInfo();
                monitor.Size = (uint)Marshal.SizeOf(monitor);
                monitor.DeviceName = null;
                bool Success = GetMonitorInfo(hMonitor, ref monitor);
                if (Success)
                {
                    DisplayInfo displayinfo = new DisplayInfo();
                    displayinfo.ScreenWidth = monitor.Monitor.Right - monitor.Monitor.Left;
                    displayinfo.ScreenHeight = monitor.Monitor.Bottom - monitor.Monitor.Top;
                    displayinfo.MonitorArea = new Rect(monitor.Monitor.Left, monitor.Monitor.Top, displayinfo.ScreenWidth, displayinfo.ScreenHeight);
                    displayinfo.MonitorTop = monitor.Monitor.Top;
                    displayinfo.MonitorLeft = monitor.Monitor.Left;
                    displayinfo.Availability = monitor.Flags.ToString();
                    displayinfo.DeviceName = monitor.DeviceName;
                    displayinfo.FriendlyName = QueryDisplaysFriendlyName(monitor.DeviceName);
                    displayinfo.VendorsName = QueryDisplaysVendorName(monitor.DeviceName);
                    Monitors.Add(displayinfo);
                }
                return true;
            }, IntPtr.Zero);
        return Monitors;
    }
    /// <summary>
    /// Returns the Friendly Name of a target Display using the Win32 functions.
    /// </summary>
    /// <returns>A string of with FriendlyName from DeviceName.</returns>
    private static string QueryDisplaysFriendlyName(string DeviceName)
    {
        string FriendlyName = null;

        // Get Friendly Name for the Device code goes here.


        return FriendlyName;
    }
    /// <summary>
    /// Returns the Vendors Name of a target Display using the Win32 functions.
    /// </summary>
    /// <returns>A string of with VendorName from DeviceName.</returns>
    private static string QueryDisplaysVendorName(string DeviceName)
    {
        string VendorName = null;

        // Get Vendors Name for the Device code goes here.


        return VendorName;
    }

}
