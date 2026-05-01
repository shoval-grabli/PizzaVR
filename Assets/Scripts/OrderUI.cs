using UnityEngine;
using TMPro;

public class OrderUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI orderText;

    public void UpdateUI(string display)
    {
        orderText.text = display;
    }

}
