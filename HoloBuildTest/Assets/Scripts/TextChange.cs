using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChange : MonoBehaviour {

    public Text PeopleCountText;
    public string textValue;

	// Use this for initialization
	void Start () {
        PeopleCountText.text = "Hello, World!";
	}
	
	// Update is called once per frame
	void Update () {
        PeopleCountText.text = textValue;
	}
}
