using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class script_UIActive : NetworkBehaviour
{
    // private script_DMController _dm;
    private Text _hpText;
    private int _i;
    private Image _image;
    private Text _stateText;

    private void Start()
    {
        // _dm = transform.ReturnRoot().GetComponent<script_DMController>();
    }

    private void Update()
    {
        _i = 0;
        // foreach (var monster in _dm.monsters)
        // {
        //     if (monster != null)
        //     {
        //         _image = transform.Find("Monster" + _i + "/Image").GetComponent<Image>();
        //         _image.sprite = monster.transform.Find("Token").GetComponent<SpriteRenderer>().sprite;
        //         _hpText = transform.Find("Monster" + _i + "/Hitpoints").GetComponent<Text>();
        //         // _hpText.text = "Hitpoints:" + monster.GetComponent<script_MonsterController>().myHitpoints;
        //         _i++;
        //     }
        // }
    }
}