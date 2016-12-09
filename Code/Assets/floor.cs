using UnityEngine;
using System.Collections;

public class floor : MonoBehaviour
{
    public platform plat = new platform();
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlatformObject"))
        {
            for (int i = 0; i < 2; i++)
            {
                if (plat.array[i].Contains(other.gameObject.name))
                    plat.array[i] = "";
            }
        }
    }

}
