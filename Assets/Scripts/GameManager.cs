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
    public static int contadorDeDedoDuro;
    private static bool gameOver = false;
    private static bool playerWins = false;


    // Use this for initialization
    void Start () {
        Screen.orientation = ScreenOrientation.Landscape;
        Instance = this;
        RestartLevel();
        ganhouUI.SetActive(false);
        perdeuUI.SetActive(false);
        Debug.Log("Alunos para acabar: " + (20 - contadorDeDedoDuro));
        Debug.Log("Alunos dedo duros: " + contadorDeDedoDuro);
    }

	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        timerText.text = Mathf.Round(timer).ToString();
        if (timer <=0 || gameOver)
        {
            perdeuUI.SetActive(true);
            Time.timeScale = 0f;
            //GameOver();
        }

        if (playerWins)
        {
            GameObject.Find("Camera").GetComponent<CameraController>().WinGame();
            ganhouUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public static void GameOver()
    {
        gameOver = true;
    }

    IEnumerator WaitToLoad ()
    {
        yield return new WaitForSeconds(3f);
    }

    public static void GanhouJogo()
    {
        playerWins = true;
    }

    public void PlayAgain ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        //AlunoController.busted = false;
        timer = 260f;
        contadorDeDedoDuro = 0;
        ganhouUI.SetActive(false);
        perdeuUI.SetActive(false);
        playerWins = false;
        gameOver = false;
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
