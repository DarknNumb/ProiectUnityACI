using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ReferenceTextureSet",
    menuName = "Image Analysis/Reference Texture Set"
    )]

public class ReferenceTextureSet : ScriptableObject
{
    public List<ReferenceTextureEntry> textures;
}

[System.Serializable]
public class ReferenceTextureEntry
{
    public string label;
    public Texture2D texture;

    [HideInInspector]
    public float[] histogram;
}