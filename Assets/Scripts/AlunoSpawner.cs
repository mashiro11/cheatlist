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
    private GameObject[] alunos = null;

    //public ProfessorIA professorIA;
    private void Awake()
    {
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
                
                al.GetComponent<AlunoController>().position = new Vector2(j, i);
                al.GetComponent<SpriteRenderer>().sortingLayerName = "Fileira" + i;

                if (i == inicial.x && j == inicial.y)
                {
                    al.GetComponent<AlunoController>().RecebeCola();
                    cola.transform.position = al.transform.position;
                    cola.GetComponent<Cola>().shooter = al;
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

		
	}

    public bool TodosTerminaram()
    {
        for (int i=0; i < maxLinhas*maxColunas; i++)
        {
            if (alunos[i].GetComponent<Animator>().GetFloat("tempoComCola") < 10)
            {
                return false;
            }
        }
        return true;
    }
}
