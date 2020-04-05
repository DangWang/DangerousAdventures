using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class script_UIActive : NetworkBehaviour
{
    private script_DMController dm;
    private Text hpText;
    private int i;
    private Image image;
    private Text stateText;

    private void Start()
    {
        dm = transform.ReturnRoot().GetComponent<script_DMController>();
    }

    private void Update()
    {
        i = 0;
        foreach (var monster in dm.monsters)
            if (monster != null)
            {
                image = transform.Find("Monster" + i + "/Image").GetComponent<Image>();
                image.sprite = monster.transform.Find("Token").GetComponent<SpriteRenderer>().sprite;
                hpText = transform.Find("Monster" + i + "/Hitpoints").GetComponent<Text>();
                hpText.text = "Hitpoints:" + monster.GetComponent<script_MonsterController>().myHitpoints;
                i++;
            }
    }
}