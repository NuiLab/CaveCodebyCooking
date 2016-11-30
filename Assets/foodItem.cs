using UnityEngine;

public class foodItem : MonoBehaviour {

    public stoveObject whereIn;
    // Use this for initialization
    void Start()
    {
        whereIn = null;
    }

    // Update is called once per frame
    void Update() { 
    }


    public void cook()
    {
        if (name.ToString() == "Cheese" && whereIn.name == "Pan")
        {
            Material cookedCheese = Resources.Load("cookedCheese02BMat", typeof(Material)) as Material;
            GetComponent<Renderer>().material = cookedCheese;
        }

        if (name.ToString() == "Chicken" && whereIn.name == "Pan")
        {
            Material cookedChicken = Resources.Load("cooked_chicken_diffuse", typeof(Material)) as Material;
            GetComponent<Renderer>().material = cookedChicken;
        }

        if (name.ToString() == "Fried egg" && whereIn.name == "Pan")
        {
            Material cookedWhite = Resources.Load("cooked fried egg", typeof(Material)) as Material;
            Material cookedYolk = Resources.Load("cooked yolk", typeof(Material)) as Material;
            transform.FindChild("white").GetComponent<Renderer>().material = cookedWhite;
            transform.FindChild("yolk").GetComponent<Renderer>().material = cookedYolk;
        }

        if (name.ToString() == "Fries" && whereIn.name == "Pan")
        {
            Material cookedFries = Resources.Load("cookedFries", typeof(Material)) as Material;
            GetComponent<Renderer>().material = cookedFries;
        }

        if (name.ToString() == "Eggplant" && whereIn.name == "Pot")
        {
            gameObject.SetActive(false);
            whereIn.transform.FindChild("boiling water").gameObject.SetActive(false);
            whereIn.transform.FindChild("eggplant soup").gameObject.SetActive(true);
        }

        if (name.ToString() == "tomato" && whereIn.name == "Pot")
        {
            gameObject.SetActive(false);
            whereIn.transform.FindChild("boiling water").gameObject.SetActive(false);
            whereIn.transform.FindChild("tomato soup").gameObject.SetActive(true);
        }
    }
}
