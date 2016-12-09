using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using getReal3D;

public class WandManager
	: MonoBehaviour
{
	public List<GameObject> m_managedObjects;
	
	private int m_activeIndex = 0;
	public string changeWandButton = "ChangeWand";

	// Use this for initialization
	void Start ()
	{
        foreach (GameObject go in m_managedObjects) {
            getReal3D.Helper.SetActiveRecursively(go, false);
        }
        if (m_managedObjects.Count > 0) {
            getReal3D.Helper.SetActiveRecursively(m_managedObjects[m_activeIndex], true);
        }
	}
	
	// Update is called once per frame
	void Update ()
	{
        if(getReal3D.Input.GetButtonDown(changeWandButton)) {
            ChangeWand();
        }
	}

	void ChangeWand() {
        getReal3D.Helper.SetActiveRecursively(m_managedObjects[m_activeIndex], false);
        m_activeIndex = (m_activeIndex + 1) % m_managedObjects.Count;
        getReal3D.Helper.SetActiveRecursively(m_managedObjects[m_activeIndex], true);
	}

}
