using UnityEngine;

public class CowboyHeadFollowMouse : MonoBehaviour
{
    private Vector3 mousePosition; //vector posicion del ratón
    private Vector3 direction; //vector de la direccion
    private Vector3 initialScale; //vector de la escala (necesaria para flipear el asset de la cabeza)
    private SpriteRenderer spriteRenderer; //la referencia al componente sprite renderer

    public Transform bodyTransform; //transform del parent cuerpo
    public float rotationOffset = 0f; //offset para calcular el angulo de rotación de la cabeza
    public FieldOfView fov;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        initialScale = transform.localScale;
        fov.SetOrigin(transform.position);
    }

    void Update()
    {
        // Obtener toda la dirección de donde está el mouse
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - transform.position;
        direction.z = 0;

        // Booleano para comprobar si el cuerpo está flipeado (+1 derecha / -1 izquierda)
        bool bodyFlipped = bodyTransform.localScale.x < 0f;

        // Sistema hardcodeado porque hay un bug que los controles se invierten cuando el cuerpo está flipeado
        if (bodyFlipped)
        {
            direction.x = -direction.x;
            direction.y = -direction.y;
        }

        // Calcular ángulo de la cabeza
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplicar la rotación del ángulo
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);

        // Flipear la cabeza dependiendo de si está en el eje X del mouse y del cuerpo
        if ((mousePosition.x < transform.position.x && !bodyFlipped) ||
            (mousePosition.x > transform.position.x && bodyFlipped))
        {
            transform.localScale = new Vector3(initialScale.x, -Mathf.Abs(initialScale.y), initialScale.z);
        }
        else
        {
            transform.localScale = new Vector3(initialScale.x, Mathf.Abs(initialScale.y), initialScale.z);
        }

        // Actualizar el campo de visión:
        // Se pasa la posición actual de la cabeza como origen
        fov.SetOrigin(transform.position);

        // Se pasa la dirección en la que mira la cabeza usando su rotación real
        // transform.right apunta hacia la derecha local del objeto
        Vector3 headForward = transform.right * (bodyFlipped ? -1f : 1f); // corregimos si el cuerpo está flipeado
        fov.SetAimDirection(headForward);
    }
}

