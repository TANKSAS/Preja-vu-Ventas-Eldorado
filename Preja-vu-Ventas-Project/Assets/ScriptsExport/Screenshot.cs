using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class Screenshot : MonoBehaviour
{
    // Material donde aplicaremos la textura
    [SerializeField] Material targetMaterial;
    [SerializeField] Sprite targetSprite;
    public float rango;
    public int  blurRad;
    public float rangocolor;
    public Texture2D newTexture;

    Texture2D screenshotTexture;

    public QuadScript reference;

    [SerializeField]
    private Camera targetCamera; // Cámara específica para la captura

    [SerializeField]
    private string savePath = "Screenshots"; // Ruta donde se guardará la captura

    [SerializeField]
    private int imageWidth = 1920; // Ancho de la imagen
    [SerializeField]
    private int imageHeight = 1080; // Alto de la imagen

    private string fullPath;
    private string fileName;
    public string filePath;
    public bool isImage;

    public void Start()
    {
        GameManager.Instance.screenshotController = this;
    }

    public Material GetHeatMapMaterial()
    {
        return targetMaterial;
    }

    public Sprite GetHeatMapSprite()
    {
        return targetSprite;
    }


    public void TakeScreenShot()
    {
        if (targetCamera == null)
        {
            Debug.LogError("No se ha asignado ninguna cámara para tomar la captura de pantalla.");
            return;
        }

        // Asegúrate de que la carpeta existe
        fullPath = Path.Combine(Application.persistentDataPath, savePath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        // Crear el nombre del archivo
        fileName = "screenshot-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
        filePath = Path.Combine(fullPath, fileName);

        // Capturar la pantalla desde la cámara específica
        RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24);
        targetCamera.targetTexture = renderTexture;
        targetCamera.Render();

        // Crear una nueva textura
        RenderTexture.active = renderTexture;
        screenshotTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        screenshotTexture.Apply();

        // Restablecer la cámara
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        //  RenderTexture.ReleaseTemporary(renderTexture);
        // Guardar la captura
        File.WriteAllBytes(filePath, screenshotTexture.EncodeToPNG());

        // Aplicar la transparencia al negro
        ApplyTransparencyAndBlur(screenshotTexture,rango,rangocolor,blurRad);

        // Mensaje de debug
        Debug.Log($"Captura de pantalla completada: {filePath}");
    }

    public void LoadImage(string path)
    {
        // Verifica si el archivo existe
        if (File.Exists(path))
        {
            // Leer los bytes de la imagen desde el archivo
            byte[] fileData = File.ReadAllBytes(path);

            // Crear una nueva textura
            Texture2D texture = new Texture2D(imageWidth, imageHeight);  // Se inicializa con un tamaño pequeño, se redimensionará
            newTexture = texture;
            
            // Cargar los datos de imagen en la textura
            if (texture.LoadImage(fileData))
            {
                ApplyTransparencyAndBlur(texture, rango, rangocolor, blurRad);
                // Asignar la nueva textura con transparencia y desenfoque al material

                if (isImage)
                {
                    targetSprite = Sprite.Create(newTexture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    targetMaterial.mainTexture = newTexture;
                }

                // Si quieres usarla como sprite en un SpriteRenderer

            }
            else
            {
                Debug.LogError("No se pudo cargar la textura desde los bytes de imagen.");
            }
        }
        else
        {
            Debug.LogError("El archivo no existe en la ruta: " + path);
        }
    }

    void ApplyTransparencyAndBlur(Texture2D texture, float blackThreshold , float colorTransparency , int blurRadius)
    {
        // Asegúrate de que la textura tenga el formato adecuado para permitir la transparencia
        newTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        // Procesar cada píxel
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color pixelColor = texture.GetPixel(x, y);
                // Calcular la intensidad del color negro
                float intensity = pixelColor.r + pixelColor.g + pixelColor.b;

                // Si el píxel es suficientemente oscuro, hacerlo completamente transparente
                if (intensity < blackThreshold)
                {
                    pixelColor.a = 0; // Asignar transparencia completa para negro
                }
                else
                {
                    // Aplicar cierta transparencia a otros colores
                    pixelColor.a *= colorTransparency; // Ajustar la transparencia de los demás colores
                }

                newTexture.SetPixel(x, y, pixelColor);
            }
        }
    
        // Aplicar desenfoque
        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                Color blurredColor = Color.clear;
                int count = 0;

                // Promediar los colores en el área de desenfoque
                for (int dy = -blurRadius; dy <= blurRadius; dy++)
                {
                    for (int dx = -blurRadius; dx <= blurRadius; dx++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        // Asegurarse de que no se salga de los límites
                        if (nx >= 0 && nx < newTexture.width && ny >= 0 && ny < newTexture.height)
                        {
                            blurredColor += newTexture.GetPixel(nx, ny);
                            count++;
                        }
                    }
                }

                // Establecer el color promedio en el píxel actual
                newTexture.SetPixel(x, y, blurredColor / count);
            }
        }

        newTexture.Apply();
    }
}

