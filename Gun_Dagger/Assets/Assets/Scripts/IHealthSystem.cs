using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��� ������Ʈ�� ���� ü�� �ý��� �������̽�

public interface IHealthSystem
{
    // ü�� ������ �Լ�
    public void TakeDamage(float damage, int? pen); // ����� �Ű����� �߰�

    // ����� ��� �Լ�
    public int RetPen(int pen);

    public void KnockBack(Vector2 dir);

    public void PlayerEffectDir(Vector2 dir);

}
