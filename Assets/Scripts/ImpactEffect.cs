using UnityEngine;
using System.Collections;

public class ImpactEffect : MonoBehaviour
{
    [Header("Movement Settings")]
    public float initialDelay = 0.1f; //delay antes de iniciar todo       
    public float flyUpDuration = 0.5f; //Duracion del salto hacia arriba       
    public float flyUpHeight = 3f; //max altura del salto hacia arriba          
    public float stickDuration = 1.4f; //tiempo que se queda "enganchado a la pared"       
    public float initialFallSpeed = 0.05f; //velocidad de caida inicial después de desengancharse de la pared   
    public float finalFallSpeed = 2.0f;  //velocidad máxima de caida     
    public float fallAcceleration = 0.1f; //aceleración de la caida   
    public float arcIntensity = 0.5f;        

    [Header("Scale Settings")]
    public float startScale = 0.1f;  //escala incial del momento de choque        
    public float finalScale = 1.5f;  //escala máxima final de choque        

    [Header("Rotation Settings")]
    public float rotationSpeed = 60f;   //velocidad de rotación     
    public bool randomInitialRotation = true; //si es activado, rota aleatoriamente

    [Header("Screen Position")] 
    public bool randomCrashPosition = true; //booleano de la posición aleatoria  
    [Range(0.05f, 0.45f)]
    public float edgeBuffer = 0.1f; //margen desde los bordes de la pantalla

    [Range(0f, 1f)]
    public float horizontalPosition = 0.5f; //posición incial h si no es aleatoria  
    [Range(0, 1f)]
    public float verticalPosition = 0.5f; //posición incial v si no es aleatoria

    private SpriteRenderer sr; //referencia al spriteRenderer
    private Vector3 initialPosition; //posición inicial
    private Vector3 middle; //posición al centro (es la posición final antes de la caida)
    private Vector3 initialScale; //escala inicial

    void Start()
    {
        sr = GetComponent<SpriteRenderer>(); //obtiene la referencia del sprite

        initialPosition = transform.position; //guarda la posición incial
        initialScale = new Vector3(startScale, startScale, 1); //escala inicial
        transform.localScale = initialScale; //agarra la escala inicial y la aplica

        if (randomInitialRotation) //esta la rotación inicial aleatoria activa?
        {
            float r = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(0, 0, r); //rotacion aleatoria inicialmente
        }

        CalcMiddle(); //metodo que calcula dónde esta la mitad de la pantalla para aterrizar

        StartCoroutine(DoCrashEffect()); //empieza la rutina
    }

    void CalcMiddle()
    {
        Camera camera = Camera.main;
        Vector3 screenPos;

        if (randomCrashPosition) //esta el booleano de la posición aleatoria al chocar activa?
        {
            float minX = edgeBuffer; 
            float maxX = 1f - edgeBuffer;
            float minY = edgeBuffer;
            float maxY = 1f - edgeBuffer;

            //calcular la posición aleatoria en la pantalla respetando los bordes maximos y minimos implementados por el buffer
            float randX = Random.Range(minX, maxX);
            float randY = Random.Range(minY, maxY);

            screenPos = new Vector3(Screen.width * randX, Screen.height * randY, 10);
        }
        else
        {
            //posicion fija desde los valores dados
            screenPos = new Vector3(Screen.width * horizontalPosition, Screen.height * verticalPosition, 10);
        }

        middle = camera.ScreenToWorldPoint(screenPos); //convierte en coordenadas de mundo la posición de la camara
        middle.z = 0f; //aseguremos que esté en el plano 2D
    }

