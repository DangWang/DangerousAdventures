using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class script_UITrap : NetworkBehaviour
{
    private Image _image;
    private Text _text;

    private void Start()
    {
        _image = transform.Find("TrapImage/Image").GetComponent<Image>();
        _text = transform.Find("Description/Text").GetComponent<Text>();
    }

    public void ChangeTrap(GameObject trapType)
    {
        _image.sprite = trapType.GetComponent<SpriteRenderer>().sprite;
        _text.text = "Cost: " + trapType.GetComponent<script_Trap>().trapCost;
    }
}