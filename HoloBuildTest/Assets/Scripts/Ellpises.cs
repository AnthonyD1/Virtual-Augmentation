using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ellpises : MonoBehaviour {

    public Text Analyzing;
    public Text Name;
	// Use this for initialization
	void Start ()
    {
        /* Setting Text*/
        Analyzing.text = "Analyzing   ";
        /*Starts the invoke repeating method at second 0 and recalls it every .5 seconds*/
        InvokeRepeating("Ellipses", 0, 0.5f);
        /* Used to cancel ellipses when triggered, must set a trigger before using or
         * the ellipses won't work at all. Change false to trigger if communication 
         * between hololens and server work. */
        if (false)
        {
            CancelInvoke();
            GameObject.Find("Analyzing").SetActive(false);
            NameDisplay("Bob");
        }
    }
	
	/*Checks how many periods the string has and adds another or removes all three*/
	void Ellipses () {
		if(Analyzing.text == "Analyzing   ")
        {
            Analyzing.text = "Analyzing.  ";
        }
        else if(Analyzing.text == "Analyzing.  ")
        {
            Analyzing.text = "Analyzing.. ";
        }
        else if(Analyzing.text == "Analyzing.. ")
        {
            Analyzing.text = "Analyzing...";
        }
        else if(Analyzing.text == "Analyzing...")
        {
            Analyzing.text = "Analyzing   ";
        }
    }

    /*Should take parameter name from server. Used to display name over the analyzing text
     when a person is found. */
    void NameDisplay(string name)
    {
        Name.text = name;
    }
}
