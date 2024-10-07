using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BulletShop : MonoBehaviour
{
    public Weapon weapon;
    public GameObject bulletShop;
    public TMP_Text bulText;
    // Start is called before the first frame update
    void Start()
    {
        weapon = FindObjectOfType<Player>().GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        bulText.text = "���� źȯ ���� : " + weapon.rangeWeaponData.bulletPrice + "$\n" +
                                   "���� �ڱ� : " + GameManager.Instance.money + "$\n" +
                                   "���� źȯ : " + weapon.GetCollectionBulletNum + "$\n";
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKey(KeyCode.Z))
            bulletShop.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHealthSystem healthSystem) && collision.CompareTag("Player"))
            bulletShop.SetActive(false);
    }
}
