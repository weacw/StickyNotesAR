using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(ARPlaneMeshVisualizer), typeof(MeshRenderer), typeof(ARPlane))]
public class PlaceOnPlane : MonoBehaviour
{
    internal ARSessionOrigin m_ARSessionOrigin;
    private List<ARRaycastHit> m_RaycastHitList = new List<ARRaycastHit>();
    public GameObject m_Prefab;
    public Transform m_Content;
    public ARStickyNoteListComponent m_ContentList = new ARStickyNoteListComponent();
    public Transform Content
    {
        get { return m_Content; }
        set { m_Content = value; }
    }

    Quaternion m_Rotation;
    public Quaternion rotation
    {
        get { return m_Rotation; }
        set
        {
            m_Rotation = value;
            if (m_ARSessionOrigin != null)
                m_ARSessionOrigin.MakeContentAppearAt(Content, Content.transform.position, m_Rotation);
        }
    }


    private void Awake()
    {
        m_ARSessionOrigin = GetComponent<ARSessionOrigin>();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            if (m_ARSessionOrigin.Raycast(Input.mousePosition, m_RaycastHitList, TrackableType.PlaneWithinPolygon))
            {
                var tmp_HitPose = m_RaycastHitList[0].pose;

                ARPlane tmp_ARPlane = GetComponent<ARPlaneManager>().TryGetPlane(m_RaycastHitList[0].trackableId);
                Vector3 tmp_ContentEulerAngle = Vector3.zero;
                if (tmp_ARPlane.boundedPlane.Alignment == PlaneAlignment.Horizontal)
                {
                    tmp_ContentEulerAngle = new Vector3(90, tmp_HitPose.rotation.y, tmp_HitPose.rotation.z);
                }
                else if (tmp_ARPlane.boundedPlane.Alignment == PlaneAlignment.Vertical)
                {

                }
                else
                {
                    tmp_ContentEulerAngle = new Vector3(0, tmp_HitPose.rotation.y, tmp_HitPose.rotation.z);
                }


                Content = Instantiate(m_Prefab).transform;
                Content.gameObject.SetActive(true);
                Content.position = tmp_HitPose.position;
                Content.rotation = Quaternion.Euler(tmp_ContentEulerAngle);

                ARStickyNotePoseComponent tmp_ContentData = new ARStickyNotePoseComponent();
                tmp_ContentData.m_ContentId = m_Prefab.name;
                tmp_ContentData.m_ContentPosition = tmp_HitPose.position;
                tmp_ContentData.m_ContentRotation = Quaternion.Euler(tmp_ContentEulerAngle);
                m_ContentList.m_Contents.Add(tmp_ContentData);
            }
        }
    }


    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current == null)
            return false;
        PointerEventData tmp_EventData = new PointerEventData(EventSystem.current);
        tmp_EventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> tmp_Results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(tmp_EventData, tmp_Results);
        return tmp_Results.Count > 0;
    }

}


