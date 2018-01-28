using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public static string NextSceneName;
    public string nextScene;
    public Text textContainer;
    public Image progressBar;

    private Vector2 max, min;

    public void LoadNextScene()
    {
        if (NextSceneName == string.Empty)
        {
            Debug.LogError("NextSceneName nao informado na scena " + SceneManager.GetActiveScene().name);
            return;
        }
        NextSceneName = nextScene;
        SceneManager.LoadScene("Loading");
    }

    void Awake()
    {
        Instance = this;
       
        if (SceneManager.GetActiveScene().name == "Loading")
        {
         
            progressBar.fillAmount = 0f;
            //textContainer.text = NextSceneName;
            //Debug.Log("NextSceneName:" + NextSceneName??"null");
            if (string.IsNullOrEmpty(NextSceneName)) //primeiro load ante menu princiapl
            {
                NextSceneName = nextScene;
               
            }
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Loading")
        { 
            StartCoroutine("LoadAsync");
        }
    }

    IEnumerator LoadAsync()
    {
        string loadingTxt;

        loadingTxt = "Loading...";

        var asy = SceneManager.LoadSceneAsync(NextSceneName);

        while (!asy.isDone)
        {
            yield return new WaitForEndOfFrame();

            if (asy.isDone)
            {
                progressBar.fillAmount = 1f;
                textContainer.text = string.Format("{0} {1}%", loadingTxt, Mathf.RoundToInt(Mathf.Clamp(asy.progress, 0, 1) * 100f).ToString());

            }
            else
            {
                progressBar.fillAmount = asy.progress;
                textContainer.text = string.Format("{0} {1}%", loadingTxt, Mathf.RoundToInt(Mathf.Clamp(asy.progress, 0, 1) * 100f).ToString());
               
            }
        }
        yield return new WaitForSeconds(1f);
    }
}
