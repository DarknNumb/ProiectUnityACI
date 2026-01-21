using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimilarityManager : MonoBehaviour
{
    public TMP_Text output;
    public Slider slider;

    private void Start()
    {
        output.text = slider.value.ToString("F2");
    }

    public void outputText()
    {
        output.text = slider.value.ToString("F2");
    }
}
