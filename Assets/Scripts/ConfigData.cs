using UnityEngine;

[System.Serializable]
public class ConfigData
{
    public string apiUrl;

    public static ConfigData LoadConfig(string path)
    {
        TextAsset configFile = Resources.Load<TextAsset>(path);
        if (configFile != null)
        {
            return JsonUtility.FromJson<ConfigData>(configFile.text);
        }
        else
        {
            Debug.LogError("Config file not found at path: " + path);
            return null;
        }
    }
}
