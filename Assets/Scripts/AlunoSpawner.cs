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
    public GameObject cola;
    private GameObject[] alunos = null;
    private ProfessorIA professor;

    public Dictionary<int,float> zPositionAndSortingLayerPerQueue;


    //public ProfessorIA professorIA;
    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        professor = GameObject.FindGameObjectWithTag("Professor").GetComponent<ProfessorIA>();
		temCola = false;
        GameObject al;
        cola.transform.position = this.transform.position;
        //cl = (GameObject)Instantiate(cola, transform.position, Quaternion.identity);
        
        for (int i = 0; i < maxColunas; i++)
        {
        	for(int j = 0; j < maxLinhas; j++){
        		Vector2 position = new Vector2(initialX + i * espacamentoX , initialY + j * espacamentoY);
                al = ((GameObject)Instantiate(aluno, position, Quaternion.identity));
                
                al.GetComponent<AlunoController>().position = new Vector2(j, i);
                al.GetComponent<SpriteRenderer>().sortingLayerName = "Fileira" + (maxLinhas-j);
                
            	if(!temCola && Random.Range(1,10) > 7){
                    temCola = true;
                    al.GetComponent<AlunoController>().RecebeCola();
                    cola.transform.position = al.transform.position;
                    cola.GetComponent<Cola>().shooter = al;
                }

                if (!temCola && i == 4 && j == 3)
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
