using UnityEngine;
using System.Collections;
using getReal3D;

[AddComponentMenu("getReal3D/Navigation/Wand Look")]

public class getRealWandLook
	: MonoBehaviour
{
	public enum RotationAxes { WandX = 0, WandY, WandXY, WandZ, WandXZ, WandYZ, WandXYZ}
	public RotationAxes axes = RotationAxes.WandXY;
	public string activationButton = "WandLook";
	public string resetButton = "Reset";
	public bool ContinuousDrive = false;
	public float WandLookDeadZone = 5f;
	public float RotationSpeed = 30.0f;
	
	private Quaternion m_initialWand = new Quaternion();
	private Quaternion m_initialRotation = new Quaternion();
	private bool m_active = false;

	private Transform m_transform;
	public enum RotationAround { Wand, Head, Reference };
    public RotationAround rotationAround = RotationAround.Head;
    public Transform rotationReference = null;
	private CharacterController controller = null;

	void Awake()
	{
        m_transform = transform;
		if (controller == null)
			controller = GetComponent<CharacterController>();
	}

	// Use this for initialization
	void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().freezeRotation = true;
    }
	
	// Update is called once per frame
	void NavigationUpdate()
	{
		if (getReal3D.Input.GetButton(activationButton))
		{
			if (!m_active)
			{
				Debug.Log ("Activate!");
				m_active = true;
				m_initialWand = getReal3D.Input.wand.rotation;
				m_initialRotation = m_transform.rotation;
			}
            else
			{
				UpdateRotation(m_initialWand, getReal3D.Input.wand.rotation);
			}
		}
		else if (m_active) {
			Debug.Log ("Deactivate!");
			m_active = false;
			m_initialWand = getReal3D.Input.wand.rotation;
			m_initialRotation = m_transform.rotation;
		}
	}

	void Update()
	{
		if (getReal3D.Input.GetButtonDown(resetButton))
		{
			doReset();
		}
		NavigationUpdate();
	}

	private void doReset()
	{
		Vector3 up = -Physics.gravity;
		up = (up.sqrMagnitude == 0.0f) ? Vector3.up : up.normalized;
		up.Scale(m_transform.eulerAngles);
		m_transform.rotation = Quaternion.Euler(up);
	}

	void UpdateRotation (Quaternion initialWand, Quaternion currentWand)
    {
		Quaternion diffOrn = Quaternion.Inverse(initialWand) * currentWand;
		diffOrn = getReal3D.Input.wand.rotation * diffOrn * Quaternion.Inverse(getReal3D.Input.wand.rotation);
		diffOrn = getReal3D.Input.head.rotation * diffOrn * Quaternion.Inverse(getReal3D.Input.head.rotation);
		float angle;
		Vector3 axis;
		diffOrn.ToAngleAxis(out angle, out axis);
		float sign = angle < 0 ? -1f : 1f;
		angle = Mathf.Abs(angle);
		getReal3D.Input.NavOptions.GetValue<float>("WandLookDeadZone", ref WandLookDeadZone);
		if (angle < WandLookDeadZone) return;
		if (ContinuousDrive) {
			getReal3D.Input.NavOptions.GetValue<float>("RotationSpeed", ref RotationSpeed);
			angle = Mathf.Clamp01((angle-WandLookDeadZone)/(RotationSpeed-WandLookDeadZone)) * RotationSpeed;
			diffOrn = Quaternion.AngleAxis(sign * angle, axis);
		}
		switch(axes)
		{
		case RotationAxes.WandX:   diffOrn = Quaternion.Euler(new Vector3(diffOrn.eulerAngles.x, 0, 0)); break;
		case RotationAxes.WandY:   diffOrn = Quaternion.Euler(new Vector3(0, diffOrn.eulerAngles.y, 0)); break;
		case RotationAxes.WandXY:  diffOrn = Quaternion.Euler(new Vector3(diffOrn.eulerAngles.x, diffOrn.eulerAngles.y, 0)); break;
		case RotationAxes.WandZ:   diffOrn = Quaternion.Euler(new Vector3(0, 0, diffOrn.eulerAngles.z)); break;
		case RotationAxes.WandXZ:  diffOrn = Quaternion.Euler(new Vector3(diffOrn.eulerAngles.x, 0, diffOrn.eulerAngles.z)); break;
		case RotationAxes.WandYZ:  diffOrn = Quaternion.Euler(new Vector3(0, diffOrn.eulerAngles.y, diffOrn.eulerAngles.z)); break;
		case RotationAxes.WandXYZ: break;
		}
		Vector3 up = m_transform.up;
		Vector3 forward = getReal3D.Input.head.rotation * Vector3.forward;
		Vector3 right = Vector3.Cross(forward, up);
		forward = Vector3.Cross(right, up);
		Quaternion frame = Quaternion.LookRotation(forward, up);
		diffOrn = Quaternion.Inverse(frame) * diffOrn * frame;
		Vector3 about = m_transform.position;
		switch(rotationAround)
		{
		case RotationAround.Head: about = m_transform.localToWorldMatrix.MultiplyPoint3x4(getReal3D.Input.head.position); break;
		case RotationAround.Wand: about = m_transform.localToWorldMatrix.MultiplyPoint3x4(getReal3D.Input.wand.position); break;
		case RotationAround.Reference: if (rotationReference != null) about = rotationReference.position; break;
		}
		about = m_transform.worldToLocalMatrix * (about - m_transform.position);
		if (controller == null || !controller.enabled)
			m_transform.Translate(about, Space.Self);
		if (ContinuousDrive) {
			m_transform.rotation = Quaternion.Slerp(m_initialRotation, m_initialRotation * diffOrn, Time.smoothDeltaTime);
			m_initialRotation = m_transform.rotation;
		}
		else {
			m_transform.rotation = m_initialRotation * diffOrn;
		}
		if (controller == null || !controller.enabled)
			m_transform.Translate(-about, Space.Self);
	}
}
