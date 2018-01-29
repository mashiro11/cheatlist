using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlunoController : MonoBehaviour {

    public GameObject cola;
    public GameObject progressoCola;

    public AnimationClip[] animIdle;
    public AnimationClip[] animColando;
    public AnimationClip[] animTran;
    public AnimationClip[] animPassaDir;
    public AnimationClip[] animPassaEsq;
    public AnimationClip[] animPassaFrente;
    public AnimationClip[] animPassaTraz;

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
    private int tipoAluno = 0;
    private bool busted = false;
    

	// Use this for initialization
	void Awake () {
        //tipoAluno = Random.Range(0,5);
        tipoAluno = 0;
        cola = GameObject.FindGameObjectWithTag("Cola");

        /*
         *  Selecionando as animacoes 
         */
        animator = GetComponent<Animator>();

        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = aoc;
        aoc["IDLE"] = animIdle[tipoAluno];
        aoc["TRANQUILO"] = animTran[tipoAluno];
        aoc["PASSADIR"] = animPassaDir[tipoAluno];
        aoc["PASSAESQ"] = animPassaEsq[tipoAluno];
        //aoc["COLANDO"] = animColando[tipoAluno];
        //aoc["PASSAFRENTE"] = animPassaFrente[tipoAluno];
        //aoc["PASSATRAZ"] = animPassaTraz[tipoAluno];

        animator.SetBool("temCola", false);
	}
	
	// Update is called once per frame
	void Update () {
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
            PassaCola(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        }
    }

    public void PassaCola(Vector2 velocity)
    {
        if (Mathf.Abs(velocity.x) == Mathf.Abs(velocity.y))
        {
            velocity = new Vector3(0, 0, 0);
        }

        velocity *= velocidadeCola;
        bool firstTry = false;
        bool buttonPressed = false;
        firstTry = Input.GetKeyDown(KeyCode.UpArrow) ||
                    Input.GetKeyDown(KeyCode.DownArrow) ||
                    Input.GetKeyDown(KeyCode.LeftArrow) ||
                    Input.GetKeyDown(KeyCode.RightArrow);
        if (firstTry && animator.GetBool("temCola") && tempoMinimo <= 0)
        {
            if (position.x == 0)
            {
                Debug.Log("Sou de baixo, pode ir pra cima");
                buttonPressed |= Input.GetKeyDown(KeyCode.UpArrow);
            }
            else
            {
                Debug.Log("Nao sou de baixo, pode ir pra baixo");
                buttonPressed |= Input.GetKeyDown(KeyCode.DownArrow);
                if (position.x != 3)
                {
                    Debug.Log("Nao sou de cima, pode ir pra cima");
                    buttonPressed |= Input.GetKeyDown(KeyCode.UpArrow);
                }
            }

            if (position.y == 0)
            {
                Debug.Log("Sou de esquerda, pode ir pra direita");
                buttonPressed |= Input.GetKeyDown(KeyCode.RightArrow);
            }
            else
            {
                Debug.Log("Nao sou de esquerda, pode ir pra esquerda");
                buttonPressed |= Input.GetKeyDown(KeyCode.LeftArrow);
                if (position.y != 4)
                {
                    Debug.Log("Nao sou de direita, pode ir pra direita");
                    buttonPressed |= Input.GetKeyDown(KeyCode.RightArrow);
                }
            }
        }

        if (animator.GetBool("temCola") && velocity.magnitude > 0 && buttonPressed)
        {
            Debug.Log(position.x + ", " + position.y);
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

}
