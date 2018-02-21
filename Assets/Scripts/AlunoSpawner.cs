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
    public GameObject cola;
    private static GameObject[,] alunos = null;
    private static int bustedCounter;
    private static int finishedCounter;
    public static int chances;
    public static int minToWin;
    
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
        cola.transform.position = this.transform.position;
        
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
                    cola.transform.position = al.transform.position;
                    cola.GetComponent<Cola>().shooter = al.GetComponent<AlunoController>();
                    
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
}
