using UnityEngine;
using System.IO;

public class TextureExtractor : MonoBehaviour
{
    public Renderer targetRenderer;

    void Start()
    {
        ExtractTexture();
    }

    void ExtractTexture()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("No renderer assigned.");
            return;
        }

        Material mat = targetRenderer.material;

        if (!mat.HasProperty("_MainTex"))
        {
            Debug.LogWarning("Material has no _MainTex property.");
            return;
        }

        Texture mainTex = mat.GetTexture("_MainTex");

        if (mainTex == null)
        {
            Debug.LogWarning("Material has _MainTex but it is null.");
            return;
        }

        Texture2D tex2D = mainTex as Texture2D;

        if (tex2D == null)
        {
            Debug.LogWarning("Texture is not a Texture2D (it might be procedural or GPU-only).");
            return;
        }

        // Detect Unity's default texture
        if (tex2D.width <= 2 && tex2D.height <= 2)
        {
            Debug.LogWarning("Texture is likely a default placeholder (no texture assigned).");
            return;
        }

        SaveTexture(tex2D);
    }

    void SaveTexture(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();

        string path = Application.dataPath + "/SavedTexture.png";
        File.WriteAllBytes(path, bytes);

        Debug.Log("Texture saved to: " + path);
    }
}
