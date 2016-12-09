using UnityEngine;
using System.Collections;

public class NavigationUI : MonoBehaviour {

    public UnityEngine.UI.Slider m_translationSpeedSlider = null;
    public UnityEngine.UI.Slider m_rotationSpeedSlider = null;
    public GameObject player = null;

	void Start () {
        MenuSettings ms = GetComponentInParent<MenuSettings>() as MenuSettings;
        if(ms) {
            if(ms.maxRotationSpeed.HasValue) {
                m_rotationSpeedSlider.maxValue = ms.maxRotationSpeed.Value;
            }
            if(ms.maxTranslationSpeed.HasValue) {
                m_translationSpeedSlider.maxValue = ms.maxTranslationSpeed.Value;
            }
        }

        retrieveFromNavOptions();
	}

    void Update()
    {
        retrieveFromNavOptions();
    }

    public void setRotationSpeed(float val)
    {
        getReal3D.Input.NavOptions.SetValue<float>("RotationSpeed", val);
    }

    public void setTranslationSpeed(float val)
    {
        getReal3D.Input.NavOptions.SetValue<float>("TranslationSpeed", val);
    }

    public void changeNavigation(int val)
    {
        switch(val) {
        case 0:
            player.GetComponent<getRealAimAndGoController>().enabled = false;
            player.GetComponent<getRealWandDriveController>().enabled = false;
            player.GetComponent<getRealWalkthruController>().enabled = true;
            player.GetComponent<getRealWandLook>().ContinuousDrive = false;
            break;
        case 1:
            player.GetComponent<getRealWalkthruController>().enabled = false;
            player.GetComponent<getRealWandDriveController>().enabled = false;
            player.GetComponent<getRealAimAndGoController>().enabled = true;
            player.GetComponent<getRealWandLook>().ContinuousDrive = false;
            break;
        case 2:
            player.GetComponent<getRealWalkthruController>().enabled = false;
            player.GetComponent<getRealWandDriveController>().enabled = true;
            player.GetComponent<getRealAimAndGoController>().enabled = false;
            player.GetComponent<getRealWandLook>().ContinuousDrive = true;
            break;
        }
    }

    private void retrieveFromNavOptions()
    {
        float val = 1.0f;
        getReal3D.Input.NavOptions.GetValue<float>("RotationSpeed", ref val);
        setSliderValue(m_rotationSpeedSlider, val);

        val = 1.0f;
        getReal3D.Input.NavOptions.GetValue<float>("TranslationSpeed", ref val);
        setSliderValue(m_translationSpeedSlider, val);
    }

    private void setSliderValue(UnityEngine.UI.Slider slider, float val)
    {
        if(val < slider.minValue) {
            return;
        }
        if(val > slider.maxValue) {
            slider.maxValue = 2 * val;
        }
        if(val != slider.value) {
            slider.value = val;
        }
    }

}
