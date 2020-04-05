using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class script_UITrap : NetworkBehaviour
{
    private script_DMController dm;
    private Image image;
    private Text text;

    private void Start()
    {
        image = transform.Find("TrapImage/Image").GetComponent<Image>();
        text = transform.Find("Description/Text").GetComponent<Text>();
    }

    public void ChangeTrap(GameObject trapType)
    {
        image.sprite = trapType.GetComponent<SpriteRenderer>().sprite;
        text.text = "Cost: " + trapType.GetComponent<script_Trap>().trapCost;
    }
}