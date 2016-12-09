using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class getReal3D_Menu {
	
	[UnityEditor.Callbacks.PostProcessScene(100)]
	static public void FixPlayerSettings()
	{
		Debug.Log ("Adjusting PlayerSettings for build ...");
		PlayerSettings.captureSingleScreen = false;
		PlayerSettings.defaultIsFullScreen = false;
		PlayerSettings.defaultIsNativeResolution = false;
		PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
		PlayerSettings.forceSingleInstance = false;
		PlayerSettings.resizableWindow = false;
		PlayerSettings.runInBackground = true;
		PlayerSettings.usePlayerLog = true;
	}

	[UnityEditor.Callbacks.PostProcessScene(101)]
	[MenuItem("getReal3D/Advanced/getReal3D Script Execution Order", false, 105)]
	static public void FixExecutionOrder() {
		Debug.Log ("Fixing script execution order ...");
		getReal3D.Editor.Utils.FixScriptExecutionOrder();
	}

    public static void BuildPlayerImpl(string[] levels, string output, bool arch64 = false)
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);

        UnityEditor.BuildOptions options = BuildOptions.None;
        BuildPipeline.BuildPlayer(levels, output, arch64 ? BuildTarget.StandaloneWindows64 : BuildTarget.StandaloneWindows, options);
        EditorUserBuildSettings.SwitchActiveBuildTarget(target);
    }
}
