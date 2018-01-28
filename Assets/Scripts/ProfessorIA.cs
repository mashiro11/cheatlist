using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessorIA : MonoBehaviour {
    //List<List<Vector3>> positions;
    private int lastLine = 0;
    private int lastItem = 0;
    // Use this for initialization
	void Start () {
        //positions.Add(new List<Vector3>());
    }

    // Update is called once per frame
    void Update () {
		
	}
    public void AddPonto(Vector3 ponto)
    {
        /*
        if(positions[lastLine].Count < 4)
        {
            positions[lastLine].Add(ponto);
        }
        else
        {
            positions.Add(new List<Vector3>());
            lastLine++;
            positions[lastLine].Add(ponto);
        }
        */
    }
    public void SetPosition()
    {
       // transform.position = positions[1][3];
    }
    
}
