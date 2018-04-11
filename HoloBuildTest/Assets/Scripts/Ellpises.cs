using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ellpises : MonoBehaviour {

    public Text Analyzing;  //Text for the analysis ellipses
    public Text Name;       //Text for name display
	
	void Start ()
    {
        Analyzing.text = "Analyzing   "; //Initializing text

        /* Starts the invoke repeating method on the Ellipses function at second 0 in *
         * the program and calls it again every .5 seconds                            */
        InvokeRepeating("Ellipses", 0, 0.5f);
        /* Used to cancel ellipses when triggered, must set a trigger before using or *
         * the ellipses won't work at all. Change false to trigger if communication   *
         * between hololens and server work.                                          */
        if (false)
        {
            CancelInvoke();  //Stop calling the function that was invoked. (Ellipses in this case)
            GameObject.Find("Analyzing").SetActive(false);  //Turn off Analzying text.
            NameDisplay("Bob");  //Turn on name display text.
        }
    }
	
	/* Checks how many periods the string has and adds another or removes all three *
         * to create an ellipses effect.                                                */
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

    /* Takes parameter name from server. Used to display name over the analyzing text  *
     * when a person is found.                                                         */
    void NameDisplay(string name)
    {
        Name.text = name;
    }
}
