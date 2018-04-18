using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshooterSelector : MonoBehaviour {

    AlunoController alunoParent;
    // Use this for initialization
    private void Awake()
    {
        alunoParent = gameObject.transform.parent.transform.parent.GetComponent<AlunoController>();

    }
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Aluno") &&
            alunoParent.transform != collider.transform )
        {
            AlunoController aluno = collider.GetComponent<AlunoController>();
            Debug.Log("Encontrei aluno " + aluno.position);
        }
    }
}
