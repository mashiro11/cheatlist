using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlunoController : MonoBehaviour {

    public GameObject cola;
    public GameObject progressoCola;

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
    
    float tempoMinimo;
    public float tempoMinimoDefinido;
    public float tempoNecessario;

    public Sprite[] sprites;
    public Animator animator;
    public AnimationClip anim;
    public int tipoAluno = 0;
    private bool busted = false;
    

	// Use this for initialization
	void Awake () {
        tipoAluno = Random.Range(0,2);
        if(tipoAluno == 2) tipoAluno = 1;

        cola = GameObject.FindGameObjectWithTag("Cola");

        /*
         *  Selecionando as animacoes 
         */
        animator = GetComponent<Animator>();

        aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = aoc;
        
        animator.SetBool("temCola", false);
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
        if (!busted)
        {
            if (animator.GetBool("temCola") && animator.GetFloat("tempoComCola") < tempoNecessario)
            {
                MostraProgressoCola();
            }
            if (tempoMinimo > 0)
            {
                tempoMinimo -= Time.deltaTime;
            }
            PassaCola();
        }
    }

    public void PassaCola()
    {
		bool canThrow = false;
		#if UNITY_STANDALONE || UNITY_WEBPLAYER
		Vector2 velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (Mathf.Abs(velocity.x) == Mathf.Abs(velocity.y))
		{
			velocity = new Vector3(0, 0, 0);
		}

		velocity *= velocidadeCola;
		bool firstTry = false;
        
        firstTry = Input.GetKeyDown(KeyCode.UpArrow) ||
                    Input.GetKeyDown(KeyCode.DownArrow) ||
                    Input.GetKeyDown(KeyCode.LeftArrow) ||
                    Input.GetKeyDown(KeyCode.RightArrow);
        if (firstTry && animator.GetBool("temCola") && tempoMinimo <= 0)
        {
            if (position.x == 0)
            {
                //Debug.Log("Sou de baixo, pode ir pra cima");
                canThrow |= Input.GetKeyDown(KeyCode.UpArrow);
            }
            else
            {
                //Debug.Log("Nao sou de baixo, pode ir pra baixo");
                canThrow |= Input.GetKeyDown(KeyCode.DownArrow);
                if (position.x != 3)
                {
                    //Debug.Log("Nao sou de cima, pode ir pra cima");
                    canThrow |= Input.GetKeyDown(KeyCode.UpArrow);
                }
            }

            if (position.y == 0)
            {
                //Debug.Log("Sou de esquerda, pode ir pra direita");
                canThrow |= Input.GetKeyDown(KeyCode.RightArrow);
            }
            else
            {
                //Debug.Log("Nao sou de esquerda, pode ir pra esquerda");
                canThrow |= Input.GetKeyDown(KeyCode.LeftArrow);
                if (position.y != 4)
                {
                    //Debug.Log("Nao sou de direita, pode ir pra direita");
                    canThrow |= Input.GetKeyDown(KeyCode.RightArrow);
                }
            }
        }
		#elif UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID
		Vector2 velocity = new Vector2();
		if(Input.touchCount > 0){
			Vector2 touchDirection = GetDirection();

			if(Mathf.Abs(touchDirection.x) >= Mathf.Abs(touchDirection.y)){
				velocity.Set(touchDirection.x/Mathf.Abs(touchDirection.x), 0);
			}else{
				velocity.Set(0, touchDirection.y/Mathf.Abs(touchDirection.y));
			}
			if (animator.GetBool("temCola") && tempoMinimo <= 0)
			{
				canThrow = CanThrow(velocity);
			}

		}
		#endif

        if (animator.GetBool("temCola") && velocity.magnitude > 0 && canThrow)
        {
            //Debug.Log(position.x + ", " + position.y);
            //GameObject cl = (GameObject)Instantiate(cola, transform.position, Quaternion.identity);
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
            cola.GetComponent<Rigidbody2D>().velocity = velocity;
            animator.SetBool("temCola", false);
            gameObject.GetComponent<AudioSource>().Play();
            progressoCola.SetActive(false);
            cola.GetComponent<Cola>().shooter = this.gameObject;
        }
    }

    public void MostraProgressoCola()
    {
        animator.SetFloat("tempoComCola", animator.GetFloat("tempoComCola") + Time.deltaTime);
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
                FindObjectOfType<GameManager>().contadorDeAlunos++;
            }
        }
    } 

    public void RecebeCola ()
    {
        animator.SetBool("temCola", true);
        tempoMinimo = tempoMinimoDefinido;
        progressoCola.SetActive(true);
    }

    public void Busted()
    {
        busted = true;
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
            GameObject shooter = collider.gameObject.GetComponent<Cola>().shooter;
            if (shooter != this.gameObject)
            {
                RecebeCola();
                //Destroy(this.gameObject);
                collider.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collider.transform.position = this.transform.position;
            }
        }
    }

	public bool CanThrow(Vector2 direction){
		// position(i==x, j==y), linha e coluna
		//direction(x, y), direção horizontal ou vertical
		//coluna move com x
		//linha move com y
		bool canThrow = false;
		if (position.x == 0)
		{
			//Debug.Log("Sou de baixo, pode ir pra cima");
			canThrow |= direction.y == 1;
		}
		else
		{
			//Debug.Log("Nao sou de baixo, pode ir pra baixo");
			canThrow |= direction.y == -1;
			if (position.x != 3)
			{
				//Debug.Log("Nao sou de cima, pode ir pra cima");
				canThrow |= direction.y == 1;
			}
		}

		if (position.y == 0)
		{
			//Debug.Log("Sou de esquerda, pode ir pra direita");
			canThrow |= direction.x == 1;
		}
		else
		{
			//Debug.Log("Nao sou de esquerda, pode ir pra esquerda");
			canThrow |= direction.x == -1;
			if (position.y != 4)
			{
				//Debug.Log("Nao sou de direita, pode ir pra direita");
				canThrow |= direction.x == 1;
			}
		}
		return canThrow;
	}

	private Vector2 GetDirection (){
		Vector2 touchDirection = new Vector2 ();
		if (Input.touchCount > 0) {
			Touch myTouch = Input.touches [0];
			Vector2 touchOrigin = new Vector2 ();
			Vector2 touchEnd = new Vector2 ();


			if (myTouch.phase == TouchPhase.Began) {
				touchOrigin = myTouch.position;
			} else if (myTouch.phase == TouchPhase.Ended) {
				touchEnd = myTouch.position;
			}
			touchDirection.x = touchEnd.x - touchOrigin.x;
			touchDirection.y = touchEnd.y - touchOrigin.y;
		}
		return touchDirection;
	}

}
