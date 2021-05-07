using BestoryIntelligentControlClientLibrary;
using JashOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCInteraction : MonoBehaviour
{

    public delegate void OSC_MessageReceive(string command);
    public event OSC_MessageReceive OnOSCMessageReceived;


    public static OSCInteraction Instance;

    SettingsProfile settings;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        InitOSCService();
    }

    private void Update()
    {
        if (clientInstance != null)
            clientInstance.ProcessEvents();
    }

    private void OnApplicationQuit()
    {
        ShutDown();
    }

    #region OSC Related
    BestoryOSCClient clientInstance = null;

    void InitOSCService()
    {
        clientInstance = BestoryOSCClient.GetInstance(settings.commandCenterIP, settings.commandCenterPort, settings.commandCenterName, true, false);


        string[] projects = settings.commandCenterProjectName.Split('|');
        string[] addresses = settings.commandCenterAddressName.Split('|');

        for (int i = 0; i < projects.Length; i++)
        {
            clientInstance.AddProjectRegister(projects[i]);
        }

        for (int i = 0; i < addresses.Length; i++)
        {
            string addr = string.Format("/{0}/{1}", settings.commandCenterName, addresses[i]);
            clientInstance.AddAddressListen(addr);
        }

        clientInstance.SetDefaultParams(settings.commandCenterAddressName, settings.commandCenterProjectName);

        clientInstance.OnMessageReceived += ClientInstance_OnMessageReceived;
    }

    public void SendOSCMessageSimple(params object[] objs)
    {
        clientInstance.SendMessageSimple(objs);
    }

    public void SendOSCMessage(OSCMessage msg)
    {
        clientInstance.SendMessage(msg);
    }

    public void ShutDown()
    {
        clientInstance.Shutdown();
    }

    public bool debugMode = false;
    private void OnGUI()
    {
        if (debugMode) windowRect = GUILayout.Window(21, windowRect, DebugWindow, "OSC Control Debug View");
    }

    Rect windowRect = new Rect(100, 100, 560, 200);
    void DebugWindow(int windowID)
    {

    }

    private void ClientInstance_OnMessageReceived(OSCMessage msg)
    {

        if(msg.args.Count >= 2 && msg.args[1] is string)
        {
            if (OnOSCMessageReceived != null)
                OnOSCMessageReceived(msg.args[1] as string);
        }

        Debug.Log("OnNewMessage: "+msg.ToString());
    }
    #endregion
}

/// <summary>
/// 电机控制指令
/// </summary>
public enum MOTOR_COMMAND
{
    /// <summary>
    /// 按圈数旋转（float rev）
    /// </summary>
    RotateByRev,
    /// <summary>
    /// 停止旋转
    /// </summary>
    Stop,
    /// <summary>
    /// 缓慢正转
    /// </summary>
    StartRotateClockwise,
    /// <summary>
    /// 缓慢逆转
    /// </summary>
    StartRotateCounterClockwise,
    /// <summary>
    /// 停止旋转
    /// </summary>
    StopRotate,
    /// <summary>
    /// 设置原点
    /// </summary>
    ResetOrgion,
    /// <summary>
    /// 重置回原点
    /// </summary>
    ResetToOrgion,
}