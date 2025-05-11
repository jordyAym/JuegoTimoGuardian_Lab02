using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);
    [SerializeField] private float lookAheadFactor = 3f;

    // L�mites de la c�mara (para que no se salga del nivel)
    [Header("L�mites de la c�mara")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private float leftLimit = -10f;
    [SerializeField] private float rightLimit = 10f;
    [SerializeField] private float bottomLimit = -5f;
    [SerializeField] private float topLimit = 5f;

    private Vector3 velocity = Vector3.zero;
    private PlayerController playerController;

    private void Start()
    {
        if (target == null)
        {
            // Buscar autom�ticamente al jugador si no est� asignado
            target = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (target == null)
            {
                Debug.LogError("�No se encontr� el objetivo para la c�mara! Aseg�rate de que el jugador tiene el tag 'Player'");
                return;
            }
        }

        playerController = target.GetComponent<PlayerController>();

        Debug.Log("CameraFollow inicializado con objetivo: " + target.name);
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Calculamos la direcci�n del movimiento para mirar adelante
        float targetDirectionX = 0f;
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            targetDirectionX = Mathf.Sign(rb.velocity.x);
        }
        float lookAheadX = targetDirectionX * lookAheadFactor;

        // Posici�n deseada con offset y mirada adelante
        Vector3 desiredPosition = target.position + offset + new Vector3(lookAheadX, 0, 0);

        // Aplicamos suavizado al movimiento
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        // Aplicamos l�mites de c�mara si est�n activados
        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, leftLimit, rightLimit);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, bottomLimit, topLimit);
        }

        // Actualizamos la posici�n de la c�mara
        transform.position = smoothedPosition;
    }

    // M�todo para dibujar los l�mites en el editor
    private void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(leftLimit, bottomLimit, 0), new Vector3(leftLimit, topLimit, 0));
            Gizmos.DrawLine(new Vector3(rightLimit, bottomLimit, 0), new Vector3(rightLimit, topLimit, 0));
            Gizmos.DrawLine(new Vector3(leftLimit, topLimit, 0), new Vector3(rightLimit, topLimit, 0));
            Gizmos.DrawLine(new Vector3(leftLimit, bottomLimit, 0), new Vector3(rightLimit, bottomLimit, 0));
        }
    }
}
