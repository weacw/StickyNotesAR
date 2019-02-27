using UnityEngine;
using System.IO;
using UnityEngine.XR.ARKit;
using Unity.Collections;
using System;
using System.Text;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HelperSystem
{
    private static HelperSystem Instance;
    public static HelperSystem GetHelper
    {
        get
        {
            if (Instance == null) Instance = new HelperSystem();
            return Instance;
        }
    }


    /// <summary>
    /// 保存ARWorldMap
    /// </summary>
    public void SaveARWorldMap(ARWorldMap _worldMap,string _path)
    {
        if (FileIsExists(_path))
            File.Delete(_path);
        
        var tmp_WorldMapData = _worldMap.Serialize(Allocator.Temp);
        var tmp_File = File.Open(_path, FileMode.Create);
        var tmp_Writer = new BinaryWriter(tmp_File);
        tmp_Writer.Write(tmp_WorldMapData.ToArray());
        tmp_Writer.Close();
        tmp_WorldMapData.Dispose();
    }


    /// <summary>
    /// 存储字符串到本地文件
    /// </summary>
    public void WriteToPath(string _path, string _text)
    {
        using (StreamWriter tmp_StreamWrite = new StreamWriter(_path))
        {
            tmp_StreamWrite.Write(_text);
            tmp_StreamWrite.Close();
        }
    }

    public void WriteByteToLoad(string _path,byte[] _bytes)
    {
        File.WriteAllBytes(_path, _bytes);
    }

    public void GetTexture2DByPath(string _path,out Texture2D _result,Rect _rect) 
    {
        _result = new Texture2D((int)_rect.width,(int)_rect.height,TextureFormat.RGB24,false);
        _result.LoadImage(File.ReadAllBytes(_path));
    }


    /// <summary>
    /// 读取字符串来自文件
    /// </summary>
    public string GetStringFromFile(string _path)
    {
        return File.ReadAllText(_path, Encoding.Default);
    }


    /// <summary>
    /// 文件是否存在
    /// </summary>
    public bool FileIsExists(string _path)
    {
        return File.Exists(_path);
    }


    /// <summary>
    /// 获取APP根目录
    /// </summary>
    /// <value>根目录路径</value>
    public string GetBasePath
    {
        get
        {
            return Application.persistentDataPath;
        }
    }

    /// <summary>
    /// 获取当前时间的时间戳
    /// </summary>
    /// <value>当前时间戳</value>
    public string GetTimeStamp
    {
        get
        {
            TimeSpan tmp_TimeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToUInt64(tmp_TimeSpan.TotalSeconds).ToString();
        }
    }


    /// <summary>
    /// 获取当前日期
    /// </summary>
    /// <value>yyyy:m:d</value>
    public string GetData
    {
        get
        {
            return DateTime.Now.ToString("yyyy-M-d");
        }
    }

    /// <summary>
    /// 检查当前点击的对象是否为UGUI对象
    /// </summary>
    /// <returns><c>true</c>, if GUI raycast objects was checked, <c>false</c> otherwise.</returns>
    public bool CheckGuiRaycastObjects()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            pressPosition = Input.mousePosition,
            position = Input.mousePosition
        };

        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, list);
        return list.Count > 0;
    }

}
