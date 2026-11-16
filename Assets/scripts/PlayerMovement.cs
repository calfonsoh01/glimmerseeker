using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;       // velocidad al correr
    public float jumpForce = 25f;  // fuerza del salto

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    private bool onGround;         // ¿está tocando el suelo?
    private bool canDoubleJump;    // ¿le queda el segundo salto?
    private bool doubleJumping;    // ¿está haciendo la voltereta?
    private bool dead;             // ¿ha muerto?
    private string currentAnim;    // animación que se está viendo ahora

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Si ha muerto, ya no se puede controlar
        if (dead) return;

        // 1. Leer las teclas: flechas o A/D
        float move = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) move = -1f;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) move = 1f;

        // 2. Moverse a izquierda o derecha
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // 3. Saltar con la barra espaciadora
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (onGround)
            {
                // Salto normal, desde el suelo
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (canDoubleJump)
            {
                // Doble salto, en el aire (solo una vez)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = false;
                doubleJumping = true;
            }
        }

        // 4. Girar el dibujo hacia donde camina
        if (move != 0)
        {
            sprite.flipX = move < 0;
        }

        // 5. Elegir la animación según lo que está haciendo
        if (!onGround)
        {
            if (doubleJumping) SetAnimation("DoubleJump");            // voltereta
            else if (rb.linearVelocity.y > 0.1f) SetAnimation("Jump"); // subiendo
            else SetAnimation("Fall");                                 // cayendo
        }
        else if (move != 0) SetAnimation("Run");                       // corriendo
        else SetAnimation("Idle");                                     // quieta
    }

    // Cambia de animación solo si es distinta de la actual
    void SetAnimation(string name)
    {
        if (currentAnim == name) return;
        anim.Play(name);
        currentAnim = name;
    }

    // Muerte: no tiene tecla. La llamarán los enemigos más adelante.
    public void Die()
    {
        if (dead) return;
        dead = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        SetAnimation("Die");
    }

    // Detectar si toca el suelo
    void OnCollisionEnter2D(Collision2D other)
    {
        onGround = true;
        canDoubleJump = true; // al tocar el suelo recupera el doble salto
        doubleJumping = false;
    }

    void OnCollisionExit2D(Collision2D other) { onGround = false; }
}
