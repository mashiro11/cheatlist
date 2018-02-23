using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlunoSpawner : MonoBehaviour {

    public static AlunoSpawner instance;

    public GameObject aluno;
    public  float initialX, initialY;
    private bool temCola;
    public float espacamentoX, espacamentoY;
    public int maxLinhas = 4;
    public int maxColunas = 5;
    public Vector2 inicial = new Vector2(1, 3);
    private static GameObject[,] alunos = null;
    private static int bustedCounter;
    private static int finishedCounter;
    public static int chances;
    public static int minToWin;
    private static string debugTag = "[AlunoSpawner]: ";
    
    //public ProfessorIA professorIA;
    private void Awake()
    {
        chances = 5;
        minToWin = 8;
        alunos = new GameObject[maxLinhas,maxColunas];
        bustedCounter = 0;
        finishedCounter = 0;
        instance = this;
    }

    // Use this for initialization
    void Start () {
        temCola = false;
        GameObject al;
        Instantiate(Resources.Load("Cola"), transform.position, Quaternion.identity );
        Cola.SetPosition(transform.position);

        for(int i = 0; i < maxLinhas; i++){
            for (int j = 0; j < maxColunas; j++)
            {
                Vector2 position = new Vector2(initialX + j * espacamentoX , initialY + i * espacamentoY);
                al = ((GameObject)Instantiate(aluno, position, Quaternion.identity));
                alunos[i, j] = al;
                
                al.GetComponent<AlunoController>().position = new Vector2(i, j);
                al.GetComponent<SpriteRenderer>().sortingLayerName = "Fileira" + i;
                //Debug.Log(i + ", " + j);
                if (Random.Range (1, 11) <= 1)
                {
                    al.GetComponent<AlunoController>().dedoDuro = true;
                    GameManager.contadorDeDedoDuro++;
                }

                if (i == inicial.x && j == inicial.y)
                {
                    al.GetComponent<AlunoController>().RecebeCola();
                    Cola.SetShooter( al.GetComponent<AlunoController>() );
                    
                    if (al.GetComponent<AlunoController>().dedoDuro)
                    {
                        al.GetComponent<AlunoController>().dedoDuro = false;
                        GameManager.contadorDeDedoDuro--;
                    }
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static AlunoController GetAluno(int i, int j)
    {
        return alunos[i,j].GetComponent<AlunoController>();
    }
    public static void OneMoreBusted()
    {
        Debug.Log("Busted: " + bustedCounter + "| Chances: " + chances);
        if (++bustedCounter > chances)
        {
            GameManager.GameOver();
        }

    }
    public static void OneMoreFinished()
    {
        if (++finishedCounter > minToWin)
        {
            GameManager.GanhouJogo();
        }
    }
    public void ResetParameters()
    {
        chances = 5;
        minToWin = 8;
        bustedCounter = 0;
        finishedCounter = 0;
    } 
}
