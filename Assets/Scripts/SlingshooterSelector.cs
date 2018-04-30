using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshooterSelector : MonoBehaviour {

    AlunoController alunoParent;
    Vector2Int alunoPosition;
    public Vector2Int AlunoPosition
    {
        get { return alunoPosition; }
    }

    private bool alunoSelected;
    public bool AlunoSelected {
        get { return alunoSelected; }
    }
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
            aluno.SetOutline(true);
            alunoSelected = true;
            alunoPosition.Set((int)aluno.position.x, (int)aluno.position.y);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Aluno") &&
            alunoParent.transform != collider.transform)
        {
            AlunoController aluno = collider.GetComponent<AlunoController>();
            aluno.SetOutline(false);
            alunoSelected = false;
        }
    }
}
