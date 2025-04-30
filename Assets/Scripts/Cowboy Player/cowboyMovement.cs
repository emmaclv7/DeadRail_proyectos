using System;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering;

public class cowboyMovement : MonoBehaviour
{
    private float horizontalMovement;
    private float speed = 3f;
    private float jumpingPower = 4f;
    private bool isFacingRight = true; //para que el sprite se gire cuando cuambio de direcci�n

    //para editar las variables privadas en el inspector de unity
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // Update is called once per frame
    void Update()
    {
        
        horizontalMovement = Input.GetAxisRaw("Horizontal");

        //si presionamos w y está en el suelo salta
        if (Input.GetKeyDown(KeyCode.W) && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        Flip();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);
    }
    private bool IsGrounded()
    {
        //para saber si el personaje está tocando el suelo
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        //para que el jugador se gire cuando se cambia de dirección
        if (isFacingRight && horizontalMovement < 0f || !isFacingRight && horizontalMovement > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f; //para invertir la escala en el eje x
            transform.localScale = localScale;
        }
    }
}
