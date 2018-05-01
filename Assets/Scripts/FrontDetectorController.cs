using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDetectorController : MonoBehaviour {

    private ProfessorIA professor;
    private Animator professorAnimator;
    public float distanciaDaCola;
    // Use this for initialization
	void Start () {
        professor = GameObject.FindGameObjectWithTag("Professor").GetComponent<ProfessorIA>();
        professorAnimator = professor.GetComponentInChildren<Animator>();

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
            if( (professorAnimator.GetInteger("direction") == 3 &&
                collision.transform.position.y >= professor.transform.position.y) 
                ||
                ((professorAnimator.GetInteger("direction") == 4 ||
                 professorAnimator.GetInteger("direction") == 0) && 
                 collision.transform.position.y <= professor.transform.position.y ))
            {
                professor.Catch(collision.GetComponent<Cola>());
            }

            if  (((Mathf.Abs(collision.transform.position.y - professor.transform.position.y) < distanciaDaCola )))
            {
                AlunoController.colaNasCostas = true;
            }

        }
    }
}
