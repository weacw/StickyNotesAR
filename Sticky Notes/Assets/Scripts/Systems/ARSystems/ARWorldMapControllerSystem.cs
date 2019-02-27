using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARExtensions;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif


using System.IO;
public class ARWorldMapControllerSystem : MonoBehaviour
{
    private string m_Timestamp;
    public string CurrentLoadARWorldMapFileName { get; private set; }

    private static ARWorldMapControllerSystem m_Instance;
    public static ARWorldMapControllerSystem GetARWorldMapSystem
    {
        get
        {
            if (m_Instance == null)
                m_Instance = FindObjectOfType<ARWorldMapControllerSystem>();
            return m_Instance;
        }
    }


    private void OnEnable()
    {
        NotifCenterSystem.GetNotice.AddEventListener(NotifEventKeyComponent.SaveARContentData, SaveARContentData);
        NotifCenterSystem.GetNotice.AddEventListener(NotifEventKeyComponent.OnSaveARWorldMap, OnSaveARWorldMap);
        NotifCenterSystem.GetNotice.AddEventListener(NotifEventKeyComponent.OnLoadARWorldMap, OnLoadARWorldMap);
    }

    private void OnDisable()
    {
        NotifCenterSystem.GetNotice.RemoveEventListener(NotifEventKeyComponent.SaveARContentData, SaveARContentData);
        NotifCenterSystem.GetNotice.RemoveEventListener(NotifEventKeyComponent.OnSaveARWorldMap, OnSaveARWorldMap);
        NotifCenterSystem.GetNotice.RemoveEventListener(NotifEventKeyComponent.OnLoadARWorldMap, OnLoadARWorldMap);
    }

    private void OnSaveARWorldMap(Notification _notif)
    {
        AssingObject<string> tmp_InputName = _notif.param as AssingObject<string>;
        string tmp_FilePath = string.Empty;
        if (string.IsNullOrEmpty(CurrentLoadARWorldMapFileName))
        {
            m_Timestamp = HelperSystem.GetHelper.GetTimeStamp;
            StickyNotePanelData tmp_Data = new StickyNotePanelData()
            {
                m_Id = m_Timestamp,
                m_Timestamp = HelperSystem.GetHelper.GetData,
                m_Title = tmp_InputName.m_Object
            };
            NotifCenterSystem.GetNotice.PostDispatchEvent(NotifEventKeyComponent.SaveStickyNoteDateToPanel, new AssingObject<StickyNotePanelData>(tmp_Data));
            tmp_FilePath = Path.Combine(HelperSystem.GetHelper.GetBasePath, tmp_InputName.m_Object + m_Timestamp + ".worldmap");
        }
        else
        {
            tmp_FilePath = HelperSystem.GetHelper.GetBasePath+"/"+ CurrentLoadARWorldMapFileName;
        }

        AssingObject<string> tmp_Json = new AssingObject<string>(JsonUtility.ToJson(StickyNoteManagerSystem.GetStickyNote.m_ARContentListComponent));
        NotifCenterSystem.GetNotice.PostDispatchEvent(NotifEventKeyComponent.SaveARContentData, tmp_Json);
        StartCoroutine(SaveARWorldMap(tmp_FilePath));
        CurrentLoadARWorldMapFileName = null;
    }

    private void OnLoadARWorldMap(Notification _notif)
    {
        AssingObject<StickyNotePanelData> tmp_StickyNotePanelData = _notif.param as AssingObject<StickyNotePanelData>;
        CurrentLoadARWorldMapFileName = tmp_StickyNotePanelData.m_Object.m_Title + tmp_StickyNotePanelData.m_Object.m_Id + ".worldmap";
        StartCoroutine(LoadARWorldMap(tmp_StickyNotePanelData.m_Object));
    }

    /// <summary>
    /// 保存ARWorldmap
    /// </summary>
    private IEnumerator SaveARWorldMap(string _fileName)
    {
        var tmp_SessionSubSystem = ARSubsystemManager.sessionSubsystem;
        if (tmp_SessionSubSystem == null)
        {
            Debug.Log("No seesion");
            yield break;
        }
        var tmp_Request = tmp_SessionSubSystem.GetARWorldMapAsync();
        while (!tmp_Request.status.IsDone()) yield return null;
        if (tmp_Request.status.IsError())
        {
            Debug.Log("No seesion");
            yield break;
        }
        var tmp_WorldMap = tmp_Request.GetWorldMap();
        tmp_Request.Dispose();

        string tmp_WorldMapPath = _fileName;
        HelperSystem.GetHelper.SaveARWorldMap(tmp_WorldMap, tmp_WorldMapPath);

        ARSubsystemManager.StopSubsystems();
        ARSubsystemManager.DestroySubsystems();
    }

