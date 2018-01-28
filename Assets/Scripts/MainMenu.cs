using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject creditPanel;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1f;
	}

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame ()
    {
        SceneManager.LoadScene("Main");
    }

    public void CallCredits ()
    {
        creditPanel.SetActive(true);

    }

    public void EndCredits()
    {
        creditPanel.SetActive(false);

    }

}
