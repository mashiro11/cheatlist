using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessorIA : MonoBehaviour {
    List<List<Vector2>> positions = new List<List<Vector2>>();
    private int lastLine = 0;
    private int lastItem = 0;
    public GameObject cola;
    private int maxLinhas = 0;
    private int maxColunas = 0;

    // Use this for initialization
	void Start () {
        AlunoSpawner alSpawner = GameObject.FindGameObjectWithTag("AlunoSpawner").GetComponent<AlunoSpawner>();
        maxColunas =  alSpawner.maxColunas;
        maxLinhas = alSpawner.maxLinhas;
        positions.Add(new List<Vector2>());
        for(int i = 0; i < maxColunas + 1; i++)
        {
            for(int j = 0; j < maxLinhas; j++)
            {
                Vector2 position = new Vector2(alSpawner.initialX + i * alSpawner.espacamentoX, alSpawner.initialY + j * alSpawner.espacamentoY);
                AddPonto(new Vector2(position.x - ((i == 0) ? 1.3f : ((i == maxColunas)? 2.7f :2f ) ), position.y + 0.5f));
            }
        }
        transform.position = positions[1][3];
        GetComponent<SpriteRenderer>().sortingLayerName = "Fileira" + maxLinhas;
    }

    // Update is called once per frame
    void Update () {
		
	}
    public void AddPonto(Vector2 ponto)
    {
        if(positions[lastLine].Count < maxLinhas)
        {
            positions[lastLine].Add(ponto);
        }
        else
        {
            positions.Add(new List<Vector2>());
            lastLine++;
            positions[lastLine].Add(ponto);
        }
        Instantiate(cola, ponto, Quaternion.identity);
    }
    
    
}
