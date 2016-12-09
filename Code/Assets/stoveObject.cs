using UnityEngine;

public class stoveObject : MonoBehaviour
{

    public string code;
    private string action;
    public foodItem objectToCook;
    public bool isReadyToCook; //used to display button to start animation?
    public bool isColliding;
    public bool needsUpdate;
   
    void Awake()
    {
        isColliding = false;
        isReadyToCook = false;
        needsUpdate = true;
        objectToCook = null;
        if (name.Equals("Pan"))
            action = "Fry";
        else if (name.Equals("Pot"))
            action = "Boil";
        updateText();
    }

    void Update() { 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name.ToString() == "Egg")
        {
            other.gameObject.SetActive(false);
            foodItem f = transform.FindChild("Fried egg").GetComponent<foodItem>();
            f.gameObject.SetActive(true);
            f.transform.SetParent(null);
        }

        if (other.CompareTag("Food"))
        {
            objectToCook = other.gameObject.GetComponent<foodItem>();
            objectToCook.whereIn = this;
            isReadyToCook = true;
            needsUpdate = true;
            updateText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            objectToCook.whereIn = null;
            objectToCook = null;
            isReadyToCook = false;
            needsUpdate = true;
            updateText();
            other.transform.SetParent(null);
        }
    }

    public void updateText()
    {
        if (!objectToCook)
            code = action + " ( objectToCook ) ";
        else
            code = action + " ( " + objectToCook.name.ToString() + " ) ";
    }
}