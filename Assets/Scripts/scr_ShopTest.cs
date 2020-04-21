using UnityEngine;

public class scr_ShopTest : MonoBehaviour
{
    public GameObject shopMenu;

    public void OpenShopMenu()
    {
        if (shopMenu.activeInHierarchy)
        {
            shopMenu.SetActive(false);
        }
        else
        {
            shopMenu.SetActive(true);
        }
    }
}