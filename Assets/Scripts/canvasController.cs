using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class canvasController : MonoBehaviour {

    public Text tutorText;
    public GameObject tutorialUI;
    public GameObject timerPanel;
    public AudioSource aSource;
    private void Awake()
    {
        Time.timeScale = 0f;
        aSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        Time.timeScale = 0f;
    }

    public void LeaveTutorial()
    {
        DataCollector.StartDataCollection();
        GameManager.aSource.Play();
        Time.timeScale = 1f;
        tutorialUI.SetActive(false);
        timerPanel.SetActive(true);
    }

    // Use this for initialization
    /*void Start () {
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
        var textToWrite = "The objective is to pass the cheat sheet to all the students before the time runs out. All of them must  spend enough time to finish copying the answers. Meanwhile, avoid the sight of the teacher.";
        foreach (var t in new MimicWriting().TypeText(textToWrite, 5f))
        {
            tutorText.text = t;
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        tutorText.text = "";
    }*/
}
