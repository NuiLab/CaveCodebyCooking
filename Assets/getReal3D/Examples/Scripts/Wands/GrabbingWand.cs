using UnityEngine;

using getReal3D;

public class GrabbingWand
 : MonoBehaviour
{
    public string button = "WandButton";
    private GameObject grabObject = null;
    public LayerMask grabLayerMask = -1;
    public bool allowGrabSteal = false;

    void OnDisable()
    {
        DropObject();
    }

    void DropObject()
    {
        if (grabObject != null)
        {
            // If the object has the GrabbedObject behavior, tell it to drop
            GrabbedObject grabbedObject = grabObject.GetComponent<GrabbedObject>();
            if (grabbedObject)
                grabbedObject.dropObject(transform.parent);

            grabObject = null;
        }
    }

    void Update()
    {
        Debug.DrawRay(transform.parent.position, transform.parent.forward * 2f, Color.yellow);

        // If the wand button is released, drop the object
        if (getReal3D.Input.GetButtonUp(button))
        {
            DropObject();
        }
        // If the wand button was pressed and we're not already grabbing something, test for objects to grab
        else if (grabObject == null && getReal3D.Input.GetButtonDown(button))
        {
            // Raycast test for objects to grab
            RaycastHit hit = new RaycastHit();
            bool hitTest = Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, 2.0f, grabLayerMask);
            if (hitTest)
            {
                Rigidbody rb = hit.rigidbody;
                Transform tf = hit.transform.parent;
                while (rb == null && tf.parent != null)
                {
                    tf = tf.parent;
                    rb = tf.GetComponent<Rigidbody>();
                }

                // If the object doesn't have a rigidbody, don't do anything
                if (!rb)
                    return;

                grabObject = rb.gameObject;

                // Add the GrabbedObject behavior(script) to the object if it hasn't already been grabbed by someone else
                GrabbedObject grabbedObject = grabObject.GetComponent<GrabbedObject>();
                if (!grabbedObject)
                {
                    grabbedObject = grabObject.AddComponent<GrabbedObject>();
                }

                // Grab the object
                grabbedObject.grabObject(transform.parent, allowGrabSteal);
            }
        }
    }
}
