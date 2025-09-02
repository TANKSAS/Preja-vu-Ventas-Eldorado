using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveTexture : MonoBehaviour
{

    Material mMaterial;
    [SerializeField] private string savePath = "Capturas"; // Ruta donde se guardará la textura, asignable en el editor
    public void SaveTexture()
    {
        // Obtener la textura del material asignado al MeshRenderer
        Texture2D texture = (Texture2D)mMaterial.mainTexture;

        // Validar que la textura no sea nula
        if (texture != null)
        {
            // Convertir a formato no comprimido y aplicarle modificaciones
            Texture2D readableTexture = ConvertToReadableTexture(texture);

            // Aquí aplicas las modificaciones necesarias a 'readableTexture'
            ApplyModifications(readableTexture);

            // Usar Path.Combine para crear la ruta correctamente
            string fullPath = System.IO.Path.Combine(Application.persistentDataPath, savePath, "SavedTexture_" + Random.Range(0, 100000) + ".png");

            // Guardar la textura como PNG
            SaveTextureAsPNG(readableTexture, fullPath);
        }
        else
        {
            Debug.LogError("No se encontró ninguna textura en el material asignado.");
        }
    }
    private void ApplyModifications(Texture2D texture)
    {
        // Obtener tiling y offset del material
        Vector2 tiling = mMaterial.GetTextureScale("_MainTex"); // Asegúrate de usar el nombre correcto del parámetro
        Vector2 offset = mMaterial.GetTextureOffset("_MainTex"); // Asegúrate de usar el nombre correcto del parámetro

        Color[] pixels = new Color[texture.width * texture.height];

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                // Calcular las coordenadas de textura con offset y tiling
                float u = (x / (float)texture.width) * tiling.x + offset.x;
                float v = (y / (float)texture.height) * tiling.y + offset.y;

                // Asegúrate de que u y v estén dentro del rango [0, 1]
                u = Mathf.Repeat(u, 1);
                v = Mathf.Repeat(v, 1);

                // Obtener el color original usando las coordenadas calculadas
                Color originalColor = texture.GetPixelBilinear(u, v);
                // Modificar el color según lo desees
                pixels[y * texture.width + x] = originalColor; // Ejemplo: manteniendo el color original
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(); // Aplica los cambios a la textura
    }
    public Texture2D CaptureRenderTexture(RenderTexture rt)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        // Crear una nueva Texture2D con las dimensiones de la RenderTexture
        Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();

        RenderTexture.active = currentRT;
        return texture;
    }

    public void SaveRenderTexture(RenderTexture renderTexture)
    {
        // Convertir RenderTexture a Texture2D
        Texture2D texture = CaptureRenderTexture(renderTexture);

        // Usar Path.Combine para generar la ruta correctamente
        string fullPath = System.IO.Path.Combine(Application.persistentDataPath, savePath, "RenderTexture_" + Random.Range(0, 100000) + ".png");

        // Guardar la textura como PNG
        SaveTextureAsPNG(texture, fullPath);
    }
    Texture2D ConvertToReadableTexture(Texture2D texture)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(texture, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;

        Texture2D readableTex = new Texture2D(texture.width, texture.height);
        readableTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableTex;
    }

    public void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        byte[] _bytes = _texture.EncodeToPNG();

        // Verificar que la carpeta exista, si no, crearla
        string dirPath = System.IO.Path.GetDirectoryName(_fullPath);
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }

        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
    }
}
