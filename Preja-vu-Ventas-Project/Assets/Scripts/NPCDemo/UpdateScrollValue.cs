using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateScrollValue : MonoBehaviour
{
    public enum BANTType { B, A, N, T }
    public BANTType selectedType;
    public UIbantElement UIPanel;
    public int value;

    [SerializeField] private Slider slider;
    private TMP_Text textoValor;

    void Start()
    {
        textoValor = GetComponent<TMP_Text>();
        UpdateUIAndController();
    }

    void FixedUpdate()
    {
        if (slider != null && textoValor != null)
        {
            UpdateUIAndController();
        }
    }

    public void UpdateUIAndController()
    {
        value = (int)slider.value;
        textoValor.text = value.ToString() + "%";

        if (UIPanel != null)
            return;

        switch (selectedType)
        {
            case BANTType.B:
                UIPanel.temporalBantValueB = value;
                break;
            case BANTType.A:
                UIPanel.temporalBantValueA = value;
                break;
            case BANTType.N:
                UIPanel.temporalBantValueN = value;
                break;
            case BANTType.T:
                UIPanel.temporalBantValueT = value;
                break;
        }
        Debug.Log(value);
    }
    public void ResetValue()
    {
        slider.value = 0f;
    }
     public void SetValue(int value)
    {
        slider.value = value;
    }
}
