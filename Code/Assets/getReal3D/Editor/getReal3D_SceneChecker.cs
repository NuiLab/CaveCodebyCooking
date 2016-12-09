using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace getReal3D
{
	public class SceneChecker
		: EditorWindow
	{
		static bool needsUpdate = true;
		static bool foundCameraUpdater = false;
		static bool foundNavigationScript = false;

        static bool ArrayEmpty(object[] array)
		{
			return array == null || array.Length == 0;
		}
	
		[MenuItem("getReal3D/Scene Checker", false, 50)]
		static void CreateChecker()
		{
			UpdateSceneStatus();
			EditorWindow.GetWindow(typeof(SceneChecker), false, "Scene Checker");
		}
		
		static void UpdateSceneStatus()
		{
			getRealCameraUpdater[] cu = Resources.FindObjectsOfTypeAll(typeof(getRealCameraUpdater)) as getRealCameraUpdater[];
			getRealAimAndGoController[] ag = Resources.FindObjectsOfTypeAll(typeof(getRealAimAndGoController)) as getRealAimAndGoController[];
			getRealWalkthruController[] wt = Resources.FindObjectsOfTypeAll(typeof(getRealWalkthruController)) as getRealWalkthruController[];
			getRealWandLook[] wl = Resources.FindObjectsOfTypeAll(typeof(getRealWandLook)) as getRealWandLook[];
			getRealJoyLook[] jl = Resources.FindObjectsOfTypeAll(typeof(getRealJoyLook)) as getRealJoyLook[];
			
			
			foundCameraUpdater = !ArrayEmpty(cu);
			foundNavigationScript = !(ArrayEmpty(ag) && ArrayEmpty(wt) && ArrayEmpty(wl) && ArrayEmpty(jl));
			
			needsUpdate = false;
		}
		
		void OnGUI()
		{
			if (needsUpdate) UpdateSceneStatus();
			if (!foundCameraUpdater)
				EditorGUILayout.HelpBox("No getRealCameraUpdater script found. You probably want a getRealCameraUpdater attached to a GameObject with a Camera.", MessageType.Warning, true);
			else
				EditorGUILayout.HelpBox("Found getRealCameraUpdater.", MessageType.Info, true);
	
			if (!foundNavigationScript)
				EditorGUILayout.HelpBox("No getReal3D navigation scripts found. You probably want a navigation script (getRealAimAndGoController, getRealWalkthruController, getRealWandLook, getRealJoyLook) attached to a GameObject.", MessageType.None, true);
			else
				EditorGUILayout.HelpBox("Found getReal3D navigation scripts.", MessageType.Info, true);
	
			EditorGUILayout.HelpBox("Prefer getReal3D.Input over UnityEngine.Input.", MessageType.Info, true);
			
			if (GUILayout.Button("Update"))
			{
				UpdateSceneStatus();
			}
		}
	}
}
