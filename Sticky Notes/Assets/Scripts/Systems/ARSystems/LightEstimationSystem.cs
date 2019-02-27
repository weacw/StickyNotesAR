using UnityEngine;
using UnityEngine.XR.ARFoundation;
public class LightEstimationSystem : MonoBehaviour
{

    private Light m_Light;
    public float? Bbrightness { get; private set; }
    public float? ColorTemperature { get; private set; }
    public Color? CcolorCorrection { get; private set; }

    private void Awake()
    {
        m_Light = GetComponent<Light>();
        ARSubsystemManager.cameraFrameReceived += FrameChanged;
    }

    private void FrameChanged(ARCameraFrameEventArgs _args)
    {
        if (_args.lightEstimation.averageBrightness.HasValue)
        {
            Bbrightness = _args.lightEstimation.averageBrightness.Value;
            m_Light.intensity = Bbrightness.Value;
        }

        if (_args.lightEstimation.averageColorTemperature.HasValue)
        {
            ColorTemperature = _args.lightEstimation.averageColorTemperature.Value;
            m_Light.colorTemperature = ColorTemperature.Value;
        }

        if (_args.lightEstimation.colorCorrection.HasValue)
        {
            CcolorCorrection = _args.lightEstimation.colorCorrection.Value;
            m_Light.color = CcolorCorrection.Value;
        }
    }
}
