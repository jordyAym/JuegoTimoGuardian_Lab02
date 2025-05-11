using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float parallaxEffect = 0.5f;
    [SerializeField] private bool infiniteHorizontal = true;

    private Camera cam;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float startPositionX;

    private void Start()
    {
        // Usamos Camera.main en lugar de buscar un transform espec�fico
        cam = Camera.main;
        lastCameraPosition = cam.transform.position;
        startPositionX = transform.position.x;

        // Obtenemos el tama�o de la textura para saber cu�ndo reposicionar
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x;

        Debug.Log($"Parallax inicializado en {gameObject.name} con efecto: {parallaxEffect}");
    }

    private void LateUpdate()
    {
        if (cam == null)
        {
            Debug.LogError("�C�mara principal no encontrada!");
            return;
        }

        // Calculamos el desplazamiento de la c�mara desde el �ltimo frame
        Vector3 deltaMovement = cam.transform.position - lastCameraPosition;

        // Movemos el fondo en direcci�n contraria al movimiento de la c�mara, 
        // pero aplicando el efecto parallax
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, 0, 0);
        lastCameraPosition = cam.transform.position;

        // Efecto de desplazamiento infinito si est� activado
        if (infiniteHorizontal)
        {
            float distanceFromCamera = cam.transform.position.x * (1 - parallaxEffect);
            float distanceTravelled = cam.transform.position.x * parallaxEffect;

            // Si la c�mara se ha alejado demasiado, reposicionamos el fondo
            if (distanceTravelled > startPositionX + textureUnitSizeX ||
                distanceTravelled < startPositionX - textureUnitSizeX)
            {
                float offsetPositionX = (distanceTravelled - startPositionX) % textureUnitSizeX;
                transform.position = new Vector3(startPositionX + offsetPositionX, transform.position.y, transform.position.z);
                Debug.Log($"Reposicionando {gameObject.name}: nueva posici�n X = {transform.position.x}");
            }
        }
    }
}