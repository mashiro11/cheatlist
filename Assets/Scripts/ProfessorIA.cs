﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessorIA : MonoBehaviour {
    public GameObject cola;
    public Vector2 startingPoint = new Vector2(1, 3);
    public float speed = 3f;
    public float waitTime = 4f;
    public float distanceToTables;
    private Vector2 destination;//Diz qual posição da matriz positions o professor deve ir

    private List<List<Vector2>> positions = new List<List<Vector2>>();
    private int lastLine = 0;
    private int lastItem = 0;
    private int maxLinhas = 0;
    private int maxColunas = 0;
    private Vector2 currentPoint = Vector2.zero;//Diz qual a posição da matriz positions o professor vai sair
    private Vector2 direction = Vector2.zero;
    private float delta = 0.3f;
    private float waitTimer;
    private bool professorStopped = true;
    private Animator animator;
    private bool facingLeft = true;
    private SpriteRenderer shadow;
    private SpriteRenderer professorSprite;
    private AudioListener listener;

    private GameObject destinationKnob;
    private string debugTag;
    private void Awake()
    {
        debugTag = "[Professor]:";
    }
    // Use this for initialization
    void Start () {
        professorSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        shadow = professorSprite.transform.GetChild(0).GetComponent<SpriteRenderer>();
        destinationKnob = GameObject.Find("Destination");
        listener = GetComponent<AudioListener>();
        currentPoint = startingPoint;
        destination = Vector2.zero;
        professorSprite.sortingLayerName = shadow.sortingLayerName = "Default";

        animator = professorSprite.gameObject.GetComponent<Animator>();
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
                AddPonto(new Vector2(position.x - ((j == 0) ? 1.3f : ((j == maxColunas) ? 2.7f : 2f)), position.y + distanceToTables));
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
            if(animator.GetFloat("waitTimer") < 0)
            {
                waitTimer = waitTime;
                animator.SetBool("professorStopped", false);
            }
        }
        else
        {
            if (animator.GetInteger("direction") == 0)
            {
                GenerateNextPosition();
                SetDirection();
            }
            MoveProfessor();
        }

        if(animator.GetFloat("waitTimer") <= 0)
        {
            animator.SetFloat("waitTimer", waitTime);
            animator.SetBool("professorStopped", false);
            if (animator.GetBool("foundCheat"))
            {
                animator.SetBool("foundCheat", false);
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

    private void GenerateNextPosition()
    {
        if (Random.Range(1, 11) > 5f)
        {
            //sorteia uma linha             //mantem a coluna
            destination.Set(Random.Range(0, maxLinhas), currentPoint.y);
        }
        else
        {                   //mantem a linha       //sorteia uma coluna
            destination.Set(currentPoint.x, Random.Range(0, maxColunas) );
        }
        destinationKnob.transform.position = positions[(int)destination.x][(int)destination.y];

    }

    private void SetDirection()
    {
        //destination.x -> qual linha de destino
        //destination.y -> qual coluna de destino
        //movimento entre linhas
        if (destination.x != currentPoint.x)
        {
            //Linha destino > linha atual
            if (destination.x > currentPoint.x)
                direction.Set(0, 1);//cima
            //linha destino < linha atual
            else if (destination.x < currentPoint.x)
                direction.Set(0, -1);//baixo
            animator.SetInteger("direction", ((direction.y > 0) ? 3 : 4));
            string sortingLayer = "";
            if (destination.x == 3) sortingLayer = "Default";
            else sortingLayer = "Fileira" + destination.x;
            professorSprite.sortingLayerName = shadow.sortingLayerName = sortingLayer;
        }
        //movimento entre colunas
        else if (destination.y != currentPoint.y)
        {
            //coluna destino > coluna atual
            if (destination.y > currentPoint.y)
            {
                direction.Set(1, 0);//direita
                animator.SetInteger("direction", 2);

            }
            //coluna destino < coluna atual
            else if (destination.y < currentPoint.y)
            {
                direction.Set(-1, 0);//esquerda
                animator.SetInteger("direction", 1);
            }
            if (facingLeft && direction.x > 0)
                Flip();
            else if (!facingLeft && direction.x < 0)
                Flip();
        }
        Debug.Log(debugTag + "New Direction: " + direction);
    }

    private void MoveProfessor()
    {
        Vector2 position = transform.position;
        position += direction * speed * Time.deltaTime;
        transform.position = position;
        //Verificar se ja está junto
        try
        {
            if (transform.position.x > positions[(int)destination.x][(int)destination.y].x - delta &&
                transform.position.x < positions[(int)destination.x][(int)destination.y].x + delta &&
                transform.position.y > positions[(int)destination.x][(int)destination.y].y - delta &&
                transform.position.y < positions[(int)destination.x][(int)destination.y].y + delta)
            {
                //Se estiver, trunca a posição atual para o destino
                transform.position = positions[(int)destination.x][(int)destination.y];
                currentPoint = destination;
                animator.SetBool("professorStopped", true);
                if (!facingLeft) Flip();
                animator.SetInteger("direction", 0);
            }
        }catch(System.ArgumentOutOfRangeException e)
        {
            Debug.Log("Tentando acessar: [" + destination.x + "][" + destination.y + "]");
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
            Catch(collider.gameObject.GetComponent<Cola>());
        }
    }

    public void Catch(Cola cola)
    {
        animator.SetBool("professorStopped", true);
        animator.SetBool("foundCheat", true);

        GameManager.Instance.Busted();

        if (cola.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 0)
        {
            GenerateNearest(Cola.GetShooter().transform.position);
            SetDirection();
        }

        if(Cola.GetShooter()) Cola.GetShooter().Busted();
        if(Cola.GetReceiver()) Cola.GetReceiver().Busted();
    }

    public void GenerateNearest(Vector2 position)
    {
        Vector2 temp = transform.position;
        Vector2 lineOrColumn = temp - position;
        
        Vector2 nearest = new Vector2(currentPoint.x, currentPoint.y);
        int max = 0;
        int i = 0, j = 0;
        unsafe
        {
            int* k;
            if (lineOrColumn.x > lineOrColumn.y)
            {
                max = maxColunas;
                k = &i;
                j = (int)currentPoint.y;
            }
            else
            {
                max = maxLinhas;
                k = &j;
                i = (int)currentPoint.x;
            }

            for (; *k < max; (*k)++)
            {
                try
                {
                    if ((positions[i][j] - position).magnitude < (positions[(int)nearest.x][(int)nearest.y] - position).magnitude)
                    {
                        nearest.Set(i, j);
                    }
                }
                catch(System.ArgumentOutOfRangeException e)
                {
                    Debug.Log("Tentando acessar: [" + i + "][" + j + "]");
                }
            }
        }
        destination = nearest;
        destinationKnob.transform.position = positions[(int)destination.x][(int)destination.y];
        Debug.Log(debugTag + "Nearest position: " + destination );
    }


}
