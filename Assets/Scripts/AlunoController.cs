using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlunoController : MonoBehaviour {

    public GameObject progressoCola;
    
    public AudioClip[] sounds;
    float colaRapida = 1;
    public bool dedoDuro;

    public AnimationClip[] animIdle;
    public AnimationClip[] animTran;
    public AnimationClip[] animPassaDir;
    public AnimationClip[] animPassaEsq;
    public AnimationClip[] animPassaFrente;
    public AnimationClip[] animPassaTraz;
    public AnimationClip[] animColando;
    AnimatorOverrideController aoc;
    private bool changed;

    public int velocidadeCola;
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
    public float tempoNecessario;

    public Sprite[] sprites;
    public Animator animator;
    public AnimationClip anim;
    public int tipoAluno = 0;
    public bool busted = false;
    private AudioSource aSource;
    private LineRenderer lineRenderer;
    private LineRenderer arcRenderer;
    private Camera cam;
    private string debugTag;

	// Use this for initialization
	void Awake () {
        tipoAluno = Random.Range(0,2);
        if(tipoAluno == 2) tipoAluno = 1;
        
        aSource = GetComponent<AudioSource>();
        /*
         *  Selecionando as animacoes 
         */
        animator = GetComponent<Animator>();
        aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = aoc;
        
        animator.SetBool("temCola", false);
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
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
        lineRenderer.sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
        lineRenderer.enabled = false;

        
        arcRenderer = gameObject.transform.GetChild(1).GetComponent<LineRenderer>();
        arcRenderer.sortingLayerName = lineRenderer.sortingLayerName;
        arcRenderer.positionCount = 2;
        arcRenderer.SetPosition(0, transform.position);
        arcRenderer.SetPosition(1, transform.position);
        arcRenderer.gameObject.SetActive(false);

        gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sortingLayerName = arcRenderer.sortingLayerName;
        debugTag = "[Aluno " + position + "]: ";
    }

    // Update is called once per frame
    void Update () {
        if (!changed)
        {
            changed = true;
            aoc["IDLE"] = animIdle[tipoAluno];
            aoc["TRANQUILO"] = animTran[tipoAluno];
            aoc["PASSADIR"] = animPassaDir[tipoAluno];
            aoc["PASSAESQ"] = animPassaEsq[tipoAluno];
            //aoc["COLANDO"] = animColando[tipoAluno];
            //aoc["PASSAFRENTE"] = animPassaFrente[tipoAluno];
            //aoc["PASSATRAZ"] = animPassaTraz[tipoAluno];

        }
        if (!busted && animator.GetBool("temCola"))
        {
            if (animator.GetFloat("tempoComCola") < tempoNecessario)
            {
                MostraProgressoCola();
            }
            if (tempoMinimo > 0)
            {
                tempoMinimo -= Time.deltaTime;
            }
            if (!draggin)
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
                    Cola.SetReceiver(AlunoSpawner.GetAluno((int)(position.x + direction.y), (int)(position.y + direction.x)));
                    PassaCola(direction);
                }
            }
        }
    }

    public void PassaCola(Vector2 direction)
    {
        Debug.Log(debugTag + "vou passar a cola para " + new Vector2(position.x+direction.y,position.y+direction.x));
		Vector2 velocity = direction * velocidadeCola;
        //Debug.Log(position.x + ", " + position.y);
        if (velocity.x < 0) {
            animator.SetInteger("direcao", 1);
            if (!animator.GetBool("olhandoEsquerda")) Flip();
        }else if (velocity.x > 0)
        {
            animator.SetInteger("direcao", 2);
            if (animator.GetBool("olhandoEsquerda")) Flip();
        }

        if (velocity.y > 0) animator.SetInteger("direcao", 3);
        else if(velocity.y < 0) animator.SetInteger("direcao", 4);


        animator.SetTrigger("passandoACola");
        Cola.SetVelocity(velocity);
        animator.SetBool("temCola", false);
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
        animator.SetFloat("tempoComCola", animator.GetFloat("tempoComCola") + Time.deltaTime * colaRapida);
        
        if (animator.GetFloat("tempoComCola") > tempoNecessario/4 && animator.GetFloat("tempoComCola") < tempoNecessario/2)
        {
            progressoCola.GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
        if (animator.GetFloat("tempoComCola") > tempoNecessario/2 && animator.GetFloat("tempoComCola") < 3*tempoNecessario/4)
        {
            progressoCola.GetComponent<SpriteRenderer>().sprite = sprites[2];
        }
        if (animator.GetFloat("tempoComCola") > tempoNecessario)
        {
            progressoCola.GetComponent<SpriteRenderer>().sprite = sprites[3];
            if (!terminou)
            {
                terminou = true;
                AlunoSpawner.OneMoreFinished();
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
        if (Cola.GetReceiver() == this)
        {
            RecebeCola();
        }
        
        Vector2 direction = FindAvailabe();
        if (direction.magnitude == Vector2.zero.magnitude)
        {
            GameManager.GameOver();
        }
        PassaCola( direction );
        
        AlunoSpawner.OneMoreBusted();
        busted = true;
        tempoMinimo = 0;
        aSource.Stop();
        aSource.clip = sounds[1];
        aSource.Play();
        animator.SetTrigger("busted");
    }

    private Vector2 FindAvailabe()
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
        Debug.Log(debugTag+ "Direction available!");
        return direction;
    }

    public void Flip()
    {
        animator.SetBool("olhandoEsquerda", !animator.GetBool("olhandoEsquerda"));
        Vector3 flipper = transform.localScale;
        flipper.x *= -1;
        transform.localScale = flipper;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Cola")
        {
            if (Cola.GetShooter() != this)
            {
                RecebeCola();
            }
        }
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
            AlunoController receiver = (AlunoSpawner.GetAluno((int)(position.x + direction.y), (int)(position.y + direction.x))).GetComponent<AlunoController>();
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
                direction.x = touchEnd.x - touchOrigin.x;
                direction.y = touchEnd.y - touchOrigin.y;
                touchOrigin.Set(-1, -1);
                if (Mathf.Abs(direction.x) == Mathf.Abs(direction.y))
                {
                    direction.Set(0, 0);
                }else if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    direction.Set(direction.x / Mathf.Abs(direction.x), 0);
                }else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
                {
                    direction.Set(0, direction.y/ Mathf.Abs(direction.y));
                }
            }            
        }
