using UnityEngine;
using System.Collections;

public class RenderingSettings : MonoBehaviour {

    public UnityEngine.UI.Slider m_renderingPathSlider;
    public UnityEngine.UI.Slider m_qualitySlider;
    public UnityEngine.UI.Text m_qualityName;
    public UnityEngine.UI.Text m_fps;
    public UnityEngine.UI.Text m_renderingPathName;
    public UnityEngine.UI.Text m_updater;

    private GameObject[] m_camerasObjects;

    void Start () {
        m_camerasObjects = GameObject.FindGameObjectsWithTag("MainCamera");

        m_qualitySlider.minValue = 0;
        m_qualitySlider.maxValue = QualitySettings.names.Length - 1;
        m_qualitySlider.wholeNumbers = true;
        m_qualitySlider.value = QualitySettings.GetQualityLevel();

        m_renderingPathSlider.minValue = 0;
        m_renderingPathSlider.maxValue = System.Enum.GetNames(typeof(RenderingPath)).Length - 1;
        m_renderingPathSlider.wholeNumbers = true;
        string[] renderingPathNames = System.Enum.GetNames(typeof(RenderingPath));
        RenderingPath currentRenderingPath = getRenderingPath();
        m_renderingPathSlider.value = System.Array.IndexOf(renderingPathNames, currentRenderingPath.ToString());

        
        updateQualityName();
        updateRenderingPathName();
    }
    
    void Update () {
        m_fps.text = (1.0f / Time.smoothDeltaTime).ToString("##0.00");
        updateText();
    }

    public void qualitySliderChanged(float val)
    {
        int qualityLevel = (int) val;
        int currentQualityLevel = QualitySettings.GetQualityLevel();
        if(qualityLevel != currentQualityLevel) {
            QualitySettings.SetQualityLevel(qualityLevel, false);
        }
        updateQualityName();
    }

    public void renderingPathSliderChanged(float val)
    {
        string[] renderingPathNames = System.Enum.GetNames(typeof(RenderingPath));
        string renderingPathName = renderingPathNames[(int) val];
        RenderingPath rp = (RenderingPath) System.Enum.Parse(typeof(RenderingPath), renderingPathName);
        foreach(GameObject cam in m_camerasObjects) {
            cam.GetComponent<Camera>().renderingPath = rp;
        }
        updateRenderingPathName();
    }

    private void updateText()
    {
        m_updater.text = "Derrick".ToString();
    }
    private void updateQualityName()
    {
        m_qualityName.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
    }

    private void updateRenderingPathName()
    {
        m_renderingPathName.text = getRenderingPath().ToString();
    }

    private RenderingPath getRenderingPath()
    {
        if(m_camerasObjects.Length == 0) {
            return RenderingPath.DeferredLighting;
        }
        else {
            return m_camerasObjects[0].GetComponent<Camera>().renderingPath;
        }
    }
}
