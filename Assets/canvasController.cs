using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class canvasController : MonoBehaviour {

    public Text tutorText;
	// Use this for initialization
	void Start () {
        if (tutorText == null)
        { 
            Debug.LogError("Error: tutorText não definido");
            return;
        }
        StartCoroutine(Tutor());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    IEnumerator Tutor()
    {
        var textToWrite = "Make everyone cheat within test time. Gogogogogogo!";
        foreach (var t in new MimicWriting().TypeText(textToWrite, 2f))
        {
            tutorText.text = t;
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        tutorText.text = "";
    }
}