#endif
        return direction;
    }
    private void OnMouseDown()
    {
        if (animator.GetBool("temCola"))
        {
            draggin = true;
            arcRenderer.gameObject.SetActive(true);
            //Debug.Log("Clicou em mim");
            touchOrigin = transform.position;
            lineRenderer.SetPosition(1, touchOrigin);
            lineRenderer.enabled = true;
        }
    }
    private void OnMouseUp()
    {
        draggin = false;
        //Debug.Log("Me soltou");
        touchEnd = cam.ScreenToWorldPoint(Input.mousePosition);
        lineRenderer.SetPosition(1, transform.position);
        lineRenderer.enabled = false;
        arcRenderer.SetPosition(1, transform.position);
        arcRenderer.gameObject.SetActive(false);
    }
    private void OnMouseDrag()
    {
        if (animator.GetBool("temCola"))
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - touchOrigin;
            float magnitude = direction.magnitude;
            direction = direction.normalized;//Vetor unitário, a partir da origem, na direção de interesse
            direction *= (magnitude < maxDragging) ? magnitude : maxDragging ;//vetor de tamanho maxDragging a partir da origem

            direction.Set(direction.x + transform.position.x, direction.y + transform.position.y);//vetor transladado para posição correta
            lineRenderer.SetPosition(1, direction);

            DrawArc(direction);
            //Debug.Log("Me arrastandooo");
        }
    }
    private void DrawArc(Vector2 direction)
    {
        for (int i = 1; i < arcRenderer.positionCount; i++)
        {
            Vector2 difference = new Vector2(transform.position.x - direction.x, transform.position.y - direction.y);
            float magnitude = difference.magnitude;
            difference = difference.normalized * maxDistance * (magnitude/maxDragging); //direction.magnitude/maxDragging <= 1
            Vector2 destination = new Vector2(transform.position.x + difference.x,  transform.position.y + difference.y);
            Debug.Log("direction: " + direction + "| transform.position: " + transform.position + "| destination:  " + destination);
            arcRenderer.SetPosition(i, destination);
        }
    }
}
