using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalDetector : MonoBehaviour {

    private GameObject professor;
    private Animator professorAnimator;

    // Use this for initialization
    void Start()
    {
        professor = GameObject.FindGameObjectWithTag("Professor");
        professorAnimator = professor.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        transform.position = new Vector2(0, professor.transform.position.y);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Cola"))
        {
            //Debug.Log("Colidiu horizontal:");
            //Debug.Log("Direction: " + professor.GetComponent<Animator>().GetInteger("direction"));
            //Debug.Log("cola|prof: " + collision.transform.position + " | " + professor.transform.position);

            if (collision.GetComponent<Rigidbody2D>().velocity.magnitude > 0 &&
                ((professorAnimator.GetInteger("direction") == 1 &&
                collision.transform.position.x <= professor.transform.position.x)
                ||
                (professorAnimator.GetInteger("direction") == 2 &&
                 collision.transform.position.x >= professor.transform.position.x)))
            {
                professor.GetComponent<ProfessorIA>().Catch(collision.GetComponent<Cola>());
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("Cola"))
        {
            //Debug.Log("Colidiu horizontal:");
            //Debug.Log("Direction: " + professor.GetComponent<Animator>().GetInteger("direction"));
            //Debug.Log("cola|prof: " + collision.transform.position + " | " + professor.transform.position);

            if (collision.GetComponent<Rigidbody2D>().velocity.magnitude > 0 &&
                ((professorAnimator.GetInteger("direction") == 1 &&
                collision.transform.position.x <= professor.transform.position.x)
                ||
                (professorAnimator.GetInteger("direction") == 2 &&
                 collision.transform.position.x >= professor.transform.position.x)))
            {
                professor.GetComponent<ProfessorIA>().Catch(collision.GetComponent<Cola>());
            }
        }
    }
}
