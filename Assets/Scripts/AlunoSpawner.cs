using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlunoSpawner : MonoBehaviour {

    public static AlunoSpawner instance;

    public GameObject aluno;
    public  float initialX, initialY;
    private bool temCola;
    public float espacamentoX, espacamentoY;
    const int maxLinhas = 4;
    const int maxColunas = 5;
    public GameObject cola;
    private GameObject[] alunos = null;
    public GameObject professor;

    public Dictionary<int,float> zPositionAndSortingLayerPerQueue;


    //public ProfessorIA professorIA;
    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        professor = GameObject.FindGameObjectWithTag("Professor");
		temCola = false;
        GameObject al;
        int contador = 0;
        cola.transform.position = this.transform.position;
        //cl = (GameObject)Instantiate(cola, transform.position, Quaternion.identity);
        zPositionAndSortingLayerPerQueue = new Dictionary<int, float>();

        for (int i = 0; i < maxColunas; i++)
        {
        	for(int j = 0; j < maxLinhas; j++, contador++){
        		Vector3 position = new Vector3(initialX + i * espacamentoX , 0, initialY + j * espacamentoY);
                //professorIA.AddPonto(new Vector3(position.x - 2, position.y + 2));
                al = ((GameObject)Instantiate(aluno, position, Quaternion.identity));
                if (!zPositionAndSortingLayerPerQueue.ContainsKey(maxLinhas - j ))
                    zPositionAndSortingLayerPerQueue.Add(maxLinhas - j,  position.z);

                al.GetComponent<AlunoController>().position = new Vector2(j, i);
                al.GetComponent<SpriteRenderer>().sortingLayerName = "Fileira" + (maxLinhas-j);
                al.transform.Rotate(new Vector3(90,0,0));
                
            	if(!temCola && Random.Range(1,10) > 7){
                    temCola = true;
                    al.GetComponent<AlunoController>().RecebeCola();
                    cola.transform.position = new Vector3(al.transform.position.x, al.transform.position.y + 1f, al.transform.position.z);
                    cola.GetComponent<Cola>().shooter = al;
                }

                if (!temCola && i == 4 && j == 3)
                {
                    al.GetComponent<AlunoController>().RecebeCola();
                    cola.transform.position = new Vector3( al.transform.position.x, al.transform.position.y+1f, al.transform.position.z);
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
