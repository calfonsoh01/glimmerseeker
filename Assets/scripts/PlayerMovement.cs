using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;      // velocidad al caminar
    public float jumpForce = 8f;  // fuerza del salto

    private Rigidbody2D rb;
    private bool onGround;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Leer las teclas: flechas o A/D
        float move = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) move = -1f;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) move = 1f;

        // 2. Moverse a izquierda o derecha
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        
            // Animación: corre si se mueve, quieta si no
        anim.SetBool("isRunning", move != 0);

        // 3. Saltar con la barra espaciadora (solo si está en el suelo)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && onGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 4. Girar el dibujo hacia donde camina
        if (move != 0)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * move;
            transform.localScale = s;
        }
    }

    // Detectar si toca el suelo
    void OnCollisionEnter2D(Collision2D other) { onGround = true; }
    void OnCollisionExit2D(Collision2D other)  { onGround = false; }
}