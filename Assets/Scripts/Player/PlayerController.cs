using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Scale")]
    [SerializeField] private float normalScale = 10f;
    [SerializeField] private float smallScale = 5f;
    [SerializeField] private float scaleTransitionSpeed = 7.5f;

    // Referencias a componentes
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isSmall;
    private float currentScale;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentScale = normalScale;
    }

    private void Start()
    {
        // Forzar escala al inicio
        transform.localScale = new Vector3(normalScale, normalScale, normalScale);

        // Verificar componentes necesarios
        if (groundCheck == null)
        {
            Debug.LogError("�GroundCheck no asignado al PlayerController!");
        }

        if (animator == null)
        {
            Debug.LogWarning("No se encontr� componente Animator en el Player");
        }
        else
        {
            // Verificar par�metros del Animator
            Debug.Log("Animator tiene par�metro Speed: " + HasParameter("Speed"));
            Debug.Log("Animator tiene par�metro Jump: " + HasParameter("Jump"));
            Debug.Log("Animator tiene par�metro IsGrounded: " + HasParameter("IsGrounded"));
        }
    }

    private void Update()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
        HandleScale();
    }

    private void CheckGround()
    {
        // Asegurarse de que groundCheck existe
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

            // Actualizar el par�metro en el Animator si existe
            if (animator != null && HasParameter("IsGrounded"))
            {
                animator.SetBool("IsGrounded", isGrounded);
            }

            // Debug para verificar detecci�n del suelo
            Debug.DrawRay(groundCheck.position, Vector2.down * 0.3f, isGrounded ? Color.green : Color.red);
        }
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Actualizar animaciones si existe el Animator
        if (animator != null && HasParameter("Speed"))
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }

        // Voltear el sprite seg�n la direcci�n
        if (moveInput > 0)
            spriteRenderer.flipX = false;
        else if (moveInput < 0)
            spriteRenderer.flipX = true;
    }

    private void HandleJump()
    {
        // Al principio del m�todo HandleJump
        Debug.Log("Bot�n Jump presionado: " + Input.GetButtonDown("Jump"));
        Debug.Log("isGrounded: " + isGrounded);
        // Salto con verificaci�n de suelo
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Aplicar fuerza de salto
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            // A�adir efecto de salto
            if (animator != null && HasParameter("Jump"))
            {
                animator.SetTrigger("Jump");
            }

            // Opcional: Reproducir sonido de salto
            // AudioSource.PlayClipAtPoint(jumpSound, transform.position);

            Debug.Log("�Salto!");
        }
    }

    private void HandleScale()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isSmall = !isSmall;
            Debug.Log("Cambio de tama�o: " + (isSmall ? "Peque�o" : "Normal"));
        }

        float targetScale = isSmall ? smallScale : normalScale;
        currentScale = Mathf.Lerp(currentScale, targetScale, scaleTransitionSpeed * Time.deltaTime);
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        // Ajustar f�sica seg�n el tama�o
        moveSpeed = isSmall ? 7f : 5f;

        // Opcional: Ajustar altura de salto seg�n tama�o
        jumpForce = isSmall ? 15f : 14f;
    }

    // Funci�n auxiliar para verificar si un par�metro existe en el Animator
    private bool HasParameter(string paramName)
    {
        if (animator == null)
            return false;

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}