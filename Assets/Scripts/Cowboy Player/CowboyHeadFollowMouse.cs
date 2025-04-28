using UnityEngine;

public class CowboyHeadFollowMouse : MonoBehaviour
{
    private Vector3 mousePosition; //vector posicion del ratón
    private Vector3 direction; //vector de la direccion
    private Vector3 initialScale; //vector de la escala (necesaria para flipear el asset de la cabeza)
    private SpriteRenderer spriteRenderer; //la referencia al componente sprite renderer

    public Transform bodyTransform; //transform del parent cuerpo
    public float rotationOffset = 0f; //offset para calcular el angulo de rotación de la cabeza

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        initialScale = transform.localScale;
    }

    void Update()
    {
        //Obtener toda la dirección de donde esta el mouse 
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - transform.position;
        direction.z = 0;

        //booleano para comprobar si el cuerpo esta flipeado (+1 derecha / -1 izquierda) 
        bool bodyFlipped = bodyTransform.localScale.x < 0f;

        if (bodyFlipped) //Sistema hardcodeado porque hay un bug que los controles se invierten cuando el cuerpo esta flipeado
        {
            direction.x = -direction.x;
            direction.y = -direction.y;
        }

        //calcular angulo de la cabeza
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //aplicar la rotacion del angulo
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);


        //flipear la cabez adependiendo de si esta en el eje Y del mouse y del cuerpo
        if ((mousePosition.x < transform.position.x && !bodyFlipped) ||
            (mousePosition.x > transform.position.x && bodyFlipped))
        {
            transform.localScale = new Vector3(initialScale.x, -Mathf.Abs(initialScale.y), initialScale.z);
        }
        else
        {
            transform.localScale = new Vector3(initialScale.x, Mathf.Abs(initialScale.y), initialScale.z);
        }
    }
}

