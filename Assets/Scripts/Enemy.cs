using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage = 20; //Puntos que se restan al jugador cuando toca a un enemigo
    
    private Animator animator;
    private Transform target; //dice la posicion del jugador y nos indica hacia donde se tiene q mover el enemigo
    private bool skipMove; //para hacer que el enemigo se mueva cada dos turnos
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;


    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target=GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove <T> (xDir, yDir);

        skipMove=true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1: -1; //Asigna valor de menos 1 si es mayor el transform o de 1 si es menor
        else 
            xDir = target.position.x > transform.position.x ? 1:-1;

        AttemptMove <Player> (xDir, yDir);
    }

    protected override void OnCantMove <T> (T component)
    {
        Player hitPlayer = component as Player;
        hitPlayer.LooseFood(playerDamage);
        animator.SetTrigger("EnemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1,enemyAttack2);


    }

}
