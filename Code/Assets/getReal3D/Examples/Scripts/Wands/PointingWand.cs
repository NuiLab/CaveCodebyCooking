using UnityEngine;

using getReal3D;

public class PointingWand : MonoBehaviour
{
    public string button = "WandButton";
    public string pointObject;
    public LayerMask grabLayerMask = -1;

    void Update()
    {
        if (getReal3D.Input.GetButtonDown(button))
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
                pointObject = rb.gameObject.name;
                Debug.Log(pointObject + "  zzzzzzzzzzzzzz");
            }
        }
        else
            pointObject = "xyz";
    }
}