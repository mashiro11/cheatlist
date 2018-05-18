using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataCollector : MonoBehaviour {

    public static float tempoDeJogo = 0;
    public static int alunosFinalizados = 0;//quantidade de alunos que terminaram ao termino do jogo
    public static int alunosNaoFinalizados = 0;//quantidade de alunos que não terminaram
    public static int alunosPegos = 0;//quantidade de alunos que foram pegos
    public static int alunosAtendidos = 0;
    public static int alunosNaoAtendidos = 0;
    public static int transmissao = 0;//quantidade de movimentos realizados pelo jogador
    public static Vector2 posicaoProfessor = Vector2.zero;
    public static Vector2 posicaoCola = Vector2.zero;
    public static string outputPath;
    public static string directories;
    public static string fileName;
    public static bool started = false;
    private void Awake()
    {
       
    }

    // Use this for initialization
    void Start () {

	}

    public static void StartDataCollection()
    {
        started = true;
        outputPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Dropbox\Dados Experimentos\Experimentos\";
        Debug.Log("outputPath: " + outputPath);
        string text = "\"TIME\", \"GAME_TIME\", \"FINISHED_STUDENTS\", \"NOT_FINISHED_STUDENTS\", \"ATENDED_STUDENT\", \"NOT_ATENDED_STUDENT\", \"BUSTEDS\", \"TRANSMISSIONS\", \"TEACHER_POSITION_X\", \"TEACHER_POSITION_Y\", \"CHEAT_POSITION_X\", \"CHEAT_POSITION\"";
        Debug.Log("HEADER: " + text);
        string[] lines = System.IO.File.ReadAllLines(outputPath + "Time.txt");
        fileName = "gameData_" + lines[0] + ".csv";
        Debug.Log("FileName: " + fileName);

        lines = System.IO.File.ReadAllLines(outputPath + "LastPath.txt");
        directories = lines[0];
        
        System.IO.File.WriteAllText(outputPath + directories + fileName, text + '\n');
        Debug.Log("LastPath: " + outputPath);
        System.IO.File.WriteAllText(outputPath + directories + "videoStart.txt", DateTime.Now.ToString("HHmmss"));
    }

    private void LateUpdate()
    {
        if (!started) return;

        string line = DateTime.Now.ToString("HH:mm:ss:fff") + ", " + 
                      tempoDeJogo.ToString("0.0") + ", " +
                      alunosFinalizados + ", " +
                      alunosNaoFinalizados + ", " +
                      alunosAtendidos + ", " +
                      alunosNaoAtendidos + ", " +
                      alunosPegos + ", " + 
                      transmissao + ", " +
                      posicaoProfessor.x + ", " +
                      posicaoProfessor.y + ", " +
                      posicaoCola.x + ", " +
                      posicaoCola.y;

        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(outputPath + directories + fileName, true))
        {
            file.WriteLine(line);
        }
    }
}
