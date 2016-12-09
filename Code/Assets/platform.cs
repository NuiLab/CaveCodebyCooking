using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class platform : MonoBehaviour
{
    private const int NumberOfObjects = 2;
    private const int NumberOfChopped = 3;
    public GameObject WandObject;
    public string[] array = new string[NumberOfObjects];
    public Text[] displayarray = new Text[NumberOfObjects];
    public Text[] displayLoop = new Text[NumberOfObjects];
    public platformObj[] p = new platformObj[NumberOfObjects]; //has fixed order. array and displayarray orders based on which object enters first

    private bool rightObject = false;    //for collision exit
    private int children = 0;            
    private Vector3 grabbedObjectPosition;          
    private bool initialPosition = true;

    private int glassNumber = 0;          //juicer
    private bool blend=true;
    private bool hasCollided = false;
    private bool isPresent = false;
    private GameObject[] juice = new GameObject[4];
    public GameObject blenderJuice;

    public GameObject[] unChopped = new GameObject[NumberOfChopped];      //chopper
    public GameObject[] chopped = new GameObject[NumberOfChopped];

    public PointingWand pointer; //pointing wand

    void Update()           // checks for collision exit and updates display
    {
        children = WandObject.transform.childCount;
        if (children > 4)
        {
            rightObject = WandObject.transform.GetChild(4).gameObject.CompareTag("PlatformObject"); //grabbed object becomes the 5th child of WandManager
            if (rightObject)
            {
                if (initialPosition) //ensures that the object has been moved after grabbing
                {
                    grabbedObjectPosition = WandObject.transform.GetChild(4).gameObject.transform.position;
                    initialPosition = false;
                }
                for (int i = 0; i < NumberOfObjects; i++) //removes platformObject from array when it stops colliding with the platform
                {
                    if (array[i].Contains(WandObject.transform.GetChild(4).gameObject.name) && (WandObject.transform.GetChild(4).gameObject.transform.position != grabbedObjectPosition))
                        array[i] = "";
                }
            }
        }
        else
            initialPosition = true;

        for (int i = 0; i < NumberOfObjects; i++) //updates display with food
        {
            for (int j = 0; j < NumberOfObjects; j++)
            {
                if (p[j].food.Trim().StartsWith(","))
                    p[j].food = p[j].food.Trim().Substring(1, p[j].food.Length - 1);
                if (p[j].food.Trim().EndsWith(","))
                    p[j].food = p[j].food.Trim().Substring(0, p[j].food.Length - 2);
                if (array[i].Contains(p[j].name))
                    array[i] = p[j].name + "( " + p[j].food + " )";
            }
            displayarray[i].text = array[i] + Environment.NewLine;
        }   
    }

    void FixedUpdate() //starts animation and displays loop
    {
        if(blend && (pointer.pointObject.Contains("Blend") || p[0].food.Contains(pointer.pointObject)) && p[0].food.Trim()!="" && pointer.isActiveAndEnabled)
        {
            blend = false;
            StartCoroutine(juicerAnimation(glassNumber));
        }
        for (int i = 0; i < NumberOfChopped; i++)
        {
            if (unChopped[i] == null)
                continue;
            if ((pointer.pointObject.Contains("Chop") || pointer.pointObject.Equals(unChopped[i].name)) && p[1].food.Trim().Contains(unChopped[i].name) && pointer.isActiveAndEnabled)
                StartCoroutine(chopperAnimation(i));
        }
    }

    IEnumerator chopperAnimation(int i)
    {
        yield return new WaitForSeconds(1f);
        p[1].food = p[1].food.Replace(unChopped[i].name + " ", "");
        unChopped[i].SetActive(false);
        unChopped[i] = null;
        chopped[i].SetActive(true);
        chopped[i].transform.SetParent(null);
        yield return true;
    }

    IEnumerator juicerAnimation(int glassNumber)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < blenderJuice.transform.parent.transform.parent.transform.childCount; i++)
        {
            if (p[0].food.Contains(blenderJuice.transform.parent.transform.parent.transform.GetChild(i).gameObject.name))
                blenderJuice.transform.parent.transform.parent.transform.GetChild(i).gameObject.SetActive(false);
        }
        blenderJuice.SetActive(true);
        yield return new WaitForSeconds(2f);
        if(glassNumber>0)
            displayLoop[0].text = " for ( int i = 1 ;  i <= " + glassNumber + " ;  i++ )";
        for (int i = 0; i < NumberOfObjects; i++)
        {
            if (array[i].Contains("Blend"))
            {
                for (int j = 0; j < glassNumber; j++)
                {
                    displayLoop[1].text = "      fillGlass ( " + (j+1) + " , Juice )";
                    if (blenderJuice.transform.localScale.z > 0.05F)
                        blenderJuice.transform.localScale -= new Vector3(0.01F, 0.01F, 0.05F);
                    juice[j].SetActive(true);
                    yield return new WaitForSeconds(2f);
                }
            }
        }
        yield return true;
    }

    void OnCollisionEnter(Collision other)
    {
        isPresent = false;
        if (other.gameObject.CompareTag("PlatformObject"))
        {
            for (int i = 0; i < NumberOfObjects; i++)
            {
                if (array[i].Contains(other.gameObject.name))
                    isPresent = true;
            }
            for (int i = 0; i < NumberOfObjects; i++)
            {
                if (array[i].Equals("") && (!isPresent))
                {
                    array[i] = other.gameObject.name + "( )";
                    break;
                }
            }
        }
        if (other.gameObject.CompareTag("JuiceGlass"))
        {
            for (int i = 0; i < glassNumber; i++)      //checks if the same glass had already collided with the platform
            {
                if (juice[i].transform.parent.name == other.gameObject.name)
                    hasCollided = true;
            }
            if (!hasCollided)
            {
                juice[glassNumber] = other.gameObject.transform.GetChild(1).gameObject;
                glassNumber += 1;
            }
        }
    }
}
