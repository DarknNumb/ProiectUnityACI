using UnityEngine;

public static class TextureComparer 
{
    public static float EuclideanDistance(float[] a, float[] b)
    {
        float sum = 0f;
        for (int i = 0; i < a.Length; i++)
        {
            float diff = a[i] - b[i];
            sum += diff * diff;
        }
        return Mathf.Sqrt(sum);
    }
}
