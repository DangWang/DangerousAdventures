using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_MonsterMenu : MonoBehaviour
{
    void Start()
    {
        var parent = transform.ReturnRoot().gameObject;
        var parentController = parent.GetComponent<scr_Player>();

        var i = 0;
        foreach (var monsterType in parentController.monsterTypes)
        {
            if (i < 18) // current limit
            {
                var monsterScript = monsterType.GetComponent<scr_Monster>();
                var monsterSprite = monsterType.transform.Find("Token").GetComponent<SpriteRenderer>().sprite;
                var menuElement = transform.Find("    " + i.ToString()).gameObject;
                menuElement.transform.Find("Image").GetComponent<Image>().sprite = monsterSprite;
                menuElement.transform.Find("Text").GetComponent<Text>().text = "Cost: " + monsterScript.monsterCost;
            }
            else
            {
                break;
            }
            i++;
        }
    }
}
