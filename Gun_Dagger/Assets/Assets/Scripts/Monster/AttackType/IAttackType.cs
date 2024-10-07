using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackType
{
    Node.NodeState Chance();

    Node.NodeState AttackLogic();

    void SetAttack();
}
