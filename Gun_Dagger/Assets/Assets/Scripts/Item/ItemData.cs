using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { stat,passive,Event,mix }
    public enum StatType {
        Hp,
        def,
        moveSpeed,
        RangeDamage,
        RangePen,
        bulletNum,
        fireRate,
        reloadTime,
        bulletPrice,
        bulletSpeed,
        meleeRangeW,
        meleeRangeR,
        meleeDamage,
        meleePen,
        firstDelay,
        secondDelay
    }
    public string ItemName;
    public byte tier;
    [Tooltip("리턴이면 true 리스크면 false")]
    public bool IsReturn; //리턴 리스크 판별
    public ItemType type;
    [TextArea]
    [Tooltip("카드 선택 전에 나오는 설명")]
    public string shortDescription;
    [TextArea]
    [Tooltip("카드 선택 후에 나오는 설명")]
    public string longDescription;

    [Header("변경할 스탯 종류")]
    public StatType[] statTypes;
    [Header("변경할 스탯의 값")]
    public int[] value;

}
