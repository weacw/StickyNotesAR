using System;
using UnityEngine;


public enum NotifEventKeyComponent
{
    //WEAPON INFO
    UPDATE_AMMO_UI_TEXT,
    SETUP_WEAPONICON_IMAGE,
    SETUP_ADDOREDITTHROWINGOBJECT_IMAGE,
    SETUP_UPDATETHROWINGOBJECT_IMAGE,
    UPDATE_CROSSHARISPOSITION,
    TRIGGER_FLASHBANG_IMAGE,
    OnFoundOrFindingPlane,
    OnCreateStickNote,
    SetInputStringToContentText,
    SaveARContentData,
    SaveStickyNoteDateToPanel,
    ReduceARStickNoteContent,
    OnSaveARWorldMap,
    OnLoadARWorldMap,
    CleanupStickNoteGameObjet,
}

public class Notification
{
    public EventArgs param;
    public Notification(EventArgs param)
    {
        this.param = param;
    }
    public Notification()
    {

    }
}

public class BoolWrapper
{
    public bool Value { get; set; }
    public BoolWrapper(bool _value)
    {
        Value = _value;
    }
}

public class AssingObject<T1, T2> : EventArgs
{
    public T1 m_ObjectA { get; private set; }
    public T2 m_ObjectB;
    public AssingObject(T1 _object, ref T2 _t2)
    {
        m_ObjectA = _object;
        m_ObjectB = _t2;
    }

    public AssingObject(T1 _object, T2 _t2)
    {
        m_ObjectA = _object;
        m_ObjectB = _t2;
    }

    public void SetContent(T1 _object, T2 _t2)
    {
        m_ObjectA = _object;
        m_ObjectB = _t2;
    }
    public AssingObject() { }
}


public class AssingObject<T> : EventArgs
{
    private StickyNotePanelData m_StickNotePanelData;

    public T m_Object { get; private set; }
    public AssingObject(T _object)
    {
        m_Object = _object;
    }
}