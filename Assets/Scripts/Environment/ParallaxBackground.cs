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
        // Usamos Camera.main en lugar de buscar un transform específico
        cam = Camera.main;
        lastCameraPosition = cam.transform.position;
        startPositionX = transform.position.x;

        // Obtenemos el tamaño de la textura para saber cuándo reposicionar
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x;

        Debug.Log($"Parallax inicializado en {gameObject.name} con efecto: {parallaxEffect}");
    }

    private void LateUpdate()
    {
        if (cam == null)
        {
            Debug.LogError("¡Cámara principal no encontrada!");
            return;
        }

        // Calculamos el desplazamiento de la cámara desde el último frame
        Vector3 deltaMovement = cam.transform.position - lastCameraPosition;

        // Movemos el fondo en dirección contraria al movimiento de la cámara, 
        // pero aplicando el efecto parallax
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, 0, 0);
        lastCameraPosition = cam.transform.position;

        // Efecto de desplazamiento infinito si está activado
        if (infiniteHorizontal)
        {
            float distanceFromCamera = cam.transform.position.x * (1 - parallaxEffect);
            float distanceTravelled = cam.transform.position.x * parallaxEffect;

            // Si la cámara se ha alejado demasiado, reposicionamos el fondo
            if (distanceTravelled > startPositionX + textureUnitSizeX ||
                distanceTravelled < startPositionX - textureUnitSizeX)
            {
                float offsetPositionX = (distanceTravelled - startPositionX) % textureUnitSizeX;
                transform.position = new Vector3(startPositionX + offsetPositionX, transform.position.y, transform.position.z);
                Debug.Log($"Reposicionando {gameObject.name}: nueva posición X = {transform.position.x}");
            }
        }
    }
}