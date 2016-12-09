using UnityEngine;
using System.Collections;

public class PageSlider : MonoBehaviour
{
    public UnityEngine.UI.Text m_title = null;
    public float m_speed = 0.0f;
    
    private int m_currentIndex;
    private bool m_sliding = false;
    private Transform m_current = null;
    private Transform[] m_pages;

    void Start()
    {
        m_pages = getAllChildren().ToArray();
        m_currentIndex = 0;
        if(m_pages.Length != 0) {
            m_current = m_pages[0];
            m_current.gameObject.SetActive(true);
        }
        disableAllButCurrent();
        updateTitle();
    }

    void Update()
    {

    }

    public void nextPage()
    {
        if(m_sliding) {
            return;
        }
        if(m_currentIndex == m_pages.Length - 1) {
            return;
        }
        StartCoroutine(changePage(m_currentIndex + 1, 1));
    }

    public void previousPage()
    {
        if(m_sliding) {
            return;
        }
        if(m_currentIndex == 0) {
            return;
        }
        StartCoroutine(changePage(m_currentIndex - 1, -1));
    }

    private void updateTitle()
    {
        if(m_title && m_current) {
            m_title.text = m_current.name;
        }
    }

    private void disableAllButCurrent()
    {
        foreach(Transform child in m_pages) {
            if(child != m_current) {
                child.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator changePage(int nextIndex, float slideDirection)
    {
        Transform next = m_pages[nextIndex];
        m_currentIndex = nextIndex;

        m_sliding = true;
        Transform old = m_current;
        m_current = next;
        m_current.gameObject.SetActive(true);

        updateTitle();

        Vector3 oldPosition = old.transform.localPosition;
        Vector3 destPosition = m_current.transform.localPosition;

        float width = 400.0f;
        RectTransform rt = transform as RectTransform;
        if(rt) {
            width = rt.rect.width;
        }

        float startTime = Time.time;
        float endTime = startTime + m_speed;
        while(Time.time < endTime) {
            float elapsed = Time.time - startTime;
            float t = elapsed / m_speed;

            old.transform.localPosition = oldPosition + slideDirection * t * Vector3.left * width;
            m_current.transform.localPosition = destPosition - slideDirection * (1-t) * Vector3.left * width;

            yield return null;
        }
        old.gameObject.SetActive(false);
        old.transform.localPosition = oldPosition;
        m_current.transform.localPosition = destPosition;
        m_sliding = false;        
    }

    private System.Collections.Generic.List<Transform> getAllChildren()
    {
        System.Collections.Generic.List<Transform> res = new System.Collections.Generic.List<Transform>();
        foreach (Transform child in transform){
            res.Add(child);
        }
        return res;
    }
}
