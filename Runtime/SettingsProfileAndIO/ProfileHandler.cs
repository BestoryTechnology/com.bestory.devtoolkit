using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ProfileHandler : MonoBehaviour
{

    public SettingsProfile settings = new SettingsProfile();

    public static ProfileHandler Instance;

    public static SettingsProfile Settings
    {
        get
        {
            return Instance == null ? null : Instance.settings;
        }
    }

    // Use this for initialization
    void Awake ()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        LoadSettings();
	}

    IniFile iniProfile = new IniFile();
    void LoadSettings()
    {
        Debug.Log("IniFile.defaultFolderPath: " + IniFile.defaultFolderPath);

        if (!Directory.Exists(IniFile.defaultFolderPath))
        {
            Directory.CreateDirectory(IniFile.defaultFolderPath);
        }
        if (!File.Exists(IniFile.defaultFilePath))
        {
            iniProfile.WriteClassDataToSection(settings);
            iniProfile.Save();
        }
        iniProfile.Load();
        iniProfile.LoadClassDataFromSection(settings);
    }

}
