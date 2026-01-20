using UnityEngine;

public static class TextureAnalyzer 
{
    public static float[] ComputeHistogram(Texture2D tex, int bins = 16)
    {
        float[] histogram = new float[bins * 3];
        Color[] pixels = tex.GetPixels();

        foreach (Color c in pixels)
        {
            int r = Mathf.FloorToInt(c.r * (bins - 1));
            int g = Mathf.FloorToInt(c.g * (bins - 1));
            int b = Mathf.FloorToInt(c.b * (bins - 1));

            histogram[r]++;
            histogram[bins + g]++;
            histogram[2 * bins + b]++;
        }

        float total = pixels.Length;
        for (int i = 0; i < histogram.Length; i++)
            histogram[i] /= total;

        return histogram;
    }
}
