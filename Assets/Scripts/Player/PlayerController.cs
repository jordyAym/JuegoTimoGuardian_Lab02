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

    [Header("L�mites de movimiento")]
    [SerializeField] private bool useBoundaries = true;
    [SerializeField] private float leftBoundary = -10f;
    [SerializeField] private float rightBoundary = 10f;

    // Referencias a componentes
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isSmall;
    private float currentScale;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentScale = normalScale;
        mainCamera = Camera.main;
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
        EnforceBoundaries();
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

    // M�todo para forzar el cambio de tama�o (utilizado por interruptores)
    public void ForceChangeSize(bool small)
    {
        isSmall = small;
        Debug.Log("Tama�o forzado: " + (isSmall ? "Peque�o" : "Normal"));
    }

    // M�todo para mantener al jugador dentro de los l�mites definidos
    private void EnforceBoundaries()
    {
        if (!useBoundaries) return;

        // Opci�n 1: L�mites fijos configurados en el inspector
        if (mainCamera == null)
        {
            // L�mites absolutos
            float clampedX = Mathf.Clamp(transform.position.x, leftBoundary, rightBoundary);
            if (transform.position.x != clampedX)
            {
                transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
                rb.velocity = new Vector2(0, rb.velocity.y); // Detener movimiento horizontal al tocar l�mite
            }
        }
        else
        {
            // Opci�n 2: L�mites relativos a la c�mara
            float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
            float leftLimit = mainCamera.transform.position.x - cameraHalfWidth + 0.5f; // Margen de 0.5 unidades
            float rightLimit = mainCamera.transform.position.x + cameraHalfWidth - 0.5f;

            float clampedX = Mathf.Clamp(transform.position.x, leftLimit, rightLimit);
            if (transform.position.x != clampedX)
            {
                transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
                rb.velocity = new Vector2(0, rb.velocity.y); // Detener movimiento horizontal al tocar l�mite
            }
        }
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

        if (useBoundaries)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(leftBoundary, transform.position.y - 2, 0),
                          new Vector3(leftBoundary, transform.position.y + 2, 0));
            Gizmos.DrawLine(new Vector3(rightBoundary, transform.position.y - 2, 0),
                          new Vector3(rightBoundary, transform.position.y + 2, 0));
        }
    }
}