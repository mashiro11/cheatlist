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
    private bool startedWinMusic = false;
    enum Songs
    {
        STAGE_THEME = 0,
        BUSTED_THEME = 1,
        WIN_THEME = 2
    }

    enum GameState
    {
        RUNNING,
        PLAYER_WINS,
        PLAYER_LOSES
    }
    static GameState gameState = GameState.RUNNING;
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
        if (Input.GetKeyDown(KeyCode.W))
            gameState = GameState.PLAYER_LOSES;

        if (Input.GetKeyDown(KeyCode.Q))
            gameState = GameState.PLAYER_WINS;

        switch (gameState)
        {
            case GameState.RUNNING:
                timer -= Time.deltaTime;
                timerText.text = Mathf.Round(timer).ToString();
                if (timer <= 0)
                {
                    AlunoController.StopControls(true);
                    if (AlunoController.GetMean() > 5)
                    {
                        gameState = GameState.PLAYER_WINS;
                        playerWins = true;
                    }
                    else
                    {
                        gameState = GameState.PLAYER_LOSES;
                        gameOver = true;
                    }

                }
                break;
            case GameState.PLAYER_LOSES:
                ProfessorIA.ai_type = ProfessorIA.AI_TYPE.NONE;
                perdeuUI.SetActive(true);
                float mean = AlunoController.GetMean();
                perdeuUI.GetComponentInChildren<Text>().text = mean.ToString(mean > 4 ? "0.00" : "0.0");
                //Time.timeScale = 0f;
                break;
            case GameState.PLAYER_WINS:
                ProfessorIA.ai_type = ProfessorIA.AI_TYPE.NONE;
                if (!startedWinMusic)
                {
                    startedWinMusic = true;
                    PlayMusic((int)Songs.WIN_THEME, false);
                    AudioSource aS = gameObject.AddComponent<AudioSource>();
                    aS.clip = sounds[((int)Songs.WIN_THEME + 1)];
                    aS.Play();
                    aS = gameObject.AddComponent<AudioSource>();
                    aS.clip = sounds[((int)Songs.WIN_THEME + 2)];
                    aS.Play();
                }

                ganhouUI.SetActive(true);
                ganhouUI.GetComponentInChildren<Text>().text = AlunoController.GetMean().ToString("0.0");
                //Time.timeScale = 0f;
                break;
        }
        
        if (busted && !aSource.isPlaying)
        {
            busted = false;
            PlayMusic((int)Songs.STAGE_THEME, true);
        }
    }

    public static void GameOver()
    {
        UnityEngine.Debug.Log((new StackFrame(1)).GetMethod().Name);
        gameState = GameState.PLAYER_LOSES;
    }

    IEnumerator WaitToLoad ()
    {
        yield return new WaitForSeconds(3f);
    }

    public static void GanhouJogo()
    {
        gameState = GameState.PLAYER_WINS;
    }

    public void PlayAgain ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ProfessorIA.ai_type = ProfessorIA.AI_TYPE.AI_1;
        AlunoController.ResetParameters();
        Time.timeScale = 1f;
        //AlunoController.busted = false;
        timer = 260f;
        contadorDeDedoDuro = 0;
        ganhouUI.SetActive(false);
        perdeuUI.SetActive(false);
        gameState = GameState.RUNNING;
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
        PlayMusic((int)Songs.BUSTED_THEME, false);
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
        if (mean > 5) gameState = GameState.PLAYER_WINS;
        else gameState = GameState.PLAYER_LOSES;
    }
}
