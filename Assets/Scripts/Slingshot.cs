using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour {

    AlunoController aluno;
    Animator animator;
    LineRenderer lineRenderer;
    LineRenderer arcRenderer;

    public bool draggin = false;
    public float maxDragging;
    public float maxDistance;
    Vector2 touchOrigin;
    Vector2 touchEnd;
    Transform parent;

    // Use this for initialization
    void Start () {
        aluno = GetComponentInParent<AlunoController>();
        animator = GetComponentInParent<Animator>();
        parent = GetComponentInParent<Transform>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, parent.position);
        lineRenderer.SetPosition(1, parent.position);
        lineRenderer.sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
        lineRenderer.enabled = false;

        /*
        arcRenderer = gameObject.transform.GetChild(0).GetComponent<LineRenderer>();
        arcRenderer.sortingLayerName = lineRenderer.sortingLayerName;
        arcRenderer.positionCount = 2;
        arcRenderer.SetPosition(0, parent.position);
        arcRenderer.SetPosition(1, parent.position);
        arcRenderer.gameObject.SetActive(false);
        //gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = arcRenderer.sortingLayerName;
        */
    }

    // Update is called once per frame
    void Update () {
		
	}
    private void OnMouseDown()
    {
        if (animator.GetBool("temCola"))
        {
            draggin = true;
            //arcRenderer.gameObject.SetActive(true);
            //Debug.Log("Clicou em mim");
            touchOrigin = parent.position;
            lineRenderer.SetPosition(1, touchOrigin);
            lineRenderer.enabled = true;
        }
    }
    private void OnMouseUp()
    {
        draggin = false;
        //Debug.Log("Me soltou");
        touchEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lineRenderer.SetPosition(1, parent.position);
        lineRenderer.enabled = false;
        //arcRenderer.SetPosition(1, parent.position);
        //arcRenderer.gameObject.SetActive(false);
    }
    private void OnMouseDrag()
    {
        if (animator.GetBool("temCola"))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - touchOrigin;
            float magnitude = direction.magnitude;
            direction = direction.normalized;//Vetor unitário, a partir da origem, na direção de interesse
            direction *= (magnitude < maxDragging) ? magnitude : maxDragging;//vetor de tamanho maxDragging a partir da origem

            direction.Set(direction.x + parent.position.x, direction.y + parent.position.y);//vetor transladado para posição correta
            lineRenderer.SetPosition(1, direction);

            //DrawArc(direction);
            //Debug.Log("Me arrastandooo");
        }
    }
    /*
    private void DrawArc(Vector2 direction)
    {
        for (int i = 1; i < arcRenderer.positionCount; i++)
        {
            Vector2 difference = new Vector2(transform.position.x - direction.x, transform.position.y - direction.y);
            float magnitude = difference.magnitude;
            difference = difference.normalized * maxDistance * (magnitude / maxDragging); //direction.magnitude/maxDragging <= 1
            Vector2 destination = new Vector2(transform.position.x + difference.x, transform.position.y + difference.y);
            Debug.Log("direction: " + direction + "| transform.position: " + transform.position + "| destination:  " + destination);
            arcRenderer.SetPosition(i, destination);
        }
    }
    */
}
