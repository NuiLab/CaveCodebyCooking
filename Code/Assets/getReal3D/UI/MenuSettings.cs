using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using getReal3D;

public class MenuSettings : MonoBehaviour {

    public float? width;
    public float? depth;
    public float? maxRotationSpeed;
    public float? maxTranslationSpeed;

    [HideInInspector]
    public string menuButton = null;

    [HideInInspector]
    public string dragMenuButton = null;

	void Start () {
        menuButton = null;
        dragMenuButton = null;

        XmlElement unityElem = getReal3D.Plugin.getApplicationConfigXml();
        if(unityElem == null) {
            return;
        }

        XmlNodeList menus = unityElem.GetElementsByTagName("menu");
        if(menus.Count == 0) {
            return;
        }

        XmlElement menu = menus.Item(0) as XmlElement;

        width = readFromAttribute(menu, "width");
        depth = readFromAttribute(menu, "depth");
        maxRotationSpeed = readFromAttribute(menu, "max_rotation_speed");
        maxTranslationSpeed = readFromAttribute(menu, "max_translation_speed");
        menuButton = readStringFromAttribute(menu, "button");
        dragMenuButton = readStringFromAttribute(menu, "drag_button");

        if(menuButton != null) {
            WandEventModule wandEventModule = FindObjectOfType(typeof(WandEventModule)) as WandEventModule;
            wandEventModule.submitButtonName = menuButton;
        }
	}

    private float? readFromAttribute(XmlElement element, String attribute)
    {
        if(element.HasAttribute(attribute)) {
            string w = element.Attributes[attribute].Value;
            try {
                return Convert.ToSingle(w);
            }
            catch(Exception e) {
                Debug.LogWarning("Bad format for menu " + attribute + " '" + w + "': " + e.Message);
            }
        }
        return default(float?);
    }

    private string readStringFromAttribute(XmlElement element, String attribute)
    {
        if(element.HasAttribute(attribute)) {
            return element.Attributes[attribute].Value;
        }
        return null;
    }

}
