using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class stove : MonoBehaviour
{

    public List<Text> functions = new List<Text>(); //holds the text fields
    public List<stoveObject> stoveObjects = new List<stoveObject>(); //holds stoveObjects
    private int i, k;

    // Use this for initialization
    void Awake()
    {
        //find all text fields tagged text_slot
        foreach (GameObject textfield in GameObject.FindGameObjectsWithTag("Text_Slot"))
        {
            functions.Add(textfield.GetComponent<Text>());
        }

        //find all stoveObjects
        foreach (GameObject stoveObj in GameObject.FindGameObjectsWithTag("StoveObject"))
        {
            stoveObjects.Add(stoveObj.GetComponent<stoveObject>());
        }
    }
    void Start()
    {
        //sort the array of text fields
        functions.Sort((x, y) => string.Compare(x.name, y.name));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        //Once a collision is detected, if the other object has component "isColliding" and "isColliding" is false, 
        //find the first empty position in the array of texts and use that to display the code of other
        if (other.gameObject.tag == "StoveObject")
            if (!other.gameObject.GetComponent<stoveObject>().isColliding)
            {
                int c = assignText(other.gameObject.GetComponent<stoveObject>());
                if ( c >= 0)
                {
                    activateButton(c, other.gameObject.GetComponent<stoveObject>().objectToCook);
                }
            }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "StoveObject")
            if (other.gameObject.GetComponent<stoveObject>().needsUpdate)
                updateTextField(other.gameObject.GetComponent<stoveObject>());
            
    }

    void OnTriggerExit(Collider other)
    {
        //if a colliding object is removed, find the text that corresponds to this object and delete the text
        if (other.gameObject.tag == "StoveObject")
            if (other.gameObject.GetComponent<stoveObject>().isColliding)
                deactivateButton(removeText(other.gameObject.GetComponent<stoveObject>()));
    }

    int assignText(stoveObject obj)
    {
        if (!obj.isColliding && !functions.Find(x => x.text.StartsWith(obj.code.Substring(0, 3))))
        {
            k = functions.FindIndex(x => x.text == "");
            i = stoveObjects.IndexOf(obj);
            obj.isColliding = true;
            functions[k].text = stoveObjects[i].code;
            return k;
        }
        else
            return -1;
    }

    void updateTextField(stoveObject obj)
    {
        obj.needsUpdate = false;
        k = functions.FindIndex(x => x.text.StartsWith(obj.code.Substring(0, 3)));
        functions[k].text = obj.code;
        if (obj.objectToCook != null)
            functions[k].GetComponentInChildren<Button>().onClick.AddListener(obj.objectToCook.cook);
        if (obj.objectToCook == null)
            functions[k].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
    }

    int removeText(stoveObject obj)
    {
        k = functions.FindIndex(x => x.text == obj.code);
        obj.isColliding = false;
        functions[k].text = "";
        return k;
    }

    void activateButton(int j, foodItem food)
    {
        foreach (Transform child in functions[j].transform)
        {
            //activate child button
            child.GetComponent<Button>().gameObject.SetActive(true);
            if (food)
                child.GetComponent<Button>().onClick.AddListener(food.cook);
        }
    }

    void deactivateButton(int j)
    {
        foreach (Transform child in functions[j].transform)
        {
            //deactivate child button
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            child.gameObject.SetActive(false);
        }
    }

}
    
