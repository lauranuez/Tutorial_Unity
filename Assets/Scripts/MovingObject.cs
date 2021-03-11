using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer; //Capa donde detectara las colisiones

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D; //Almacenar la referencia del objeto que se mueve
    private float inverseMoveTime; //para hacer los calculos mas eficientes
    private bool isMoving;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D=GetComponent<Rigidbody2D>();
        inverseMoveTime=1f/moveTime;
    }

    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;  //transform.position = vector3 -> lo pasamos a vector2 para eliminar la z
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit= Physics2D.Linecast (start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null && !isMoving ) //mira si hay algun obstaculo que le impida moverse
        {
            StartCoroutine(SmoothMovement(end)); //si no hay obstacculo utiliza la funcion SmoothMovement para moverse
            return true; //Indicar que se mueve
        }

        return false; //Hay obstaculo -> no se puede mover

    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        isMoving = true;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon) //float.Epsilon= nº muy pequeño casi 0
        {
            Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime*Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null; //Esperar una frame antes de recalcular el loop
        }

        rb2D.MovePosition (end);
        isMoving = false;
    }

    protected virtual void AttemptMove <T> (int xDir, int yDir) //Tipo de objeto que va a interactuar conmigo si estoy bloqueado
        where T: Component //Especificamos que T es una componente
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);

    }

    protected abstract void OnCantMove <T> (T component)
        where T: Component;
    
}
