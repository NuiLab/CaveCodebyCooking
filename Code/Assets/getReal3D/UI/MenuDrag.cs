using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Canvas))]
public class MenuDrag : MonoBehaviour {

    private Canvas m_canvas;
    private RectTransform m_rectTransform;
    public ShowMenu m_showMenu;
    public string m_wandButton = "WandButton";
    public WandEventModule wandEventModule;

    public Transform m_hand;
    private Transform m_originalParent;
    private bool m_sameButtonAsEventModule;

    void Start()
    {
        m_canvas = GetComponent<Canvas>() as Canvas;
        m_rectTransform = gameObject.transform as RectTransform;
        MenuSettings ms = GetComponentInParent<MenuSettings>() as MenuSettings;
        if(ms.dragMenuButton != null) {
            m_wandButton = ms.dragMenuButton;
        }
        m_originalParent = transform.parent;

        //WandEventModule wandEventModule = FindObjectOfType(typeof(WandEventModule)) as WandEventModule;
        wandEventModule = FindObjectOfType(typeof(WandEventModule)) as WandEventModule;
        m_sameButtonAsEventModule = wandEventModule.submitButtonName == m_wandButton;
    }
    
    void Update ()
    {
        bool pointerOnRect = RectTransformUtility.RectangleContainsScreenPoint(m_rectTransform, m_canvas.worldCamera.pixelRect.center, m_canvas.worldCamera);

        if(getReal3D.Input.GetButtonDown(m_showMenu.m_wandButton)) {
            if(!pointerOnRect){
                m_showMenu.clickOutside();
            }
        }

        if(getReal3D.Input.GetButtonDown(m_wandButton) && pointerOnRect && !m_sameButtonAsEventModule) {
            Grab();
        }
        
        if(getReal3D.Input.GetButtonUp(m_wandButton)) {
            Release();
        }
    }

    public void pointerUp()
    {
        if(m_sameButtonAsEventModule) {
            Release();
        }
    }

    public void pointerDown(UnityEngine.EventSystems.BaseEventData pData)
    {
        if(m_sameButtonAsEventModule) {
            Grab();
        }
    }

    void Grab()
    {
        transform.SetParent(m_hand.transform, true);
    }

    void Release()
    {
        transform.SetParent(m_originalParent, true);
    }
}
