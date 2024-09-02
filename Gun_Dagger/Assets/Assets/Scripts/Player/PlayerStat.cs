using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    PlayerHealth health;
    Weapon weapon;
    GameManager money;
    public Text stat;
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<PlayerHealth>();
        weapon = GetComponent<Weapon>();
        money = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        {// ÅØ½ºÆ® °ø°£
            stat.text = "µ· : " + money.money+ "\n"
                               +"Åº ¼ö : " + weapon.collectionBulletNum+ "\n";
        }
        stat.transform.position = Camera.main.transform.position + new Vector3(0, 0, 10);
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
