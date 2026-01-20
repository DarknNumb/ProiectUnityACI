using System.Collections.Generic;
using UnityEngine;

public class TextureClassifier : MonoBehaviour
{
    public ReferenceTextureSet referenceSet;

    void Start()
    {
        ComputeReferenceHistograms();
        ClassifySceneTextures();
    }

    void ComputeReferenceHistograms()
    {
        foreach (var entry in referenceSet.textures)
        {
            Texture2D readable = MakeReadable(entry.texture);
            entry.histogram = TextureAnalyzer.ComputeHistogram(readable);
        }
    }

    void ClassifySceneTextures()
    {
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);

        foreach (Renderer r in renderers)
        {
            bool hasTexture = false;

            foreach (Material mat in r.sharedMaterials)
            {
                if (mat && mat.HasProperty("_MainTex") &&
                mat.GetTexture("_MainTex") != null)
                {
                    hasTexture = true;

                    Texture2D tex = mat.GetTexture("_MainTex") as Texture2D;
                    if (!tex) continue;

                    Texture2D readable = MakeReadable(tex);
                    float[] hist = TextureAnalyzer.ComputeHistogram(readable);

                    string bestLabel = "Unknown";
                    float bestScore = float.MaxValue;

                    foreach (var refTex in referenceSet.textures)
                    {
                        float score = TextureComparer.EuclideanDistance(
                            hist, refTex.histogram);

                        if (score < bestScore)
                        {
                            bestScore = score;
                            bestLabel = refTex.label;
                        }
                    }

                    Debug.Log(
                        $"{r.gameObject.name} ? {bestLabel} ({bestScore:F3})");
                }
            }
            if (!hasTexture)
            {
                Debug.Log(
                    $"{r.gameObject.name} ? NoTexture");
            }
        }
    }

    Texture2D MakeReadable(Texture2D original)
    {
        RenderTexture rt = RenderTexture.GetTemporary(
            original.width, original.height, 0);

        Graphics.Blit(original, rt);
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(
            original.width, original.height, TextureFormat.RGBA32, false);

        readable.ReadPixels(
            new Rect(0, 0, rt.width, rt.height), 0, 0);
        readable.Apply();

        RenderTexture.ReleaseTemporary(rt);
        RenderTexture.active = null;

        return readable;
    }
}
