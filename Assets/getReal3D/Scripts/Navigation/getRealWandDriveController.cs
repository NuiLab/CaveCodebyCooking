using UnityEngine;
using System.Collections;
using getReal3D;

[AddComponentMenu("getReal3D/Navigation/Wand Drive Controller")]
public class getRealWandDriveController
	: MonoBehaviour
{
	private CharacterMotor m_motor;
	private CharacterController m_controller;
	private Transform m_transform;
	private Vector3 m_lastGroundedPosition = Vector3.zero;
	
	public float TranslationSpeed = 1.0f;
	public float WandDriveDeadZone = 0.010f;
	public string activationButton = "WandDrive";
	public string resetButton = "Reset";
	public string jumpButton = "Jump";
	
	private Vector3 m_initialWand = Vector3.zero;

	// Use this for initialization
	void Awake()
	{
		m_transform = transform;
		m_motor = GetComponent<CharacterMotor>();
		m_controller = GetComponent<CharacterController>();
	}
	
	void Start()
	{
		if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().freezeRotation = true;
		m_lastGroundedPosition = m_transform.position;
		if (!getReal3D.Input.NavOptions.HasValue("TranslationSpeed"))
		{
			getReal3D.Input.NavOptions.SetValue<float>("TranslationSpeed", TranslationSpeed);
		}
	}
	
	void OnEnable()
	{
		if (m_controller)
		{
			m_controller.enabled = true;
		}
		if (m_motor != null)
		{
			m_motor.SetControllable(true);
			m_motor.enabled = true;
		}
	}
	
	void Update()
	{
		doNavigation(Time.smoothDeltaTime);
	}
	
	void doNavigation(float elapsed)
	{
		if (m_motor != null && m_motor.grounded)
			m_lastGroundedPosition = m_transform.position;
		
		if (getReal3D.Input.GetButtonDown(resetButton))
		{
			doReset();
		}

        if(getReal3D.Input.GetButtonDown(activationButton)) {
            Debug.Log("Fly Button Down");
            m_initialWand = getReal3D.Input.wand.position;
        }
        else if(getReal3D.Input.GetButtonUp(activationButton)) {
            Debug.Log("Fly Button Up");
            m_initialWand = Vector3.zero;
            if(m_motor != null) {
                m_motor.inputMoveDirection = Vector3.zero;
                m_motor.inputJump = false;
            }
        }
        else if(getReal3D.Input.GetButton(activationButton)) {
            Debug.Log("Fly Button Held");
            UpdateNavigation(elapsed);
        }
	}
	
	private void doReset()
	{
		m_transform.position = m_lastGroundedPosition;
		if (m_motor != null)
		{
			m_motor.inputMoveDirection = Vector3.zero;
			m_motor.inputJump = false;
		}
	}
	
	void UpdateNavigation(float elapsed)
	{
		// Get the input vector from keyboard or analog stick
		Vector3 directionVector = getReal3D.Input.wand.position - m_initialWand;
		directionVector = m_transform.TransformDirection(directionVector);
		directionVector -= Vector3.Dot(directionVector, Physics.gravity.normalized) * Physics.gravity.normalized;

		if (directionVector != Vector3.zero)
		{
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Clamp01(directionLength - WandDriveDeadZone);

			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		getReal3D.Input.NavOptions.GetValue<float>("TranslationSpeed", ref TranslationSpeed);
		directionVector *= 3f * TranslationSpeed;

		// Apply the direction to the CharacterMotor, CharacterController, or Transform, as available
		if (m_motor != null && m_motor.enabled && m_motor.canControl)
		{
			m_motor.inputMoveDirection = directionVector;
			m_motor.inputJump = getReal3D.Input.GetButtonDown(jumpButton);
		}
		else if (m_controller != null && m_controller.enabled)
		{
			CollisionFlags flags = m_controller.Move(directionVector * elapsed);
			bool grounded = (flags & CollisionFlags.CollidedBelow) != 0;
			if (grounded) m_lastGroundedPosition = m_transform.position;
		}
		else
		{
			m_transform.position += directionVector * elapsed;
		}
	}
}
