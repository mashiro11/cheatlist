using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cola : MonoBehaviour {

    public AlunoController shooter = null;
    public AlunoController receiver = null;
    private Rigidbody2D rBody;
	// Use this for initialization
	void Start () {
        rBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		//oi
	}

    public void OnTriggerEnter(Collider collider)
    {
        /*
        if (collider.gameObject.tag == "Aluno")
        {
            if (shooter != collider.gameObject)
            {
                shooter.GetComponent<AlunoController>().shoot = false;
                collider.gameObject.GetComponent<AlunoController>().RecebeCola();
                //Destroy(this.gameObject);
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                this.transform.position = collider.transform.position;
            }
        }*/
    }
    public void SetVelocity(Vector2 velocity)
    {
        rBody.velocity = velocity;
    }
}
