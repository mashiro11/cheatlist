using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cola : MonoBehaviour {

    private static AlunoController shooter;
    private static AlunoController receiver = null;
    private static Rigidbody2D rBody;
    private static Cola instance;
    private static int velocidadeCola = 15;
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
		//oi
	}

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Aluno") )
        {
            if (collider.GetComponent<AlunoController>() == receiver)
            {
                collider.GetComponent<AlunoController>().RecebeCola();
            }
        }
    }
    public static void SetVelocity(Vector2 velocity)
    {
        rBody.velocity = velocity;
    }
    public static void SetPosition(Vector2 position)
    {
        instance.transform.position = position;
    }
    public static void SetShooter(AlunoController aluno)
    {
        shooter = aluno;
    }
    public static void SetReceiver(AlunoController aluno)
    {
        receiver = aluno;
        if (receiver != null) {
            Debug.Log("receiver.position: " + receiver.position);
            Vector3 direction = receiver.transform.position - shooter.transform.position;
            direction = Vector3.Normalize(direction);
            Debug.Log("direction: " + direction);
            int reducer = 0;
            if(direction.x != 0 && direction.y != 0) reducer = 5;
            rBody.velocity = new Vector2(direction.x * (velocidadeCola - reducer), direction.y * (velocidadeCola-reducer));
            shooter.PassaCola(receiver);
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
}
