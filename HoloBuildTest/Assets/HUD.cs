using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public Text People;

    private int PeopleCount;

	// Use this for initialization
	void Start ()
    {
        PeopleCount = 0;
        People.text = "";
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        SetPeopleText();
        IncrementPeopleCount();
	}

    void SetPeopleText()
    {
        People.text = "People in room: " + PeopleCount;
    }

    void IncrementPeopleCount()
    {
        PeopleCount++;
    }
}
