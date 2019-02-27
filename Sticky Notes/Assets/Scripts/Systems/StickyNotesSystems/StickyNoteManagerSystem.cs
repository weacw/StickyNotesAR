using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class StickyNoteManagerSystem : MonoBehaviour
{
    public System.Action<bool> OnFoundPanelCallBack;
    private static StickyNoteManagerSystem m_Instance;
    public static StickyNoteManagerSystem GetStickyNote
    {
        get
        {
            if (m_Instance == null)
                m_Instance = FindObjectOfType<StickyNoteManagerSystem>();
            return m_Instance;
        }
    }

    public GameObject m_StickyNotePrefab;
    private string m_InputContent;
    public ARPlaneManager m_ARPlaneManager;

    //private Vector3 m_StickyNotePosition;
    //private Quaternion m_StickyNoteRotation;
    public List<GameObject> m_StickyNoteGameObjectList = new List<GameObject>();
    public ARStickyNoteListComponent m_ARContentListComponent = new ARStickyNoteListComponent();
    public StickyNoteDatabase m_StickyNoteDatabase = new StickyNoteDatabase();

    public IEnumerator ScreenShot(Rect _rect, string _fileName)
    {
        yield return new WaitForEndOfFrame();
        Texture2D tmp_Texture2D = new Texture2D((int)_rect.width, (int)_rect.height, TextureFormat.RGB24, false);
        tmp_Texture2D.ReadPixels(_rect, 0, 0);
        tmp_Texture2D.Compress(false);
        tmp_Texture2D.Apply(false);
        HelperSystem.GetHelper.WriteByteToLoad(_fileName, tmp_Texture2D.EncodeToJPG(50));
    }



    private void Awake()
    {
        //添加事件
        NotifCenterSystem.GetNotice.AddEventListener(NotifEventKeyComponent.OnFoundOrFindingPlane, OnFoundOrFindingPlane);
        NotifCenterSystem.GetNotice.AddEventListener(NotifEventKeyComponent.OnCreateStickNote, OnCreateStickNoteWithARRaycastHit);
        NotifCenterSystem.GetNotice.AddEventListener(NotifEventKeyComponent.SetInputStringToContentText, SetInputStringToContentText);
        NotifCenterSystem.GetNotice.AddEventListener(NotifEventKeyComponent.SaveStickyNoteDateToPanel, SaveStickyNoteDataToPanel);
        NotifCenterSystem.GetNotice.AddEventListener(NotifEventKeyComponent.ReduceARStickNoteContent, ResumeARStickNoteContent);
        NotifCenterSystem.GetNotice.AddEventListener(NotifEventKeyComponent.CleanupStickNoteGameObjet, CleanupStickNoteGameObjet);

        m_ARPlaneManager = GetComponent<ARPlaneManager>();

        StartCoroutine(WaitToStopARSystem());

        if (HelperSystem.GetHelper.FileIsExists(HelperSystem.GetHelper.GetBasePath + "/StickyNotePanelList.json"))
        {
            var tmp_StickyNotePanelList = HelperSystem.GetHelper.GetStringFromFile(HelperSystem.GetHelper.GetBasePath + "/StickyNotePanelList.json");
            m_StickyNoteDatabase = JsonUtility.FromJson<StickyNoteDatabase>(tmp_StickyNotePanelList);
        }

        //StickyNotePanelEntity tmp_PanelEntity = UIManagerSystem.Get.GetUIByName<StickyNotePanelEntity>();
        //tmp_PanelEntity.Init();
        //tmp_PanelEntity.Appear();
    }


    /// <summary>
    /// 寻找平面/找到平面
    /// </summary>
    private void OnFoundOrFindingPlane(Notification _notif)
    {
        AssingObject<bool> tmp_Status = _notif.param as AssingObject<bool>;
        if (OnFoundPanelCallBack != null) OnFoundPanelCallBack.Invoke(tmp_Status.m_Object);
    }


    /// <summary>
    /// 设置输入的文字内容
    /// </summary>
    private void SetInputStringToContentText(Notification _notif)
    {
        AssingObject<string> tmp_Message = _notif.param as AssingObject<string>;
        m_InputContent = tmp_Message.m_Object;
    }


    /// <summary>
    /// 保存当前环境到本地
    /// </summary>
    private void SaveStickyNoteDataToPanel(Notification _notif)
    {
        AssingObject<StickyNotePanelData> tmp_StickyNotePanelData = _notif.param as AssingObject<StickyNotePanelData>;
        m_StickyNoteDatabase.m_StickNoteDatebase.Add(tmp_StickyNotePanelData.m_Object);
        string tmp_Json = JsonUtility.ToJson(m_StickyNoteDatabase);
        HelperSystem.GetHelper.WriteToPath(HelperSystem.GetHelper.GetBasePath + "/StickyNotePanelList.json", tmp_Json);
    }

    /// <summary>
    /// 创建stcik note - AR对象
    /// </summary>
    private void OnCreateStickNoteWithARRaycastHit(Notification _notif)
    {
        AssingObject<string, ARRaycastHit> tmp_Data = _notif.param as AssingObject<string, ARRaycastHit>;
        TrackableId tmp_TrackableId = tmp_Data.m_ObjectB.trackableId;

        Vector3 tmp_ContentRotation = Vector3.zero;
        Vector3 tmp_Position = tmp_Data.m_ObjectB.pose.position;
        Quaternion tmp_Rotation = tmp_Data.m_ObjectB.pose.rotation;

        ARPlane tmp_ARPlane = m_ARPlaneManager.TryGetPlane(tmp_TrackableId);
        tmp_ContentRotation = tmp_ARPlane.boundedPlane.Alignment == PlaneAlignment.Horizontal ?
         new Vector3(90, tmp_Rotation.y, tmp_ARPlane.transform.eulerAngles.z) :
         new Vector3(0, tmp_ARPlane.transform.eulerAngles.y, tmp_ARPlane.transform.eulerAngles.z);

        GameObject tmp_StickNote = Instantiate(m_StickyNotePrefab);

        //TODO:初始化数据到便利贴上
        StickyNoteItem tmp_StickNoteItem = tmp_StickNote.GetComponent<StickyNoteItem>();
        tmp_StickNoteItem.m_ContentText.text = m_InputContent;

        m_StickyNoteGameObjectList.Add(tmp_StickNote);
        Pose tmp_Pose = tmp_Data.m_ObjectB.pose;
        Transform tmp_Transform = tmp_StickNote.transform;
        tmp_Transform.position = tmp_Position;
        tmp_Transform.localRotation = Quaternion.Euler(tmp_ContentRotation);
        tmp_StickNote.SetActive(true);


        ARStickyNotePoseComponent tmp_ContentData = new ARStickyNotePoseComponent()
        {
            m_ContentId = tmp_StickNote.GetHashCode().ToString(),
            m_Content = m_InputContent,
            m_ContentPosition = tmp_Transform.localPosition,
            m_ContentRotation = tmp_Transform.localRotation
        };
        m_ARContentListComponent.m_Contents.Add(tmp_ContentData);
    }


    /// <summary>
    ///恢复AR Sticky note 内容在 ARWorld 中
    /// </summary>
    private void ResumeARStickNoteContent(Notification _notif)
    {
        AssingObject<string> tmp_ARStickNoteContentJson = _notif.param as AssingObject<string>;
        ARStickyNoteListComponent tmp_ARStickNoteList = JsonUtility.FromJson<ARStickyNoteListComponent>(tmp_ARStickNoteContentJson.m_Object);
        foreach (ARStickyNotePoseComponent tmp_Content in tmp_ARStickNoteList.m_Contents)
        {
            if (m_StickyNotePrefab == null) break;
            GameObject tmp_StickNote = Instantiate(m_StickyNotePrefab, tmp_Content.m_ContentPosition, tmp_Content.m_ContentRotation);
            Debug.Log(tmp_Content.m_Content);
            tmp_StickNote.GetComponent<StickyNoteItem>().m_ContentText.text = tmp_Content.m_Content;
            tmp_StickNote.SetActive(true);
            m_StickyNoteGameObjectList.Add(tmp_StickNote);
        }
    }


    /// <summary>
    /// 清理AR世界中的GameObject 对象
    /// </summary>
    private void CleanupStickNoteGameObjet(Notification _notif)
    {
        foreach (var tmp_stickyNote in m_StickyNoteGameObjectList)
            Destroy(tmp_stickyNote);

        m_StickyNoteGameObjectList.Clear();
    }

    private IEnumerator WaitToStopARSystem()
    {
        yield return new WaitForSeconds(0.25f);
        ARSubsystemManager.StopSubsystems();
        ARSubsystemManager.DestroySubsystems();
    }
}
