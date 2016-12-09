using UnityEngine;

public class GrabbedObject : MonoBehaviour {

    private Transform m_originalParent = null;
    private Transform m_grabParent = null;
    private bool m_wasKinematic = false;
    private bool m_allowGrabSteal = false;

    private Vector3 m_lastPosition = Vector3.zero;
    private Vector3 m_velocity = Vector3.zero;
    private Vector3 m_velocitySmoothed = Vector3.zero;
    private Vector3 m_angularVelocity = Vector3.zero;
    private Quaternion m_lastOrientation = Quaternion.identity;
    private Vector3 m_angularVelocitySmoothed = Vector3.zero;

    /// <summary>
    /// Signal that this object is to be grabbed by a new parent (the grabbing user's wand)
    /// </summary>
    /// <param name="newParent">The new parent. Usually the grabbing user's wand.</param>
    /// <param name="allowGrabSteal">Whether or not to allow other user's to grab this object and take if from the original grabber.</param>
    public void grabObject(Transform newParent, bool allowGrabSteal)
    {
        // Don't allow the grab if the object is already grabbed by someone else and grab steal is disallowed
        if (m_grabParent != null && !m_allowGrabSteal)
            return;

        // Set initial values if this is the first user to grab the object
        if (m_grabParent == null)
        {
            m_originalParent = transform.parent;
            m_allowGrabSteal = allowGrabSteal;
        }

        // Set the starting position, orientation, and velocity
        m_lastPosition = transform.position;
        m_lastOrientation = transform.rotation;
        m_velocitySmoothed = m_angularVelocitySmoothed = Vector3.zero;

        // Reparent the grabbed object
        m_grabParent = newParent;
        transform.parent = newParent;

        // Set kinematic to true so physics don't apply to it while grabbed
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Signal that this object is to be dropped
    /// </summary>
    /// <param name="parent">The dropping parent. Used to determine if the dropping user is the current parent of the object.</param>
    public void dropObject(Transform parent)
    {
        // Don't do anything if a different user has grabbed this object
        if (m_grabParent != parent)
            return;

        // Restore the original parent
        transform.parent = m_originalParent;

        // Re-enable physics on the object and initialize its velocity
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody)
        {
            rigidbody.isKinematic = m_wasKinematic;
            if (!m_wasKinematic)
            {
                rigidbody.velocity = m_velocitySmoothed;
                rigidbody.angularVelocity = -m_angularVelocitySmoothed;
            }
        }

        // Remove this behavior(script) from the object
        Destroy(this);
    }

    private Vector3 CalculateAngularVelocity(Quaternion prev, Quaternion current)
    {
        Quaternion deltaRotation = Quaternion.Inverse(prev) * current;
        float angle = 0.0f;
        Vector3 axis = Vector3.zero;
        deltaRotation.ToAngleAxis(out angle, out axis);
        if (axis == Vector3.zero || axis.x == Mathf.Infinity || axis.x == Mathf.NegativeInfinity)
            axis = Vector3.zero;
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return axis.normalized * angle / Time.fixedDeltaTime;
    }

    private void trackVelocity()
    {
        m_velocity = (transform.position - m_lastPosition) / Time.fixedDeltaTime;
        m_velocitySmoothed = Vector3.Lerp(m_velocitySmoothed, m_velocity, Time.fixedDeltaTime * 10);

        m_angularVelocity = CalculateAngularVelocity(m_lastOrientation, transform.rotation);
        m_angularVelocitySmoothed = Vector3.Lerp(m_angularVelocitySmoothed, m_angularVelocity, Time.fixedDeltaTime * 10);

        m_lastPosition = transform.position;
        m_lastOrientation = transform.rotation;
    }

    void FixedUpdate()
    {
        trackVelocity();
    }
}
