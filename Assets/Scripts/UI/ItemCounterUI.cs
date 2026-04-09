using UnityEngine;
using TMPro;
public class ItemCounter : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI sampleText;
    [SerializeField]
    private TextMeshProUGUI ironText;
    [SerializeField]
    private TextMeshProUGUI copperText;
    [SerializeField]
    public TextMeshProUGUI goldText;


    void Start()
    {
        SetSamples(0, 0);
        SetIron(0);
        SetCopper(0);
        SetGold(0);
    }

    public void SetSamples(int amount, int maxAmount)
    { 
        sampleText.text = amount.ToString() + "/" + maxAmount.ToString();
    }

    public void SetIron(int amount)
    {
        ironText.text = amount.ToString();
    }

    public void SetCopper(int amount)
    {
        copperText.text = amount.ToString();
    }

    public void SetGold(int amount)
    {
        goldText.text = amount.ToString();
    }
}
