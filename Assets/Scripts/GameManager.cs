using System.Collections;
using System.Collections.Generic; //Para usar listas
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public float turnDelay = 0.1f; //cuánto tiempo va a esperar el juego entre turnos
    public static GameManager instance = null;
    public BoardManager boardScript;

    public float levelStartDelay = 2f; //Tiempo antes de empezar niveles en segundos

	private int level = 1; //Porque es en el nivel que aparecen los enemigos

    private Text levelText;
    private GameObject levelImage;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup; //Para que los jugadores no se muevan durante el setup

    public int playerFoodPoints=100;
    [HideInInspector] public bool playersTurn = true; // Booleano para comprobar si es el turno de los jugadores





    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);

        enabled=false;
    }

    void Awake()
    {
        if (instance == null)
            instance=this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
    	boardScript = GetComponent<BoardManager>();
    	InitGame();
        
    }

    void OnLevelWasLoaded(int index)
    {
            level++;
            InitGame();  //Call InitGame to initialize our level
    }

    void InitGame()
    {
        doingSetup = true; //El jugador no se puede mover
        

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level; //Cambia el texto segun el nivel en el que estamos
        levelImage.SetActive(true); //Activa la imagen

        Invoke("HideLevelImage", levelStartDelay); //Despues de sacar la imagen del titulo esperaremos dos 2 para empezar a jugar

        enemies.Clear();
    	boardScript.SetupScene(level);

    }

    void HideLevelImage() //Para quitar la LevelImage
    {
        
        levelImage.SetActive(false);
        doingSetup = false; //Ahora puede moverse el muñeco
    }


    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count==0) //Miramos que no haya ningun enemigo creado
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i< enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    //Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine (MoveEnemies ());
    }
}
