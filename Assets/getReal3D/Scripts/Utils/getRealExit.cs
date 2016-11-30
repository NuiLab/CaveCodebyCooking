using UnityEngine;
using System.Collections;

public class getRealExit : MonoBehaviour {
    
    public KeyCode m_exitKey = KeyCode.Escape;

    void Update()
    {
        if(UnityEngine.Input.GetKeyDown(m_exitKey)) {
#if         UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#           else
            getReal3D.Plugin.clusterShutdown();
#           endif
        }
    }
}
