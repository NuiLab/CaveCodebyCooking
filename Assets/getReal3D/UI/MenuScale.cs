using UnityEngine;
using System;
using System.Collections;


[RequireComponent(typeof(RectTransform))]
public class MenuScale : MonoBehaviour
{

    public float m_menuWidth = 1;
    private float m_scalePerMeter = 0.00125f; /// magic number!

    void Start()
    {
        MenuSettings ms = GetComponentInParent<MenuSettings>() as MenuSettings;
        if(ms.width.HasValue) {
            m_menuWidth = ms.width.Value;
        }
        updateMenuSize();
    }

    void OnValidate()
    {
        updateMenuSize();
    }

    void updateMenuSize()
    {
        transform.localScale = Vector3.one * m_menuWidth * m_scalePerMeter;
    }

}