    System.Collections.IEnumerator DoCrashEffect()
    {
        yield return new WaitForSeconds(initialDelay); //La espera inicial (mirar variables)

        float randomAngle = Random.Range(-30f, 30f); //angulo aleatorio para lanzar el cuerpo en una dirección
        Vector3 dir = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;

        float time = 0f;
        Vector3 targetScl = new Vector3(finalScale, finalScale, 1);
        Vector3 startPos = transform.position;

        //calcula la dirección y la altura de vuelo
        Vector3 flyDir = dir * flyUpHeight * 1.2f;
        Vector3 mid = startPos + flyDir;

        //calcula el punto máximo de la parabola
        Vector3 toward = (middle - startPos).normalized;
        toward.y = Mathf.Abs(toward.y);
        Vector3 peak = mid + toward * flyUpHeight * 0.5f;

        while (time < flyUpDuration)//hace el primer vuelo parabólico 
        {
            float t = time / flyUpDuration;
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            //Interpolación de la parabóla
            Vector3 ab = Vector3.Lerp(startPos, mid, smooth);
            Vector3 bc = Vector3.Lerp(mid, peak, smooth);
            Vector3 finalPos = Vector3.Lerp(ab, bc, smooth);
            transform.position = finalPos;

            transform.localScale = Vector3.Lerp(initialScale, targetScl * 0.6f, smooth);

            float rotSpeed = rotationSpeed * (1f - 0.5f * smooth);
            transform.Rotate(0, 0, rotSpeed * Time.deltaTime);

            time += Time.deltaTime;
            yield return null; //yield return null para que no se ejectue todo entero en un solo frame
        }

        transform.position = peak; // nos aseguramos de la posición final
        transform.localScale = targetScl * 0.6f;

        //segundo vuelo parabolico hacia el centro de la pantalla
        time = 0f;
        float dur2 = 0.7f;

        Vector3 arcMid = Vector3.Lerp(peak, middle, 0.5f);
        arcMid.y += Vector3.Distance(peak, middle) * 0.2f; //eleva el punto medio para formar un arco

        while (time < dur2)
        {
            float t = time / dur2;
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            //Interpolación del arco
            Vector3 ap = Vector3.Lerp(peak, arcMid, smooth);
            Vector3 pm = Vector3.Lerp(arcMid, middle, smooth);
            Vector3 arcPos = Vector3.Lerp(ap, pm, smooth);
            transform.position = arcPos;

            transform.localScale = Vector3.Lerp(targetScl * 0.6f, targetScl, smooth);

            float finalRotation = rotationSpeed * 0.5f * (1.0f - smooth);
            transform.Rotate(0, 0, finalRotation * Time.deltaTime);

            time += Time.deltaTime;
            yield return null; //yield return null para que no se ejectue todo entero en un solo frame
        }

        transform.position = middle;

        //se estampa contra la pantalla (es decir se mantiene su posición) por un tiempo y tiembla
        float stickTime = 0f;
        Vector3 originalPosition = middle;
        float shake = 0.05f;
        float shakeSpeed = 15f;

        transform.position = middle;
        transform.localScale = targetScl;

        Quaternion keepRotation = transform.rotation;

        while (stickTime < stickDuration)
        {
            float left = stickDuration - stickTime;
            float currentShakeIntensity = shake * (left / stickDuration);

            if (stickTime > 1f)
            {
                float x = Mathf.Sin(Time.time * shakeSpeed) * currentShakeIntensity;
                float y = Mathf.Cos(Time.time * shakeSpeed * 1.3f) * currentShakeIntensity;
                transform.position = originalPosition + new Vector3(x, y, 0);
            }

            float pulse = 1f + (0.03f * Mathf.Sin(stickTime * 1.5f));
            transform.localScale = targetScl * pulse;

            transform.rotation = keepRotation;

            stickTime += Time.deltaTime;
            yield return null; //yield return null para que no se ejectue todo entero en un solo frame
        }

        //Caida con aceleracíón
        float fallSpeed = initialFallSpeed;
        float rotation = 0f;
        float maxRotation = rotationSpeed * 0.8f;

        while (transform.position.y > -10f)
        {
            fallSpeed = Mathf.Min(fallSpeed + fallAcceleration * Time.deltaTime, finalFallSpeed);

            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            rotation = Mathf.Min(rotation + fallAcceleration * 0.2f * Time.deltaTime, maxRotation);
            transform.Rotate(0, 0, rotation * Time.deltaTime);

            yield return null; //yield return null para que no se ejectue todo entero en un solo frame
        }

        Destroy(gameObject); //elimina el objecto una vez fuera de pantalla
    }
}
