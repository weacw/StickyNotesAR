using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class FocusSquareSystem : MonoBehaviour
{
    private List<ARRaycastHit> m_HitList = new List<ARRaycastHit>();
    private Transform m_CameraTransform;
    private Material m_FocusSquareMat;
    private static ARRaycastHit m_Rayhit;
    private bool m_Created;

    public float m_FindingSquareDist = .5f;
    public ARSessionOrigin m_ARSessionOrigin;
    public Transform m_FocusSquareTransform;

    public Texture m_Found;
    public Texture m_Finding;
    public Camera m_MainCamera;
    public GameObject m_tips;

    public static ARRaycastHit GetRayHit { get { return m_Rayhit; } }



    private void Awake()
    {
        m_FocusSquareMat = m_FocusSquareTransform.GetComponent<Renderer>().material;
        m_CameraTransform = m_MainCamera.transform;
        m_FocusSquareMat.SetTexture("_MainTex", m_Finding);
    }

    private void Update()
    {
        if (m_ARSessionOrigin == null || ARSubsystemManager.systemState == ARSystemState.Ready)
        {
            m_FocusSquareTransform.gameObject.SetActive(false);
            m_tips.SetActive(false);
            return;
        }

        m_FocusSquareTransform.gameObject.SetActive(true);
        m_tips.SetActive(true);

        Vector3 tmp_Center = new Vector3(Screen.width / 2, Screen.height / 2, m_FindingSquareDist);
        if (m_ARSessionOrigin.Raycast(tmp_Center, m_HitList, TrackableType.PlaneWithinBounds))
        {
            m_FocusSquareMat.SetTexture("_MainTex", m_Found);
            Pose tmp_HitPose = m_HitList[0].pose;
            m_FocusSquareTransform.position = new Vector3(tmp_HitPose.position.x, tmp_HitPose.position.y + 0.01f, tmp_HitPose.position.z);
            m_FocusSquareTransform.rotation = tmp_HitPose.rotation;
            m_Rayhit = m_HitList[0];
            m_tips.SetActive(false);
            if (!m_Created)
            {
                NotifCenterSystem.GetNotice.PostDispatchEvent(NotifEventKeyComponent.OnFoundOrFindingPlane, new AssingObject<bool>(true));
                m_Created = true;
            }

            return;
        }

        m_FocusSquareMat.SetTexture("_MainTex", m_Finding);
        float tmp_Angle = Vector3.Dot(m_CameraTransform.forward, Vector3.down);
        if (tmp_Angle > 0)
        {
            Vector3 tmp_WorldPosition = m_MainCamera.ScreenToWorldPoint(tmp_Center);
            m_FocusSquareTransform.position = tmp_WorldPosition;
            Vector3 tmp_VecToCamera = m_FocusSquareTransform.position - m_CameraTransform.position;
            Vector3 vecOrthoghonal = Vector3.Cross(tmp_VecToCamera, Vector3.up);
            Vector3 vecForward = Vector3.Cross(vecOrthoghonal, Vector3.up);
            m_FocusSquareTransform.rotation = Quaternion.LookRotation(vecForward, Vector3.up);
        }
        m_tips.SetActive(true);

        if (m_Created)
        {
            NotifCenterSystem.GetNotice.PostDispatchEvent(NotifEventKeyComponent.OnFoundOrFindingPlane, new AssingObject<bool>(false));
            m_Created = false;
        }
    }
}
