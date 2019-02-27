using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 便笺在AR世界中的数据
/// </summary>
[System.Serializable]
public struct ARStickyNotePoseComponent
{
    public string m_ContentId;
    public string m_Content;
    public Vector3 m_ContentPosition;
    public Quaternion m_ContentRotation;
}


/// <summary>
/// 便笺在AR世界中的列表
/// </summary>
[System.Serializable]
public class ARStickyNoteListComponent
{
    public List<ARStickyNotePoseComponent> m_Contents = new List<ARStickyNotePoseComponent>();
}

/// <summary>
/// 保存便笺场景属性
/// </summary>
[System.Serializable]
public struct StickyNotePanelData
{
    public string m_Id;
    public string m_Title;
    public string m_Timestamp;
    public string m_ImagePath;
}


/// <summary>
/// 保存便笺场景列表
/// </summary>
[System.Serializable]
public class StickyNoteDatabase
{
    public List<StickyNotePanelData> m_StickNoteDatebase = new List<StickyNotePanelData>();
}
