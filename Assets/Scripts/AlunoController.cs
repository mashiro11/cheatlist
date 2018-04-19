using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlunoController : MonoBehaviour {

    /*
     *      Variáveis estáticas
     */
    enum AlunoSounds
    {
        PassaCola = 0,
        Busted,
        Colando
    }
    public static Object aluno;
    public static float initialX = -8, initialY = -4.5f;
    public static float espacamentoX = 4, espacamentoY = 2.1f;
    public static int maxLinhas = 4;
    public static int maxColunas = 5;
    public static Vector2 inicial = new Vector2(1, 3); //posição de quem começa com a cola

    private static GameObject[,] alunos = new GameObject[maxLinhas, maxColunas];
    private static float[,] cheatProgress = new float[maxLinhas, maxColunas];

    private static int bustedCounter = 0;
    private static int finishedCounter = 0;
    private static int chances = 5;
    private static int minToWin = 10;

    private static bool stopControls = false;
    


    public GameObject progressoCola;
    
    public AudioClip[] sounds;
    float colaRapida = 1;
    public bool dedoDuro;

    [HideInInspector]
    public Vector2 position;
    public bool terminou = false;
    public bool draggin = false;
    public float maxDragging;
    public float maxDistance;

    //#if UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID
    public Vector2 touchOrigin = new Vector2(-1, -1);
    public Vector2 touchEnd = new Vector2(-1, -1);
    bool clicked = false;
//#endif
    const int LEFT_CLICK = 0;
    const int RIGHT_CLICK = 1;
    const int MIDDLE_CLICK = 2;

    float tempoMinimo;
    public float tempoMinimoDefinido;
    public static float tempoNecessario = 10;

    public Sprite[] sprites;
    public Animator animator;
    public AnimationClip anim;
    public int tipoAluno = 0;
    public bool busted = false;
    private AudioSource aSource;
    private Camera cam;
    private string debugTag;
    private Slingshot slingshot;
    public LineRenderer outline;
    //private Bounds spriteBounds;
    // Use this for initialization
    void Awake () {
        tipoAluno = Random.Range(0,2);
        if(tipoAluno == 2) tipoAluno = 1;
        
        aSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        slingshot = GetComponentInChildren<Slingshot>();
        
        animator.SetBool("temCola", false);
        outline = GetComponent<LineRenderer>();
    }
    private void Start()
    {

        if(Camera.main != null)
        {
            cam = Camera.main;
        }
        else
        {
            Debug.Log(debugTag + "Vai dar ruim");
        }
        debugTag = "[Aluno " + position + "]: ";
    }

    // Update is called once per frame
    void Update () {
        if (!stopControls)
        {
            if (!busted && animator.GetBool("temCola"))
            {
                if (animator.GetFloat("tempoComCola") < tempoNecessario)
                {
                    Debug.Log("AlunoColando: " + (int)AlunoSounds.Colando);
                    aSource.clip = sounds[(int)AlunoSounds.Colando];
                    aSource.Play();
                    MostraProgressoCola();
                }
                if (tempoMinimo > 0)
                {
                    tempoMinimo -= Time.deltaTime;
                }
                if (!slingshot.draggin)
                {
                    //vai ser modificado.
                    Vector2 direction = GetInputDirection();
                    if (direction.magnitude != 0 &&
                        animator.GetBool("temCola") &&
                        tempoMinimo <= 0 &&
                        CanThrow(direction))
                    {
                        //direction.y indica a direção vertical do arremesso;
                        //arremessos verticais alteram a linha que a cola está;
                        Cola.SetReceiver(alunos[(int)(position.x + direction.y), (int)(position.y + direction.x)].GetComponent<AlunoController>());
                    }
                }
            }
        }
    }

    public void PassaCola(AlunoController al)
    {
        Vector3 direction = Vector3.Normalize(al.position - position);
        if (direction.y < 0) {
            animator.SetInteger("direcao", 1);
            if (!animator.GetBool("olhandoEsquerda")) Flip();
        }else if (direction.y > 0)
        {
            animator.SetInteger("direcao", 2);
            if (animator.GetBool("olhandoEsquerda")) Flip();
        }

        if (direction.x > 0) animator.SetInteger("direcao", 3);
        else if(direction.x < 0) animator.SetInteger("direcao", 4);


        animator.SetTrigger("passandoACola");
        animator.SetBool("temCola", false);
        Debug.Log("AlunoPassaCola: " + (int)AlunoSounds.PassaCola);
        aSource.clip = sounds[(int)AlunoSounds.PassaCola];
        aSource.Play();
        progressoCola.SetActive(false);
        
        FindObjectOfType<FrontDetectorController>().colaNasCostas = false;
    }

    public void MostraProgressoCola()
    {
        if (FindObjectOfType<FrontDetectorController>().colaNasCostas)
        {
            colaRapida = 2;
        }
        cheatProgress[(int)position.x,(int)position.y] += Time.deltaTime * colaRapida;
        Mathf.Clamp(cheatProgress[(int)position.x, (int)position.y], 0, tempoNecessario);
        float time = cheatProgress[(int)position.x, (int)position.y];

        animator.SetFloat("tempoComCola", time);
        
        if (time > tempoNecessario/4 && time < tempoNecessario/2)
        {
            progressoCola.GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
        if (time > tempoNecessario/2 && time < 3*tempoNecessario/4)
        {
            progressoCola.GetComponent<SpriteRenderer>().sprite = sprites[2];
        }
        if (time > tempoNecessario)
        {
            progressoCola.GetComponent<SpriteRenderer>().sprite = sprites[3];
            if (!terminou)
            {
                terminou = true;
                if(++finishedCounter + bustedCounter == 20)
                {
                    GameManager.FimDeJogo();
                }
                
            }
        }
    } 

    public void RecebeCola ()
    {
        animator.SetBool("temCola", true);
        tempoMinimo = tempoMinimoDefinido;
        progressoCola.SetActive(true);
        Cola.SetShooter(this);
        Cola.SetReceiver(null);
        Cola.SetVelocity(new Vector2(0,0));
        Cola.SetPosition(this.transform.position);
    }

    public void Busted()
    {
        //Se chegou no limite de alunos que podem ser pegos, game over
        //Debug.Log(debugTag + "Busted: " + bustedCounter + "| Chances: " + chances);
        if (++bustedCounter > chances)
        {
            GameManager.GameOver();
        }
        else
        {
            //Se ainda estiver para receber a cola, 
            //ja considera que recebeu (professor pega os dois)
            if (Cola.GetReceiver() == this)
            {
                RecebeCola();
            }

            //tenta passar pra frente. Se não conseguir, game over
            Vector2 direction = FindAvailable();
            if (direction.magnitude == Vector2.zero.magnitude)
            {
                GameManager.GameOver();
            }
            //PassaCola(direction);

            //O que o aluno tinha de pontos não soma mais na média da turma
            cheatProgress[(int)position.x, (int)position.y] = 0;
            
            //se ele tinha terminado, é desconsiderado
            if (terminou)
            {
                --finishedCounter;
            }

            busted = true;
            tempoMinimo = 0;
            aSource.Stop();
            Debug.Log("AlunoBusted: " + (int)AlunoSounds.Busted);
            aSource.clip = sounds[(int)AlunoSounds.Busted];
            aSource.Play();
            animator.SetTrigger("busted");
        }
    }

    private Vector2 FindAvailable()
    {
        Vector2 direction = new Vector2(0, -1);
        if (!CanThrow(direction))
        {
            direction.Set(0, 1);
            if (!CanThrow(direction))
            {
                direction.Set(-1, 0);
                if (!CanThrow(direction))
                {
                    direction.Set(1, 0);
                    if (!CanThrow(direction))
                    {
                        direction.Set(0, 0);
                    }
                }
            }
        }
        //Debug.Log(debugTag+ "Direction available!");
        return direction;
    }

    public void Flip()
    {
        animator.SetBool("olhandoEsquerda", !animator.GetBool("olhandoEsquerda"));
        Vector3 flipper = transform.localScale;
        flipper.x *= -1;
        transform.localScale = flipper;
    }

	public bool CanThrow(Vector2 direction){
		// position(i==x, j==y), linha e coluna
		//direction(x, y), direção horizontal ou vertical
		//coluna move com x
		//linha move com y
		bool canThrow = false;
        bool cima = direction.y == 1;
        bool baixo = direction.y == -1;
        bool esquerda = direction.x == -1;
        bool direita = direction.x == 1;
        
        if (position.x == 0)
        {
            //Debug.Log("Sou de baixo, pode ir pra cima");
            canThrow |= cima;
        }
        else
        {
            //Debug.Log("Nao sou de baixo, pode ir pra baixo");
            canThrow |= baixo;
            if (position.x != 3)
            {
                //Debug.Log("Nao sou de cima, pode ir pra cima");
                canThrow |= cima;
            }
        }

        if (position.y == 0)
        {
            //Debug.Log("Sou de esquerda, pode ir pra direita");
            canThrow |= direita;
        }
        else
        {
            //Debug.Log("Nao sou de esquerda, pode ir pra esquerda");
            canThrow |= esquerda;
            if (position.y != 4)
            {
                //Debug.Log("Nao sou de direita, pode ir pra direita");
                canThrow |= direita;
            }
        }

        if (canThrow)
        {
            AlunoController receiver = alunos[(int)(position.x + direction.y),(int)(position.y + direction.x)].GetComponent<AlunoController>();
            if (receiver.busted)
            {
                canThrow = false;
            }
        }
        return canThrow;
	}

	private Vector2 GetInputDirection()
    {
        Vector2 direction = Vector2.zero;
        //Mouse funciona independente do target device
        if (Input.GetMouseButtonDown(LEFT_CLICK) && !clicked)
        {
            touchOrigin = Input.mousePosition;
            //Debug.Log("ori: " + touchOrigin);
            clicked = true;
        }
        else if (Input.GetMouseButtonUp(LEFT_CLICK) && clicked)
        {
            touchEnd = Input.mousePosition;
            direction.x = touchEnd.x - touchOrigin.x;
            direction.y = touchEnd.y - touchOrigin.y;
            //Debug.Log("end: " + touchEnd);
            touchOrigin.Set(-1, -1);

            if (Mathf.Abs(direction.x) == Mathf.Abs(direction.y))
            {
                direction.Set(0, 0);
            }
            else if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                direction.Set(direction.x / Mathf.Abs(direction.x), 0);
            }
            else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
            {
                direction.Set(0, direction.y / Mathf.Abs(direction.y));
            }
            clicked = false;
        }
#if UNITY_STANDALONE || UNITY_WEBPLAYER
        direction.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Mathf.Abs(direction.x) == Mathf.Abs(direction.y))
        {
            direction.Set(0, 0);
        }

#elif UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];
            
            if (myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            else if (myTouch.phase == TouchPhase.Ended)
            {
                Vector2 touchEnd = myTouch.position;
                direction = touchEnd - touchOrigin;
                direction = direction.x == direction.y ? direction : Vector2.zero;
                direction.x = (direction.x > direction.y) ? direction.x : 0;
                direction.y = (direction.y > direction.x) ? direction.y : 0;

                direction = Vector3.Normalize(direction);
                touchOrigin.Set(-1, -1);
            }            
        }
#endif
        return direction;
    }

    public static void StopControls(bool stop)
    {
        stopControls = stop;
    }

    public static void ResetParameters()
    {
        chances = 5;
        minToWin = 8;
        bustedCounter = 0;
        finishedCounter = 0;
        stopControls = false;
        for(int i = 0; i < maxLinhas; i++)
        {
            for(int j = 0; j < maxColunas; j++)
            {
                cheatProgress[i, j] = 0; 
            }
        }
    }

    public static float GetMean()
    {
        float sum = 0;
        for(int i = 0; i < maxLinhas; i++)
        {
            for(int j = 0; j < maxColunas; j++)
            {
                sum += cheatProgress[i, j] / tempoNecessario * 10;
            }
        }
        return sum/20;
    }

    public static void SpawnAlunos()
    {
        Instantiate(Resources.Load("Cola"), Vector2.zero, Quaternion.identity);
        aluno = Resources.Load("Aluno");
        GameObject al;

        for (int i = 0; i < maxLinhas; i++)
        {
            for (int j = 0; j < maxColunas; j++)
            {
                Vector2 position = new Vector2(initialX + j * espacamentoX, initialY + i * espacamentoY);
                al = ((GameObject)Instantiate(aluno, position, Quaternion.identity));
                alunos[i, j] = al;
                Bounds spriteBounds = al.GetComponent<SpriteRenderer>().sprite.bounds;
                LineRenderer outline = al.GetComponent<LineRenderer>();
                outline.positionCount = 5;
                outline.SetPosition(0, new Vector3(position.x - spriteBounds.extents.x, position.y - spriteBounds.extents.y, 0));
                outline.SetPosition(1, new Vector3(position.x + spriteBounds.extents.x, position.y - spriteBounds.extents.y, 0));
                outline.SetPosition(2, new Vector3(position.x + spriteBounds.extents.x, position.y + spriteBounds.extents.y, 0));
                outline.SetPosition(3, new Vector3(position.x - spriteBounds.extents.x, position.y + spriteBounds.extents.y, 0));
                outline.SetPosition(4, new Vector3(position.x - spriteBounds.extents.x, position.y - spriteBounds.extents.y, 0));
                outline.enabled = false;

                al.GetComponent<AlunoController>().position = new Vector2(i, j);
                al.GetComponent<SpriteRenderer>().sortingLayerName = "Fileira" + i;
                //Debug.Log(i + ", " + j);
                if (Random.Range(1, 11) <= 1)
                {
                    al.GetComponent<AlunoController>().dedoDuro = true;
                    GameManager.contadorDeDedoDuro++;
                }

                if (i == inicial.x && j == inicial.y)
                {
                    al.GetComponent<AlunoController>().RecebeCola();
                    Cola.SetShooter(al.GetComponent<AlunoController>());

                    if (al.GetComponent<AlunoController>().dedoDuro)
                    {
                        al.GetComponent<AlunoController>().dedoDuro = false;
                        GameManager.contadorDeDedoDuro--;
                    }
                }
            }
        }
    }

    public static AlunoController GetAluno (Vector2Int position)
    {
        Debug.Log("position: " + position);
        return alunos[position.x, position.y].GetComponent<AlunoController>();
    }
}
