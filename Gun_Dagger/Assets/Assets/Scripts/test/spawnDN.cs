using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnDN : MonoBehaviour
{
    public DamageNumber dn;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var sp = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            var wp = Camera.main.ScreenToWorldPoint(sp);
            dn.Spawn(wp, Random.Range(1, 100));
        }
    }

}
