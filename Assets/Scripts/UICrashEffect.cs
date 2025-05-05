using UnityEngine;

public class UiCrashEffect : MonoBehaviour
{
    public float scaleInTime = 0.3f;
    public float slideDownTime = 1.5f;
    public float stayTime = 1f;
    public float fallDistance = 300f;

    private RectTransform rect;
    private Vector3 originalPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
        rect.localScale = Vector3.zero;

        StartCoroutine(PlayCrashEffect());
    }

    private System.Collections.IEnumerator PlayCrashEffect()
    {
        // Efecto de acercamiento
        float timer = 0f;
        while (timer < scaleInTime)
        {
            float t = timer / scaleInTime;
            rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            timer += Time.deltaTime;
            yield return null;
        }
        rect.localScale = Vector3.one;

        // Esperar
        yield return new WaitForSeconds(stayTime);

        // Deslizar hacia abajo
        Vector2 targetPos = (Vector2)originalPos - new Vector2(0, fallDistance);
        timer = 0f;
        while (timer < slideDownTime)
        {
            float t = timer / slideDownTime;
            rect.anchoredPosition = Vector2.Lerp(originalPos, targetPos, t);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
