using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRandomSystem : MonoBehaviour
{
    ItemSystem itemSystem;
    // Start is called before the first frame update
    void Start()
    {
        itemSystem = GetComponent<ItemSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ClickCardFirst()
    {
        itemSystem.ItemSet[0].GetExecute();
    }

    public void ClickCardSecond()
    {

    }

    public void ClickCardThird()
    {

    }
}
