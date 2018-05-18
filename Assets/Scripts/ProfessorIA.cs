using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessorIA : MonoBehaviour {
    public enum AI_TYPE{
        AI_1,
        AI_2,
        NONE //professor stop
    }
    public static AI_TYPE ai_type = AI_TYPE.AI_1;

    //Machine states
    public enum ProfStates
    {
        IDLE,
        MOVING,
        FOUND_CHEAT
    }
    public ProfStates profState = ProfStates.IDLE;

    //GameObject hierarchy
    enum ProfessorChilds
    {
        Art = 0,
        HorizontalDetector,
        FrontDetector
    }
    
    //Pointers to instances
    private GameObject cola;
    private SpriteRenderer shadow;
    private SpriteRenderer professorSprite;
    private Animator animator;

    //Positioning
    public Vector2 startingPoint = new Vector2(1, 3);
    private List<List<Vector2>> positions = new List<List<Vector2>>();
    private Vector2 currentPoint = Vector2.zero;//Diz qual a posição da matriz positions o professor vai sair
    private Vector2 direction = Vector2.zero;

    //Movimentation
    public float speed = 3f;
    public float waitTime = 4f;
    public float distanceToTables;
    private Vector2 destination;//Diz qual posição da matriz positions o professor deve ir


    private int lastLine = 0;
    private int lastItem = 0;
    private int maxLinhas = 0;
    private int maxColunas = 0;

    private float delta = 0.3f;
    private float waitTimer;
    private bool professorStopped = true;
    private bool facingLeft = true;

    //AI metainfo
    private int toGetCloser = 3;
    private int toGetCloserCounter = 1;
    private int getCloserTimes = 4;
    private int getCloserTimesCounter = 1;

#if (DEBUG)
    private GameObject destinationKnob;
#endif
    private string debugTag;

    private void Awake()
    {
        //Debug
        debugTag = "[Professor]:";
#if (DEBUG)
        destinationKnob = Instantiate((GameObject)Resources.Load("Destination"), Vector3.zero, Quaternion.identity);
#endif
    }
    // Use this for initialization
    void Start () {
        professorSprite = transform.GetChild((int)ProfessorChilds.Art).GetComponent<SpriteRenderer>();
        shadow = professorSprite.transform.GetChild(0).GetComponent<SpriteRenderer>();
        professorSprite.sortingLayerName = shadow.sortingLayerName = "Default";

        animator = professorSprite.gameObject.GetComponent<Animator>();
        animator.SetFloat("waitTimer", waitTime);

        currentPoint = startingPoint;
        destination = Vector2.zero;

        positions.Add(new List<Vector2>());
        maxColunas = AlunoController.maxColunas;  
        maxLinhas = AlunoController.maxLinhas;
        
        
        for(int i = 0; i < maxLinhas; i++)
        {
            for (int j = 0; j < maxColunas + 1; j++)
            {
                Vector2 position = new Vector2(AlunoController.initialX + j * AlunoController.espacamentoX, AlunoController.initialY + i * AlunoController.espacamentoY);
                AddPonto(new Vector2(position.x - ((j == 0) ? 1.3f : ((j == maxColunas) ? 2.7f : 2f)), position.y + distanceToTables));
                //Debug.Log("Matriz de posicoes (" + i + ", " + j + "):" + positions.Count + ", " + positions[i].Count + " | " + positions[i][j]);
            }
        }
        //Debug.Log("Matriz de posicoes: " + positions.Count + ", " + positions[0].Count);
        transform.position = positions[(int)startingPoint.x][(int)startingPoint.y];
    }

    // Update is called once per frame
    void Update () {
        if (Input.anyKeyDown)
        {
            switch (Input.inputString)
            {
                case "1":
                    ai_type = AI_TYPE.AI_1;
                    break;
                case "2":
                    ai_type = AI_TYPE.AI_2;
                    break;
                case "0":
                    ai_type = AI_TYPE.NONE;
                    break;
            }
        }

        switch(ai_type){
            case AI_TYPE.NONE:
                return;
        }

        switch (profState)
        {
            case ProfStates.IDLE:
                break;
        }

        if (animator.GetBool("professorStopped"))
        {
            if (animator.GetBool("foundCheat"))
            {
                if (animator.GetFloat("foundCheatTimer") < 0)
                {
                    animator.SetBool("professorStopped", false);
                    animator.SetFloat("waitTimer", -1);
                    animator.SetBool("foundCheat", false);
                }
                else
                {
                    animator.SetFloat("foundCheatTimer", animator.GetFloat("foundCheatTimer") - Time.deltaTime);
                }
            }
            else
            {
                animator.SetFloat("waitTimer", animator.GetFloat("waitTimer") - Time.deltaTime);
                if (animator.GetFloat("waitTimer") < 0)
                {
                    waitTimer = waitTime;
                    animator.SetBool("professorStopped", false);
                }
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
        }
        DataCollector.posicaoProfessor = transform.position;
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
    }

    private void GenerateNextPosition()
    {
        switch (ai_type)
        {
            case AI_TYPE.AI_1:
                if(Random.Range(0, 2) == 0)
                {
                    destination.Set(currentPoint.x, Random.Range(0, maxColunas));
                }
                else
                {
                    destination.Set(Random.Range(0, maxLinhas), currentPoint.y);
                }
                break;
            case AI_TYPE.AI_2:
                if (toGetCloserCounter == toGetCloser)//quantas posições aleatorias ja foram para começar posições de perseguição
                {
                    Debug.Log(debugTag + "Perseguindo!");
                    if (getCloserTimesCounter < getCloserTimes)//quantas posições de perseguição ja foram
                    {
                        Debug.Log(debugTag + getCloserTimesCounter + "a vez");
                        if (getCloserTimesCounter++ == 1) waitTime /= 2;
                        GenerateNearest(Cola.GetPosition());
                    }
                    else
                    {
                        Debug.Log(debugTag + "Terminou essa perseguicao");
                        getCloserTimesCounter = 1;
                        toGetCloserCounter = 1;
                        waitTime *= 2;
                    }
                }
                else
                {
                    toGetCloserCounter++;
                    if (Random.Range(1, 11) > 5f)
                    {
                        //sorteia uma linha             //mantem a coluna
                        destination.Set(Random.Range(0, maxLinhas), currentPoint.y);
                    }
                    else
                    {                   //mantem a linha       //sorteia uma coluna
                        destination.Set(currentPoint.x, Random.Range(0, maxColunas));
                    }
                }
                break;
        }
#if DEBUG
            destinationKnob.transform.position = positions[(int)destination.x][(int)destination.y];
#endif
    }

    public void GenerateNearest(Vector2 position)
    {
        Vector2 temp = transform.position;
        Vector2 lineOrColumn = temp - position;//obtenho um vetor com as distâncias em x e em y

        //de inicio, chuto que o ponto mais próximo é onde estou
        Vector2 nearest = new Vector2(currentPoint.x, currentPoint.y);
        int max = 0;
        int i = 0, j = 0;
        unsafe
        {
            int* k;
            //se a distância em y é maior, significa que o professor deve andar variando as linhas
            if (Mathf.Abs(lineOrColumn.y) > Mathf.Abs(lineOrColumn.x))
            {
                Debug.Log(debugTag + " GetNearest(): k aponta i ");
                max = maxLinhas;
                k = &i;
                j = (int)currentPoint.y;
            }
            else
            {
                Debug.Log(debugTag + " GetNearest(): k aponta j ");
                max = maxColunas;
                k = &j;
                i = (int)currentPoint.x;
            }

            for (; *k < max; (*k)++)
            {
                Debug.Log(debugTag + "positions[" + i + ", " + j + "]: " + positions[i][j]);
                try
                {
                    if ((positions[i][j] - position).magnitude < (positions[(int)nearest.x][(int)nearest.y] - position).magnitude)
                    {
                        Debug.Log(debugTag + "selecionado");
                        nearest.Set(i, j);
                    }
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    Debug.Log(debugTag + "Tentando acessar: [" + i + "][" + j + "]");
                }
            }
        }
        destination = nearest;
        Debug.Log(debugTag + "Entry Position: " + position);
        Debug.Log(debugTag + "Nearest position: " + destination +
            " : (" + positions[(int)destination.x][(int)destination.y].x + "," +
                  positions[(int)destination.x][(int)destination.y].y + ")");
#if DEBUG
        destinationKnob.transform.position = positions[(int)destination.x][(int)destination.y];
#endif
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
            Catch(collider.gameObject.GetComponent<Cola>());
        }
    }

    public void Catch(Cola cola)
    {
        animator.SetBool("professorStopped", true);
        animator.SetBool("foundCheat", true);
        animator.SetFloat("foundCheatTimer", waitTime);

        GameManager.Instance.Busted();

        if (cola.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 0)
        {
            GenerateNearest(Cola.GetShooter().transform.position);
            SetDirection();
        }

        if(Cola.GetShooter()) Cola.GetShooter().Busted();
        if(Cola.GetReceiver()) Cola.GetReceiver().Busted();
        AlunoController.GenerateCheatOnFarest(transform.position);
    }
}
