using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Helper functions to make changes to HUD text easy
 */
public class TextChange : MonoBehaviour {
    public Text MyText;

	void Start () {
        MyText.text = "Hello, World!";
	}

    public void SetTextValue(string NewTextValue) {
        MyText.text = NewTextValue;
    }
}
