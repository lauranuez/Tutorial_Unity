using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerFood = 10; //Puntos que gana cuando coge una fruta
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator; //Referencia el componente animator
    private int food; //almacena la puntacion del jugador

    protected override void Start() //Porque tenemos una implementacion diferente para empezar que la que tenemos en la clase MovingObject 
    {
        animator= GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints; //Ponemos la variable food al valor almacenado en GameManager, guarda el valor durante el nivel, luego lo pasa a GameMmanager cuando cambiamos de nivel
        foodText.text= "Food: " + food;
        base.Start ();
    }

    private void OnDisable() //Almacena en GameManager la puntuacion
    {
        GameManager.instance.playerFoodPoints = food;
    }

    private void CheckIfGameOver() //Si tenemos puntuacion 0 acaba el juego
    {
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
            
    }

    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        food--; //Cada vez que se mueve pierde un punto
        foodText.text= "Food: " + food;
        base.AttemptMove <T> (xDir, yDir);
        RaycastHit2D hit;

        if (Move (xDir, yDir, out hit)) 
        {
                SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver ();
        GameManager.instance.playersTurn=false;

    }
    

    
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn) return; //No tiene que iniciar el siguiente codigo

        int horizontal = 0; //Guardar la direccin en la que nos movemos (1 o -1)
        int vertical = 0; 
        horizontal = (int) Input.GetAxisRaw("Horizontal"); //Recoge la posicion en la que se encuentra
        vertical = (int) Input.GetAxisRaw("Vertical");

        if (horizontal != 0) //Para hacer que no se mueva en didagonal
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall> (horizontal, vertical);
        }
    }

    protected override void OnCantMove <T> (T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart() //Cuando lleg aal exit se genera otro nivel
    {
       SceneManager.LoadScene (0);
    }

    public void LooseFood (int loss) //Cuando un enemigo ataca al jugador
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text= "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke ("Restart", restartLevelDelay);
            enabled = false;

        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text= "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);

        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text= "+" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);

        }
    }
}

