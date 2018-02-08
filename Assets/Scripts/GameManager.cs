using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public GameObject ganhouUI;
    public GameObject perdeuUI;

    public Text timerText;
    public float timer;
    public int contadorDeAlunos;
    public static int contadorDeDedoDuro;


    // Use this for initialization
    void Start () {
        Screen.orientation = ScreenOrientation.Landscape;
        Instance = this;
        RestartLevel();
        Debug.Log("Alunos para acabar: " + (20 - contadorDeDedoDuro));
        Debug.Log("Alunos dedo duros: " + contadorDeDedoDuro);
    }

	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        timerText.text = Mathf.Round(timer).ToString();
        if (timer <=0)
        {
            GameOver();
        }

        if (contadorDeAlunos >= 20 - contadorDeDedoDuro)
        {
            GameObject.Find("Camera").GetComponent<CameraController>().WinGame();
            GanhouJogo();
        }
    }

    public void GameOver()
    {
        perdeuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    IEnumerator WaitToLoad ()
    {
        yield return new WaitForSeconds(3f);
    }

    public void GanhouJogo()
    {
        ganhouUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void PlayAgain ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        AlunoController.busted = false;
        timer = 260f;
        contadorDeAlunos = 0;
        contadorDeDedoDuro = 0;

    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        Debug.Log(Time.timeScale);
    }

    void RestartLevel()
    {
       
    }
}
