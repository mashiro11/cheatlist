using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public GameObject ganhouUI;
    public GameObject perdeuUI;
    public GameObject timerPanel;
    
    public Text timerText;
    public float timer;
    public static int contadorDeDedoDuro;
    private static bool gameOver = false;
    private static bool playerWins = false;
    private static float spamTimer = 4f;
    public AudioClip[] sounds;
    AudioSource aSource;
    private bool busted = false;
    const int STAGE_THEME = 0;
    const int BUSTED_THEME = 1;
    const int WIN_THEME = 2;
    //CameraRecord videoCapture;



    void Start () {
        Screen.orientation = ScreenOrientation.Landscape;
        //timerPanel.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        AlunoController.SpawnAlunos();
        aSource = GetComponent<AudioSource>();
        Instance = this;
        ganhouUI.SetActive(false);
        perdeuUI.SetActive(false);
        timerPanel.SetActive(false);
        UnityEngine.Debug.Log("Alunos para acabar: " + (20 - contadorDeDedoDuro));
        UnityEngine.Debug.Log("Alunos dedo duros: " + contadorDeDedoDuro);
    }

	
	// Update is called once per frame
	void Update () {
        
        timer -= Time.deltaTime;
        timerText.text = Mathf.Round(timer).ToString();
        if(timer <= 0)
        {
            AlunoController.StopControls(true);
            if (AlunoController.GetMean() > 5)
            {
                playerWins = true;
            }
            else
            {
                gameOver = true;
            }

        }

        if (gameOver)
        {
            if (timer > 0)
            {
                if (spamTimer < 0)
                {
                    perdeuUI.SetActive(true);
                    float mean = AlunoController.GetMean();
                    perdeuUI.GetComponentInChildren<Text>().text = mean.ToString(mean > 4 ? "0.00" : "0.0");
                    Time.timeScale = 0f;
                }
                else
                {
                    spamTimer -= Time.deltaTime;
                }
            }
            else
            {
                perdeuUI.SetActive(true);
                float mean = AlunoController.GetMean();
                perdeuUI.GetComponentInChildren<Text>().text = mean.ToString(mean > 4 ? "0.00" : "0.0");
                Time.timeScale = 0f;
            }
        }

        if (playerWins)
        {
            PlayMusic(WIN_THEME, false);
            ganhouUI.SetActive(true);
            ganhouUI.GetComponentInChildren<Text>().text = AlunoController.GetMean().ToString("0.0");
            Time.timeScale = 0f;
        }

        if (busted && !aSource.isPlaying)
        {
            busted = false;
            PlayMusic(STAGE_THEME, true);
        }
    }

    public static void GameOver()
    {
        UnityEngine.Debug.Log((new StackFrame(1)).GetMethod().Name);
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
        AlunoController.ResetParameters();
        Time.timeScale = 1f;
        //AlunoController.busted = false;
        timer = 260f;
        contadorDeDedoDuro = 0;
        ganhouUI.SetActive(false);
        perdeuUI.SetActive(false);
        playerWins = false;
        gameOver = false;
        spamTimer = 4f;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        PlayAgain();
        SceneManager.LoadScene("Menu");
        UnityEngine.Debug.Log(Time.timeScale);
    }

    public void Busted()
    {
        PlayMusic(BUSTED_THEME, false);
        busted = true;
    }

    private void PlayMusic(int music, bool loop)
    {
        aSource.Stop();
        aSource.clip = sounds[music];
        aSource.Play();
        aSource.loop = loop;
    }

    public static string CallerName()
    {
        return new StackFrame(2).GetMethod().Name;
    }

    public static void FimDeJogo()
    {
        //CameraRecord.stopRecord = true;
        float mean = AlunoController.GetMean();
        if (mean > 5) playerWins = true;
        else gameOver = true;
    }
}
