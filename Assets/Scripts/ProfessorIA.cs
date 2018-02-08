using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessorIA : MonoBehaviour {
    public GameObject cola;
    public Vector2 startingPoint = new Vector2(1, 3);
    public float speed = 3f;
    public float waitTime = 4f;
    private Vector2 destination;//Diz qual posição da matriz positions o professor deve ir

    private List<List<Vector2>> positions = new List<List<Vector2>>();
    private int lastLine = 0;
    private int lastItem = 0;
    private int maxLinhas = 0;
    private int maxColunas = 0;
    private Vector2 currentPoint = Vector2.zero;//Diz qual a posição da matriz positions o professor vai sair
    private Vector2 velocity = Vector2.zero;
    private Rigidbody2D rBody = null;
    private float delta = 0.3f;
    private float waitTimer;
    private bool professorStopped = true;
    private Animator animator;
    private bool facingLeft = true;


    // Use this for initialization
    void Start () {
        currentPoint = startingPoint;
        destination = startingPoint;
        GetComponent<SpriteRenderer>().sortingLayerName = "Fileira" + destination.x;

        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("waitTimer", waitTime);
        positions.Add(new List<Vector2>());

        AlunoSpawner alSpawner = GameObject.FindGameObjectWithTag("AlunoSpawner").GetComponent<AlunoSpawner>();
        maxColunas = alSpawner.maxColunas;  
        maxLinhas = alSpawner.maxLinhas;
        
        
        for(int i = 0; i < maxLinhas; i++)
        {
            for (int j = 0; j < maxColunas + 1; j++)
            {
                Vector2 position = new Vector2(alSpawner.initialX + j * alSpawner.espacamentoX, alSpawner.initialY + i * alSpawner.espacamentoY);
                AddPonto(new Vector2(position.x - ((j == 0) ? 1.3f : ((j == maxColunas) ? 2.7f : 2f)), position.y + 1f));
                //Debug.Log("Matriz de posicoes (" + i + ", " + j + "):" + positions.Count + ", " + positions[i].Count + " | " + positions[i][j]);
            }
        }
        //Debug.Log("Matriz de posicoes: " + positions.Count + ", " + positions[0].Count);
        transform.position = positions[(int)startingPoint.x][(int)startingPoint.y];
    }

    // Update is called once per frame
    void Update () {
        if (animator.GetBool("professorStopped"))
        {
            animator.SetFloat("waitTimer", animator.GetFloat("waitTimer") - Time.deltaTime);
        }
        else
        {
            if( !animator.GetBool("foundCheat") )
                MoveProfessor();
        }

        if(animator.GetFloat("waitTimer") <= 0)
        {
            if (!animator.GetBool("foundCheat"))
            {
                animator.SetFloat("waitTimer", waitTime);
                animator.SetBool("professorStopped", false);
            }
            else
            {
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GameOver();
            }
        }
    }
    public void AddPonto(Vector2 ponto)
    {
        if(positions[lastLine].Count < maxColunas+1)//cabem maxColunas em uma linha
        {
            positions[lastLine].Add(ponto);
        }
        else
        {
            positions.Add(new List<Vector2>());
            positions[++lastLine].Add(ponto);
        }
        //Instantiate(cola, ponto, Quaternion.identity);
    }

    private void MoveProfessor()
    {
        //se o professor ja está na posicao que devia chegar
        if ((Vector2)transform.position == positions[(int)destination.x][(int)destination.y])
        {
            while (destination == currentPoint)
            {
                if (Random.Range(1, 11) > 5f)
                {
                    //sorteia uma linha             //mantem a coluna
                    destination.Set(Random.Range(0, maxLinhas), currentPoint.y);
                    //Ajusta o vetor velocidade para chegar lá
                    if (destination.x > currentPoint.x)
                        velocity.Set(0, 1);
                    else if (destination.x < currentPoint.x)
                        velocity.Set(0, -1);
                    animator.SetInteger("direction", ((velocity.y > 0) ? 3 : 4));
                    GetComponent<SpriteRenderer>().sortingLayerName = "Fileira" + destination.x;
                }
                else
                {                   //mantem a linha       //sorteia uma coluna
                    destination.Set(currentPoint.x, Random.Range(0, maxColunas));
                    if (destination.y > currentPoint.y)
                    {
                        velocity.Set(1, 0);
                        animator.SetInteger("direction", 2);

                    }
                    else if (destination.y < currentPoint.y)
                    {
                        velocity.Set(-1, 0);
                        animator.SetInteger("direction", 1);
                    }
                    if (facingLeft && velocity.x > 0)
                        Flip();
                    else if (!facingLeft && velocity.x < 0)
                        Flip();
                }
            }
            //Debug.Log("Destination: " + destination + ": " + positions[(int)destination.x][(int)destination.y]);
            //Debug.Log(velocity);
            rBody.velocity = velocity * speed;
            waitTimer = waitTime;
        }
        else
        {
            //Se ainda não está no ponto sorteado, verificar se ja está bem proximo
            if (transform.position.x > positions[(int)destination.x][(int)destination.y].x - delta &&
                transform.position.x < positions[(int)destination.x][(int)destination.y].x + delta &&
                transform.position.y > positions[(int)destination.x][(int)destination.y].y - delta &&
                transform.position.y < positions[(int)destination.x][(int)destination.y].y + delta)
            {
                //Se estiver, trunca a posição atual para o destino
                transform.position = positions[(int)destination.x][(int)destination.y];
                currentPoint = destination;
                rBody.velocity = Vector2.zero;
                animator.SetBool("professorStopped", true);
                if (!facingLeft) Flip();
                animator.SetInteger("direction", 0);
            }
            else
            {

            }
        }
    }
    public void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 flipper = transform.localScale;
        flipper.x *= -1;
        transform.localScale = flipper;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Cola")
        {
            //Debug.Log("Chamei no professor");
            Catch(collider.gameObject.GetComponent<Cola>().shooter);
        }
    }

    public void Catch(GameObject shooter)
    {
        animator.SetBool("professorStopped", true);
        animator.SetBool("foundCheat", true);
        GameObject.Find("Camera").GetComponent<CameraController>().EndGame();
        rBody.velocity = Vector2.zero;
        shooter.GetComponent<AlunoController>().Busted();
    }


}
