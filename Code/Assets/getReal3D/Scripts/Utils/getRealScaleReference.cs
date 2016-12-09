using UnityEngine;
using System.Collections;

public class getRealScaleReference
	: MonoBehaviour
{
	public getReal3D.Scale.Units scaleUnits = getReal3D.Scale.Units.meters;
	public float scaleFactor = 1.0f;

	void Awake()
	{
		getReal3D.Scale.referenceUnits = scaleUnits;
		getReal3D.Scale.referenceScale = scaleFactor;
	}
	
	void LateUpdate ()
	{
		if (!Mathf.Approximately(gameObject.transform.localScale.x, getReal3D.Scale.worldScale)) {
			gameObject.transform.localScale = Vector3.one * getReal3D.Scale.worldScale;
		}
	}
}