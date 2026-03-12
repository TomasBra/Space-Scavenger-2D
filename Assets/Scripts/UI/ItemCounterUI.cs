using UnityEngine;
using TMPro;
public class ItemCounter : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI ironText;
    [SerializeField]
    private TextMeshProUGUI copperText;
    [SerializeField]
    public TextMeshProUGUI goldText;


    void Start()
    {
        SetIron(0);
        SetCopper(0);
        SetGold(0);
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
