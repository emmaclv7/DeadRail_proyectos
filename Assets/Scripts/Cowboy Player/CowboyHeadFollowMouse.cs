using Unity.VisualScripting;
using UnityEngine;

public class CowboyHeadFollowMouse : MonoBehaviour
{
    private Vector3 mousePosition; //vector posicion del rat�n
    private Vector3 direction; //vector de la direccion
    private Vector3 initialScale; //vector de la escala (necesaria para flipear el asset de la cabeza)
    private SpriteRenderer spriteRenderer; //la referencia al componente sprite renderer
    [SerializeField] private FieldOfView fovCone;
    [SerializeField] private FieldOfView fovArea;// Referencia a la clase FieldOfView para calcular por donde puede ver

    public Transform bodyTransform; //transform del parent cuerpo
    public float rotationOffset = 0f; //offset para calcular el angulo de rotaci�n de la cabeza


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        initialScale = transform.localScale;
        fovCone.SetOrigin(transform.position);
        fovArea.SetOrigin(transform.position);
    }

    void Update()
    {
        // Obtener toda la direcci�n de donde est� el mouse
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - transform.position;
        direction.z = 0;

        // Booleano para comprobar si el cuerpo est� flipeado (+1 derecha / -1 izquierda)
        bool bodyFlipped = bodyTransform.localScale.x < 0f;

        // Sistema hardcodeado porque hay un bug que los controles se invierten cuando el cuerpo est� flipeado
        if (bodyFlipped)
        {
            direction.x = -direction.x;
            direction.y = -direction.y;
        }

        // Calcular �ngulo de la cabeza
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplicar la rotaci�n del �ngulo
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);

        // Flipear la cabeza dependiendo de si est� en el eje X del mouse y del cuerpo
        if ((mousePosition.x < transform.position.x && !bodyFlipped) ||
            (mousePosition.x > transform.position.x && bodyFlipped))
        {
            transform.localScale = new Vector3(initialScale.x, -Mathf.Abs(initialScale.y), initialScale.z);
        }
        else
        {
            transform.localScale = new Vector3(initialScale.x, Mathf.Abs(initialScale.y), initialScale.z);
        }

        // Actualizar el campo de visi�n, se pasa la posici�n actual de la cabeza como origen
        fovCone.SetOrigin(transform.position);
        fovArea.SetOrigin(transform.position);


        // Se pasa la direcci�n en la que mira la cabeza usando su rotaci�n real y corregimos si el cuerpo est� flipeado
        // transform.right apunta hacia la derecha local del objeto
        Vector3 headForward = transform.right * (bodyFlipped ? -1f : 1f);
        fovCone.SetAimDirection(headForward);
        fovArea.SetAimDirection(headForward);
    }
}

