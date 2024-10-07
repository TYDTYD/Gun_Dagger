using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    PlayerHealth health;
    Weapon weapon;
    public Text stat;
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<PlayerHealth>();
        weapon = GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        {// ÅØ½ºÆ® °ø°£
            stat.text = "µ· : " + GameManager.Instance.money+ "\n"
                               +"Åº ¼ö : " + weapon.GetCollectionBulletNum + "\n";
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
