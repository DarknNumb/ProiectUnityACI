using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class TextureClassifier : MonoBehaviour
{
    public ReferenceTextureSet referenceSet;
    public TMP_Dropdown selectMenu;
    public ScrollView scrollView;
    public TMP_Text text;
    public Dictionary<string, List<string>> resultDictionary = new Dictionary<string, List<string>>();
    public Camera mainCamera;
    public UnityEngine.UI.Slider slider;

    void Start()
    {
        ComputeOptions();
        
        ComputeReferenceHistograms();
        ClassifySceneTextures();

    }

    public void ChangeResults()
    {
        
        string yes= string.Join("\n", resultDictionary[selectMenu.options[selectMenu.value].text]);
        text.text = yes;

    }

    public void RefreshResults()
    {
        selectMenu.ClearOptions();
        resultDictionary = new Dictionary<string, List<string>>();
        ComputeOptions();

        ComputeReferenceHistograms();
        ClassifySceneTextures();
        ChangeResults();
    }

    public void ComputeOptions()
    {
        List<string> list = new List<string>();
        foreach (var entry in referenceSet.textures)
        {
            resultDictionary.Add(entry.label, new List<string>());
            list.Add(entry.label);
            
        }
        resultDictionary.Add("Missing Texture", new List<string>());
        resultDictionary.Add("No Category", new List<string>());
        list.Add("Missing Texture");
        list.Add("No Category");
        selectMenu.AddOptions(list);

    }

    public void ComputeReferenceHistograms()
    {
        foreach (var entry in referenceSet.textures)
        {
            Texture2D readable = MakeReadable(entry.texture);
            entry.histogram = TextureAnalyzer.ComputeHistogram(readable);
        }
    }

    public void ClassifySceneTextures()
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
                    if (bestScore <= slider.value)
                    {
                        resultDictionary[bestLabel].Add(r.gameObject.name);
                    } else
                    {
                        resultDictionary["No Category"].Add(r.gameObject.name);
                    }
                        Debug.Log(
                            $"{r.gameObject.name} ? {bestLabel} ({bestScore:F3})");
                }
            }
            if (!hasTexture)
            {
                resultDictionary["Missing Texture"].Add(r.gameObject.name);
                Debug.Log(
                    $"{r.gameObject.name} ? NoTexture");
            }
        }
    }

    public Texture2D MakeReadable(Texture2D original)
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
