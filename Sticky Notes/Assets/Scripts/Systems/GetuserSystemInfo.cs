using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GetuserSystemInfo : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private void CheckUserId()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("deviceName")))
        {
            SceneManager.LoadSceneAsync("StickyNotes");
        }
    }

    public void Login()
    {
        PlayerPrefs.SetString("deviceName", SystemInfo.deviceName);
        PlayerPrefs.SetString("deviceId", SystemInfo.deviceUniqueIdentifier);
        SceneManager.LoadSceneAsync("StickyNotes");
       
    }
}
