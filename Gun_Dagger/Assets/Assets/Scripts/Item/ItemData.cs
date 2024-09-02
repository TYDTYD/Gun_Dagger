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
    [Tooltip("�����̸� true ����ũ�� false")]
    public bool IsReturn; //���� ����ũ �Ǻ�
    public ItemType type;
    [TextArea]
    [Tooltip("ī�� ���� ���� ������ ����")]
    public string shortDescription;
    [TextArea]
    [Tooltip("ī�� ���� �Ŀ� ������ ����")]
    public string longDescription;

    [Header("������ ���� ����")]
    public StatType[] statTypes;
    [Header("������ ������ ��")]
    public int[] value;

}
