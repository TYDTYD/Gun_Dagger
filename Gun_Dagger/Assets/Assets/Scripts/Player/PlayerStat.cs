using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    Player GetPlayer;
    PlayerHealth health;
    Weapon weapon;
    [SerializeField] Text stat;
    Vector3 diff = new Vector3(0, 0, 10);
    // Start is called before the first frame update
    void Start()
    {
        health = GetPlayer.GetHealth;
        weapon = GetPlayer.GetWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        {// ÅØ½ºÆ® °ø°£
            stat.text = "µ· : " + GameManager.Instance.money+ "\n"
                               +"Åº ¼ö : " + weapon.GetCollectionBulletNum + "\n";
        }
        stat.transform.position = Camera.main.transform.position + diff;
        if (Input.GetKey(KeyCode.Tab))
        {
            stat.gameObject.SetActive(true);
        }
        else
        {
            stat.gameObject.SetActive(false);
        }
    }
}
