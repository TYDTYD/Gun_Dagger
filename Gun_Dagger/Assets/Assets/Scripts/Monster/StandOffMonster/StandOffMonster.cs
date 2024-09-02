using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandOffMonster : MonoBehaviour
{
    Monster GetMonster;
    // Start is called before the first frame update
    void Start()
    {
        GetMonster = GetComponent<Monster>();
        GetMonster.type = Monster.monsterType.Gunslinger;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
