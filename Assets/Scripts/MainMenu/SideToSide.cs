using UnityEngine;

public class SideToSide : MonoBehaviour
{
    public float forceAmount = 10f;       
    public float frequency = 1f;          
    public float maxSpeed = 5f;       

    private Rigidbody2D rb;
    private float initialX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialX = transform.position.x;
    }

    void Update()
    {
        
        float target = Mathf.Sin(Time.time * frequency);
        float current = Mathf.InverseLerp(-1f, 1f, target); 

        
        Vector3 force = new Vector3(target, 0f, 0f) * forceAmount;

        
        if (Mathf.Abs(rb.linearVelocity.x) < maxSpeed)
        {
            rb.AddForce(force, ForceMode2D.Force);
        }
    }
}
