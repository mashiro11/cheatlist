using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cola : MonoBehaviour {

    private static AlunoController shooter = null;
    private static AlunoController receiver = null;
    private static Rigidbody2D rBody;
    private static Cola instance;
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
            if (GetShooter() != this)
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