    /// <summary>
    /// 加载ARWorldmap
    /// </summary>
    private IEnumerator LoadARWorldMap(StickyNotePanelData _data)
    {
        ARSubsystemManager.CreateSubsystems();
        ARSubsystemManager.StartSubsystems();
        //UIManagerSystem.Get.GetUIByName<StickyNotePanelEntity>().DisAppear();
        //UIManagerSystem.Get.GetUIByName<StickyNoteWritePanelEntity>().DisAppear();
        //StickyNoteARViewEntity tmp_StickyNoteARViewEntity = UIManagerSystem.Get.GetUIByName<StickyNoteARViewEntity>();
        //tmp_StickyNoteARViewEntity.Init();
        //tmp_StickyNoteARViewEntity.Appear();

        yield return new WaitForSeconds(2);

        var tmp_SessionSubSystem = ARSubsystemManager.sessionSubsystem;
        if (tmp_SessionSubSystem == null)
        {
            Debug.Log("No session");
            yield break;
        }
        string tmp_WorldMapPath = Path.Combine(HelperSystem.GetHelper.GetBasePath, _data.m_Title + _data.m_Id + ".worldmap");
        var tmp_File = File.Open(tmp_WorldMapPath, FileMode.Open);
        if (tmp_File == null)
        {
            Debug.Log(string.Format("File {0} does not exist.", tmp_WorldMapPath));
            yield break;
        }

        int tmp_BytesPerFrame = 1024 * 10;
        var tmp_BytesRemaining = tmp_File.Length;
        var tmp_BinaryReader = new BinaryReader(tmp_File);
        var tmp_AllBytes = new List<byte>();
        while (tmp_BytesRemaining > 0)
        {
            var tmp_Bytes = tmp_BinaryReader.ReadBytes(tmp_BytesPerFrame);
            tmp_AllBytes.AddRange(tmp_Bytes);
            tmp_BytesRemaining -= tmp_BytesPerFrame;
            yield return null;
        }

        var tmp_Data = new NativeArray<byte>(tmp_AllBytes.Count, Allocator.Temp);
        tmp_Data.CopyFrom(tmp_AllBytes.ToArray());

        if (ARWorldMap.TryDeserialize(tmp_Data, out ARWorldMap tmp_WorldMap))
            tmp_Data.Dispose();
        if (tmp_WorldMap.valid) Debug.Log("Deserialized successfully");
        else
        {
            Debug.LogError("Data is not a valid ARWorldMap.");
            yield break;
        }
        tmp_SessionSubSystem.ApplyWorldMap(tmp_WorldMap);
        ARWorldMappingStatus tmp_MappingStatus = tmp_SessionSubSystem.GetWorldMappingStatus();


        while (tmp_MappingStatus == ARWorldMappingStatus.NotAvailable)
        {
            yield return null;
            Debug.Log(tmp_MappingStatus);
        }

        yield return new WaitForSeconds(1);


        string tmp_ContentFromFile = HelperSystem.GetHelper.GetStringFromFile(Path.Combine(Application.persistentDataPath, "ARContent_" + _data.m_Id + ".json"));
        if (string.IsNullOrEmpty(tmp_ContentFromFile)) yield break;
        NotifCenterSystem.GetNotice.PostDispatchEvent(NotifEventKeyComponent.ReduceARStickNoteContent, new AssingObject<string>(tmp_ContentFromFile));

    }


    /// <summary>
    /// 保存AR世界中的对象
    /// </summary>
    private void SaveARContentData(Notification _notif)
    {
        AssingObject<string> tmp_Json = _notif.param as AssingObject<string>;
        HelperSystem.GetHelper.WriteToPath(HelperSystem.GetHelper.GetBasePath + "/ARContent_" + m_Timestamp + ".json", tmp_Json.m_Object);
    }

    private bool WorldMapSupported
    {
        get
        {
#if UNITY_IOS
            var sessionSubsystem = ARSubsystemManager.sessionSubsystem;
            if (sessionSubsystem != null)
                return sessionSubsystem.WorldMapSupported();
#endif
            return false;
        }
    }

}
