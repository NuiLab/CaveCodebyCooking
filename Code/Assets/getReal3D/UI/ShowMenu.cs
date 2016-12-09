using UnityEngine;
using System.Collections;

public class ShowMenu : MonoBehaviour {

    public Transform m_menuObject;
    public string m_wandButton = "WandButton";
    public float m_defaultDistance = 1.0f;

    void Start()
    {
        MenuSettings ms = GetComponentInParent<MenuSettings>() as MenuSettings;
        if(ms.depth.HasValue) {
            m_defaultDistance = ms.depth.Value;
        }
        if(ms.menuButton != null) {
            m_wandButton = ms.menuButton;
        }
    }

    void Update () {
        if(getReal3D.Input.GetButtonDown(m_wandButton) && !m_menuObject.gameObject.activeSelf) {
            m_menuObject.gameObject.SetActive(true);
            setDefaultPosition();
        }
    }

    public void clickOutside()
    {
        m_menuObject.gameObject.SetActive(false);
    }

    private void setDefaultPosition()
    {
        Transform hand = transform.parent;
        m_menuObject.transform.position = hand.position + m_defaultDistance * (hand.rotation * Vector3.forward);
        Vector3 euler = new Vector3(0,hand.rotation.eulerAngles[1],0);
        m_menuObject.transform.rotation = Quaternion.Euler(euler);
    }
}
