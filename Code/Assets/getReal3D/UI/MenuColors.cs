using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuColors : MonoBehaviour {

    public Color m_textColor = Color.black;
    public Color m_panelColor = Color.white;
    public Color m_panelOutlineColor = Color.white;
    public Color m_buttonsColor = Color.white;
    public Color m_sliderFillColor = Color.white;
    public Color m_sliderColor = Color.white;
    public Color m_sliderHandleColor = Color.white;
    public Color m_toggleColor = Color.white;
    public Color m_toggleCheckmarkColor = Color.black;

    static private string[] panelPages = { "Panel", "Title", "PreviousButton", "NextButton" };

    void OnValidate()
    {
        updateMenu();
    }

    void Start()
    {
        updateMenu();
    }

    public void updateMenu()
    {
        handleGameObject(gameObject);
    }

    public void handleGameObject(GameObject page)
    {
        Text text = page.GetComponent<Text>() as Text;
        if(text){
            text.color = m_textColor;
        }

        Image image = page.GetComponent<Image>() as Image;
        if(image) {
            if(-1 != System.Array.IndexOf(panelPages, page.name)) {
                image.color = m_panelColor;
            }
            if(page.name.EndsWith("Outline")) {
                image.color = m_panelOutlineColor;
            }
        }

        Button button = page.GetComponent<Button>() as Button;
        if(button && image) {
            image.color = m_buttonsColor;
        }

        Slider slider = page.GetComponent<Slider>() as Slider;
        if(slider) {
            if(slider.fillRect) {
                Image imageFillRect = slider.fillRect.GetComponent<Image>() as Image;
                imageFillRect.color = m_sliderFillColor;
            }
            if(slider.handleRect){
                Image imageHandleRect = slider.handleRect.GetComponent<Image>() as Image;
                imageHandleRect.color = m_sliderHandleColor;
            }
        
            Image imageBackground = slider.transform.FindChild("Background").GetComponent<Image>() as Image;
            imageBackground.color = m_sliderColor;
        }

        Toggle toggle = page.GetComponent<Toggle>() as Toggle;
        if(toggle) {
            Graphic imageCheckmark = toggle.graphic;
            imageCheckmark.color = m_toggleCheckmarkColor;
            Image imageBackground = toggle.transform.FindChild("Background").GetComponent<Image>() as Image;
            imageBackground.color = m_toggleColor;
        }

        foreach(Transform t in page.transform){
            handleGameObject(t.gameObject);
        }
    }
}
