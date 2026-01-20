using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExportTexture : MonoBehaviour
{
    public string outputFolderName = "ExportedTextures";
    void Start()
    {
        ExportTextures();
    }

    void ExportTextures()
    {
        string folder = Path.Combine(Application.dataPath, outputFolderName);
        Directory.CreateDirectory(folder);

        int counter = 0;
        int nullCounter = 0;
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        HashSet<Texture2D> exported = new HashSet<Texture2D>();

        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.sharedMaterials)
            {
                if (mat == null || !mat.HasProperty("_MainTex"))
                    continue;
                Debug.Log("works");

                Texture2D tex = mat.GetTexture("_MainTex") as Texture2D;
                if (tex == null)
                {
                    string filepath2 = Path.Combine(folder, "NULL" + nullCounter + ".png");
                    File.WriteAllText(filepath2, "NULL");
                    nullCounter++;
                    continue;
                }

                Debug.Log("works2");
                /*if (exported.Contains(tex))
                    continue;
                */


                Texture2D readableTex = MakeTextureReadable(tex);
                byte[] png = readableTex.EncodeToPNG();
                string filepath;
                if (!exported.Contains(tex))
                {
                    filepath = Path.Combine(folder, tex.name + ".png");
                }
                else
                {
                    filepath = Path.Combine(folder, tex.name + counter + ".png" );
                    counter++;
                }

                    File.WriteAllBytes(filepath, png);
                exported.Add(tex);

                Debug.Log("Exported texture : " + filepath);
            }
        }
        
    }

    private Texture2D MakeTextureReadable(Texture2D original)
    {
        RenderTexture rt = RenderTexture.GetTemporary(
            original.width,
            original.height, 0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
            );

        Graphics.Blit(original, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readableTex = new Texture2D(original.width, original.height);
        readableTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readableTex;
    }
}
