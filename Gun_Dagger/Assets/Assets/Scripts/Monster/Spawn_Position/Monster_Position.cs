using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterPosition", menuName = "ScriptableObjects/MonsterPosition", order = 2)]

public class Monster_Position : ScriptableObject
{
    public Vector3[] vectors;
    public ObjectType[] type;
    public int spawn_count = 1;
}
