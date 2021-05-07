using System;
using System.Collections.Generic;
using System.Text;

public class SettingsProfile
{

    public float renderQuality = 0.8F;

    /// <summary>
    /// 缓存帧宽度
    /// </summary>
    public int cacheRenderWidth = 1920 / 2;
    /// <summary>
    /// 缓存帧高度
    /// </summary>
    public int cacheRenderHeight = 1080 / 2;

    /// <summary>
    /// 输入视频帧率
    /// </summary>
    public int cameraSourceFrameRate = 60;

    /// <summary>
    /// 输出视频帧率
    /// </summary>
    public int encodingFrameRate = 30;

    /// <summary>
    /// 预录制时长
    /// </summary>
    public float preRecordDuration = 5;

    /// <summary>
    /// 缓存时长
    /// </summary>
    public float cacheDuration = 10.0f;


    /// <summary>
    /// 延迟录制时长
    /// </summary>
    public float recordDelaySeconds = 0.2F;

    /// <summary>
    /// 录制时长
    /// </summary>
    public float recordDurationSeconds = 12F;

    /// <summary>
    /// 输出视频时长
    /// </summary>
    public float outputFileDuration = 11F;

    /// <summary>
    /// 目标加载AE速度曲线
    /// </summary>
    public string targetSpeedCurveProfile = "SpeedCurve.SPCAE";

    public string targetOverlayVideo = "Overlay.mov";


    public string commandCenterIP = "127.0.0.1";
    public int commandCenterPort = 8725;
    public string commandCenterName = "中控";
    public string commandCenterProjectName = "默认项目";
    public string commandCenterAddressName = "电机控制";

    public int motorRotateRevolution = 2;

    public string outputFolderPath = "";

    #region GETTER FUNCTIONS
    /// <summary>
    /// 摄像机帧间隔（秒）
    /// </summary>
    public float CameraSourceFrameInterval
    {
        get
        {
            return 1.0F / cameraSourceFrameRate;
        }
    }

    /// <summary>
    /// 编码帧间隔（秒）
    /// </summary>
    public float EncodingFrameInterval
    {
        get
        {
            return 1.0F / encodingFrameRate;
        }
    }

    /// <summary>
    /// 编码帧与摄像机帧间隔比率
    /// </summary>
    public float CameraFramerateToEncodingRatio
    {
        get
        {
            return cameraSourceFrameRate / encodingFrameRate;
        }
    }
    #endregion
}
