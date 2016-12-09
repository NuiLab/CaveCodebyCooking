using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class platformObj : MonoBehaviour
{
    public string food;
    private string commaORempty;
    void Start()
    {
        food = "";
    }
    void OnTriggerEnter(Collider other)
    {
        if (food.Trim() == "")
            commaORempty = "";
        else
            commaORempty = ",";
        if ((this.gameObject.name.Equals("Blend") && other.gameObject.CompareTag("Fruit")) || (this.gameObject.name.Equals("Chop") && other.gameObject.CompareTag("Unchopped")))
            food += (commaORempty+" "+other.gameObject.name+" ");
    }

    void OnTriggerExit(Collider other)
    {
        if ((this.gameObject.name.Equals("Blend") && other.gameObject.CompareTag("Fruit")) || (this.gameObject.name.Equals("Chop") && other.gameObject.CompareTag("Unchopped")))
        {
            if(food.Trim().StartsWith(other.gameObject.name.Substring(0)))
                food = food.Replace(" " + other.gameObject.name + " ", "");
            else
                food = food.Replace(", " + other.gameObject.name + " ", "");
            if (food.StartsWith(","))
                food = food.Substring(1, food.Length - 1);

        }
    }
}