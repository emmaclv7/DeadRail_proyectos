using System.Collections;
using UnityEngine;

public class TrainSpawner : MonoBehaviour
{
    public float timer = 0f; //el timer para escalar el objeto
    public float growTime = 4f; //lo que tarda en aparecer el tren
    public float maxSize = 6f; //la medida máxima en la que crece el tren
    public bool isMaxSize = false; //si el tren ha llegado al máximo tamaño
    public Sprite[] trainSprites; // array de sprites para elegir el tren
    public GameObject impactEffectPrefab;
    public GameObject crashUIEffectPrefab;
    public Canvas uiCanvas;

    public float visibleTime = 60f; //tiempo en el que el tren está quieto
    public float maxInterval = 120f; //máximo intervalo en el que el tren puede aparecer
    public float minInterval = 60f; //mínimo intervalo en el que el tren puede aparecer

    public GameOverScript gameOverManager; //llama a la clase que maneja la interfaz del game over

    private SpriteRenderer spriteRenderer;

    [SerializeField] private BoxCollider2D triggerCollider;
    [SerializeField] private BoxCollider2D solidCollider;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //para poder ejecutar el movimiento del tren sin bloquear el juego
        transform.localScale = Vector3.zero;//para asegurar que empieza invisible

        spriteRenderer = GetComponent<SpriteRenderer>();

        triggerCollider.enabled = false;
        solidCollider.enabled = false;

        StartCoroutine(TrainAppearece());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (impactEffectPrefab != null)
            {
                GameObject impactEffect = Instantiate(impactEffectPrefab, collision.transform.position, Quaternion.identity);
            }
            Destroy(collision.gameObject);
            gameOverManager.gameOver(); //sale la interfaz game over
        }
    }
    private IEnumerator TrainAppearece()
    {
        //bucle infinito
        while (true)
        {
            timer = 0f;

            float waitingTime = Random.Range(minInterval, maxInterval); //genara un numero aleatorio entre los intervalos
            yield return new WaitForSeconds(waitingTime);

            if (trainSprites.Length > 0)
            {
                int index = Random.Range(0, trainSprites.Length);
                spriteRenderer.sprite = trainSprites[index];
            }

            Vector2 initialSize = Vector3.zero; //cuánto mide inicialmente
            Vector2 maximumSize = new Vector3(maxSize, maxSize, maxSize); //cuánto va a crecer

            timer = 0f;
            while (timer < growTime)
            {
                //mientras el timer no llegue al growTime, el tren no para de crecer
                transform.localScale = Vector3.Lerp(initialSize, maximumSize, timer / growTime);

                if (!triggerCollider.enabled && timer > growTime * 0.75f)
                {
                    triggerCollider.enabled = true; // activar colisión
                    Debug.Log("activado");
                }

                timer += Time.deltaTime; //incremento timer
                yield return null;
            }

            transform.localScale = maximumSize;
            isMaxSize = true; //ahora ya ha llegado a su máximo tamaño
            triggerCollider.enabled = false;
            solidCollider.enabled = true;

            yield return new WaitForSeconds(visibleTime); //espera 1 minuto 

            timer = 0f;
            while (timer < growTime)
            {
                //ahora pasa del tamaño máximo a 0
                transform.localScale = Vector3.Lerp(maximumSize, initialSize, timer / growTime);
                solidCollider.enabled = false;
                timer += Time.deltaTime;
                yield return null;
            }

            transform.localScale = Vector3.zero;
            isMaxSize = false;
        }
       
    }
}
