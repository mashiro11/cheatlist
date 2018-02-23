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

    public AudioClip[] sounds;
    AudioSource aSource;
    private bool busted = false;
    const int STAGE_THEME = 0;
    const int BUSTED_THEME = 1;
    const int WIN_THEME = 2;



    void Start () {
        Screen.orientation = ScreenOrientation.Landscape;
        aSource = GetComponent<AudioSource>();
        Instance = this;
        RestartLevel();
        ganhouUI.SetActive(false);
        perdeuUI.SetActive(false);
        timerPanel.SetActive(false);
        UnityEngine.Debug.Log("Alunos para acabar: " + (20 - contadorDeDedoDuro));
        UnityEngine.Debug.Log("Alunos dedo duros: " + contadorDeDedoDuro);
    }

	
	// Update is called once per frame
	void Update () {
        timerPanel.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        UnityEngine.Debug.Log("Canvas SortingLayer: " + timerPanel.GetComponent<Canvas>().sortingLayerName);
        UnityEngine.Debug.Log("Professor SortingLayer: " + GameObject.FindGameObjectWithTag("Professor").GetComponentInChildren<SpriteRenderer>().sortingLayerName);
        UnityEngine.Debug.Log("Canvas RendererOrder: " + timerPanel.GetComponent<Canvas>().sortingLayerID);
        UnityEngine.Debug.Log("Professor OrderinLayer: " + GameObject.FindGameObjectWithTag("Professor").GetComponentInChildren<SpriteRenderer>().sortingLayerID);

        timer -= Time.deltaTime;
        timerText.text = Mathf.Round(timer).ToString();
        if (timer <=0 || (gameOver && !aSource.isPlaying) )
        {
            perdeuUI.SetActive(true);
            Time.timeScale = 0f;
            //GameOver();
        }

        if (playerWins)
        {
            PlayMusic(WIN_THEME, false);
            ganhouUI.SetActive(true);
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
        UnityEngine.Debug.Log(Time.timeScale);
    }

    void RestartLevel()
    {
       
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
}
