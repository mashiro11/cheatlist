﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlunoController : MonoBehaviour {

    /*
     *      Variáveis estáticas
     */
    //Inicialização das posições
    public static float initialX = -8, initialY = -4.5f;
    public static float espacamentoX = 4, espacamentoY = 2.1f;
    public static int maxLinhas = 4;
    public static int maxColunas = 5;
    public static Vector2 inicial = new Vector2(1, 3); //posição de quem começa com a cola

    private static GameObject[,] alunos = new GameObject[maxLinhas, maxColunas];
    private static float[,] cheatProgress = new float[maxLinhas, maxColunas];

    //Inicialização dos contadores para regras de jogo
    public static int bustedCounter = 0;
    private static int finishedCounter = 0;
    public static int chances = 5;
    private static int minToWin = 10;

    //Impede jogador de continuar movimentando após fim de jogo
    private static bool stopControls = false;

    //utilizado para saber qual tipo de arremesso será executado
    public static AlunoController clicked = null;

    /*
     *  Referências por objeto 
     */
    enum AlunoChild
    {
        Slingshooter = 0,
        ProgressoCola,
        Shadow
    }
    private Camera cam;
    private GameObject progressoCola;
    private AudioSource aSource;
    private Animator animator;
    private LineRenderer outline;
    private Slingshot slingshot;
    private Text meanLabel;

    //MetaInfo
    public Vector2 position;
    public bool draggin = false;
    public float maxDragging;
    public float maxDistance;
    float colaRapida = 1;

    float tempoMinimo;//timer
    public float tempoMinimoDefinido;//time
    public static float tempoNecessario = 10;//totalTime

    public Sprite[] sprites;
    enum AlunoSounds
    {
        PassaCola = 0,
        Busted,
        Colando,
        Receive
    }
    public AudioClip[] sounds;

    public enum AlunoStates
    {
        IDLE,
        CHEATING,
        ASKING,
        BUSTED,
        FINISHED
    }
    public AlunoStates alState = AlunoStates.IDLE;
    //Infos do objeto
    public int tipoAluno = 0;
    public bool busted = false;
    public bool terminou = false;
    private static bool bonusSpeed;
    public static bool colaNasCostas;
    public static AlunoController needsCheat = null;
    public static float needsCheatTime = 5;
    public static float needsCheatTimer = needsCheatTime;
    public bool dedoDuro;

    private string debugTag;

    void Awake () {
        tipoAluno = Random.Range(0,2);
        if(tipoAluno == 2) tipoAluno = 1;
        
        aSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        slingshot = GetComponentInChildren<Slingshot>();
        outline = GetComponent<LineRenderer>();
        progressoCola = transform.GetChild((int)AlunoChild.ProgressoCola).gameObject;
        meanLabel = GameObject.Find("ClassMeanLabel").GetComponent<Text>();

        animator.SetBool("temCola", false);
    }

    private void Start()
    {
        string path = "AnimationControllerOverride/";
        animator.runtimeAnimatorController = Resources.Load(path + "Aluno" + Random.Range(1, 7)) as RuntimeAnimatorController;
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
        if (GameManager.IsGameOver()) return;

        switch (alState)
        {
            case AlunoStates.IDLE:
                TryAskingForCheat();
                break;
            case AlunoStates.CHEATING:
                if (animator.GetFloat("tempoComCola") < tempoNecessario)
                {
                    if (!aSource.isPlaying)
                    {
                        aSource.clip = sounds[(int)AlunoSounds.Colando];
                        aSource.Play();
                    }
                    MostraProgressoCola();
                }
                if (tempoMinimo > 0)
                {
                    tempoMinimo -= Time.deltaTime;
                }
                break;
            case AlunoStates.ASKING:
                needsCheatTimer -= Time.deltaTime;
                if (needsCheatTimer < 0)
                {
                    DataCollector.alunosNaoAtendidos++;
                    NeedsCheat(false);
                    alState = AlunoStates.IDLE;
                }
                break;
            case AlunoStates.BUSTED:
                break;
            case AlunoStates.FINISHED:
                break;
        }
    }

    public void PassaCola(Vector3 direction)
    {
        if (direction.x < 0) {
            animator.SetInteger("direcao", 1);
            if (!animator.GetBool("olhandoEsquerda")) Flip();
        }else if (direction.x > 0)
        {
            animator.SetInteger("direcao", 2);
            if (animator.GetBool("olhandoEsquerda")) Flip();
        }

        if (direction.y > 0) animator.SetInteger("direcao", 3);
        else if(direction.y < 0) animator.SetInteger("direcao", 4);

        if (terminou) alState = AlunoStates.FINISHED;
        else          alState = AlunoStates.IDLE;

        animator.SetTrigger("passandoACola");
        animator.SetBool("temCola", false);
        aSource.clip = sounds[(int)AlunoSounds.PassaCola];
        aSource.Play();
        progressoCola.SetActive(false);
        colaNasCostas = false;
        bonusSpeed = false;
        meanLabel.text = "Media da turma: " + GetMean().ToString("0.0");
    }

    public void MostraProgressoCola()
    {
        if (bonusSpeed || colaNasCostas)
        {
            colaRapida = 3;
        }
        else
        {
            colaRapida = 1;
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
        DataCollector.alunosFinalizados = finishedCounter;
        DataCollector.alunosNaoFinalizados = 20 - finishedCounter - bustedCounter; 
    } 

    public void RecebeCola ()
    {
        alState = AlunoStates.CHEATING;
        animator.SetBool("temCola", true);
        tempoMinimo = tempoMinimoDefinido;
        progressoCola.SetActive(true);
        if(needsCheat == this)
        {
            aSource.clip = sounds[(int)AlunoSounds.Receive];
            aSource.Play();
            DataCollector.alunosAtendidos++;
            bonusSpeed |= true;
            NeedsCheat(false);
        }
    }

    public void Busted()
    {
        DataCollector.alunosPegos++;
        //Se chegou no limite de alunos que podem ser pegos, game over
        //Debug.Log(debugTag + "Busted: " + bustedCounter + "| Chances: " + chances);
        slingshot.Released();
        if (++bustedCounter >= chances)
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

            //O que o aluno tinha de pontos não soma mais na média da turma
            cheatProgress[(int)position.x, (int)position.y] = 0;
            
            //se ele tinha terminado, é desconsiderado
            if (terminou)
            {
                --finishedCounter;
            }

            alState = AlunoStates.BUSTED;
            GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
            busted = true;
            tempoMinimo = 0;
            aSource.Stop();
            aSource.clip = sounds[(int)AlunoSounds.Busted];
            aSource.Play();
            animator.SetBool("temCola", false);
            animator.SetTrigger("busted");
            progressoCola.SetActive(false);
            meanLabel.text = "Media da turma: " + GetMean().ToString("0.0");
        }
    }

    public void Flip()
    {
        animator.SetBool("olhandoEsquerda", !animator.GetBool("olhandoEsquerda"));
        Vector3 flipper = transform.localScale;
        flipper.x *= -1;
        transform.localScale = flipper;
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
        Object aluno = Resources.Load("Aluno");
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
                al.transform.GetChild((int)AlunoChild.Shadow).GetComponent<SpriteRenderer>().sortingLayerName = "Fileira" + i;
                al.transform.GetChild((int)AlunoChild.Shadow).GetComponent<SpriteRenderer>().sortingOrder = 0;
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

    public void SetOutline(bool outlined)
    {
        outline.enabled = outlined;
    }

    public static AlunoController GetAluno (Vector2Int position)
    {
        return alunos[position.x, position.y].GetComponent<AlunoController>();
    }

    private void OnMouseDown()
    {
        if (alState == AlunoStates.CHEATING)
        {
            slingshot.Clicked();
            clicked = this;
        }
    }

    private void OnMouseDrag()
    {
        if(alState == AlunoStates.CHEATING) slingshot.Drag();
    }

    private void OnMouseUp()
    {
        if (alState == AlunoStates.CHEATING)
        {
            slingshot.Released();
            clicked = null;
        }
    }

    public static void GenerateCheatOnFarest(Vector3 position)
    {
        //de inicio, chuto que o ponto mais distante é onde estou
        Vector2Int index = Vector2Int.zero;
        for (int i = 0; i < maxLinhas; i++)
        {
            for (int j = 0; j < maxColunas; j++)
            {
                if ((alunos[i, j].transform.position - position).magnitude > (alunos[index.x,index.y].transform.position - position).magnitude &&
                    !alunos[i, j].GetComponent<AlunoController>().busted)
                {
                    index.Set(i, j);
                }
            }
        }
        Cola.SetPosition(alunos[index.x, index.y].transform.position);
        Cola.SetShooter(alunos[index.x, index.y].GetComponent<AlunoController>());
        alunos[index.x, index.y].GetComponent<AlunoController>().RecebeCola();
    }

    public void TryAskingForCheat()
    {
        if(!needsCheat && Random.Range(0, 1f) > 0.95)
        {
            needsCheatTimer = needsCheatTime;
            NeedsCheat(true);
        }
    }

    private void NeedsCheat(bool needs)
    {
        if (needs)
        {
            needsCheat = this;
            //GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
            alState = AlunoStates.ASKING;
            animator.SetBool("requestCheat", true);
        }
        else
        {
            needsCheat = null;
            animator.SetBool("requestCheat", false);
            //GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
    }
}
