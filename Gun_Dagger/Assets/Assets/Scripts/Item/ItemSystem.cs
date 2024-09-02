using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ItemSystem : MonoBehaviour
{
    public ItemData[] ItemDataSet;
    public Item[] ItemSet = new Item[100];
    WeaponData GetWeaponData;
    Player GetPlayer;
    Subject<int> timer = new Subject<int>();
    // Start is called before the first frame update
    void Start()
    {
        GetPlayer = FindObjectOfType<Player>();
        for(int i=0; i<ItemDataSet.Length; i++)
        {
            ItemData data = ItemDataSet[i];
            ItemSet[i] = new Item(i,data.ItemName,data.IsReturn,(int)data.type,data.tier,
                data.shortDescription,data.longDescription,data.value);
        }
        ItemSet[0].GetExecute = MilitaryServiceCompletion;
        //ItemSet[1].GetEvent = Sprained_Ankle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MilitaryServiceCompletion()
    {
        GetWeaponData.damage *= 1.2f;
    }

    void Sprained_Ankle(bool p)
    {
        if (p)
        {
            StartCoroutine(MoveDecrease());
        }
    }

    IEnumerator MoveDecrease()
    {
        GetPlayer.GetMovement.moveSpeed *= 0.75f;
        yield return new WaitForSeconds(1f);
        GetPlayer.GetMovement.moveSpeed *= 1.25f;
    }
}