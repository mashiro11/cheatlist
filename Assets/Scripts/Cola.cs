using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cola : MonoBehaviour {

    private static AlunoController shooter;
    private static AlunoController receiver = null;
    private static Rigidbody2D rBody;
    private static Cola instance;
    private static int velocidadeCola = 15;
    private static Vector2 direction;

    //#if UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID
    private static Vector2 touchOrigin = new Vector2(-1, -1);
    private static Vector2 touchEnd = new Vector2(-1, -1);
    private static bool clicked = false;
    //#endif
    private const int LEFT_CLICK = 0;
    private const int RIGHT_CLICK = 1;
    private const int MIDDLE_CLICK = 2;
    // Use this for initialization
    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        instance = GameObject.FindGameObjectWithTag("Cola").GetComponent<Cola>();
    }
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (!AlunoController.clicked)
        {
            Debug.Log("Sem click no personagem");
            GetInputDirection();
        }
        else
        {
            Debug.Log("Aluno clicado: " + AlunoController.clicked.position);
        }
        if (CanThrow())
        {
            MoveTo(AlunoController.GetAluno(new Vector2Int((int)(shooter.position.x + direction.y),
                                                           (int)(shooter.position.y + direction.x))));
        }
	}

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Aluno") )
        {
            if (collider.GetComponent<AlunoController>() == receiver)
            {
                AlunoController al = collider.GetComponent<AlunoController>();
                al.RecebeCola();
                shooter = al;
                receiver = null;
                rBody.velocity = Vector3.zero;
                transform.position = al.transform.position;
            }
        }
    }
    
    public static void SetPosition(Vector2 position)
    {
        instance.transform.position = position;
    }
    public static void SetShooter(AlunoController aluno)
    {
        shooter = aluno;
        instance.transform.position = aluno.transform.position;
    }
    public static void MoveTo(AlunoController aluno)
    {
        receiver = aluno;
        if (receiver != null) {
            Vector3 direction = receiver.transform.position - shooter.transform.position;
            direction = Vector3.Normalize(direction);
            int reducer = 0;
            if(direction.x != 0 && direction.y != 0) reducer = 5;
            rBody.velocity = new Vector2(direction.x * (velocidadeCola - reducer), direction.y * (velocidadeCola-reducer));
            shooter.PassaCola(direction);
        }
    }
    
    public static AlunoController GetShooter()
    {
        return shooter;
    }
    public static AlunoController GetReceiver()
    {
        return receiver;
    }
    public static Vector3 GetPosition()
    {
        return instance.transform.position;
    }

    private static void GetInputDirection()
    {
        if (AlunoController.clicked)
        {
            Debug.Log("Aluno Clicado");
        }
        direction = Vector2.zero;
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
        //return direction;
    }

    public bool CanThrow()
    {
        // position(i==x, j==y), linha e coluna
        //direction(x, y), direção horizontal ou vertical
        //coluna move com x
        //linha move com y
        bool canThrow = false;
        bool cima = direction.y == 1;
        bool baixo = direction.y == -1;
        bool esquerda = direction.x == -1;
        bool direita = direction.x == 1;

        if (shooter.position.x == 0)
        {
            //Debug.Log("Sou de baixo, pode ir pra cima");
            canThrow |= cima;
        }
        else
        {
            //Debug.Log("Nao sou de baixo, pode ir pra baixo");
            canThrow |= baixo;
            if (shooter.position.x != 3)
            {
                //Debug.Log("Nao sou de cima, pode ir pra cima");
                canThrow |= cima;
            }
        }

        if (shooter.position.y == 0)
        {
            //Debug.Log("Sou de esquerda, pode ir pra direita");
            canThrow |= direita;
        }
        else
        {
            //Debug.Log("Nao sou de esquerda, pode ir pra esquerda");
            canThrow |= esquerda;
            if (shooter.position.y != 4)
            {
                //Debug.Log("Nao sou de direita, pode ir pra direita");
                canThrow |= direita;
            }
        }

        if (canThrow)
        {
            AlunoController receiver = AlunoController.GetAluno(new Vector2Int((int)(shooter.position.x + direction.y), 
                                                                               (int)(shooter.position.y + direction.x)));
            if (receiver.busted)
            {
                canThrow = false;
            }
        }
        return canThrow;
    }
}
