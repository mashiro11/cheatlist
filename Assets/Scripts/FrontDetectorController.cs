using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDetectorController : MonoBehaviour {

    private GameObject professor;
    public bool colaNasCostas;
    public float distanciaDaCola;
    // Use this for initialization
	void Start () {
        professor = GameObject.FindGameObjectWithTag("Professor");
	}
	
	// Update is called once per frame
	void Update () {
       
	}

    private void LateUpdate()
    {
        transform.position = new Vector2(professor.transform.position.x, 0);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Cola"))
        {
            Debug.Log("Chamei no filho");
            if( (professor.GetComponent<Animator>().GetInteger("direction") == 3 &&
                collision.transform.position.y >= professor.transform.position.y) 
                ||
                ((professor.GetComponent<Animator>().GetInteger("direction") == 4 ||
                 professor.GetComponent<Animator>().GetInteger("direction") == 0) && 
                 collision.transform.position.y <= professor.transform.position.y ))
            {
                professor.GetComponent<ProfessorIA>().Catch(collision.GetComponent<Cola>());
            }

            if  (((Mathf.Abs(collision.transform.position.y - professor.transform.position.y) < distanciaDaCola )))
            {
                colaNasCostas = true;
            }

        }
    }
}
