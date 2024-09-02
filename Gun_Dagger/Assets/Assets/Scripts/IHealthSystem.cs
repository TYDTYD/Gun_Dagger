using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 오브젝트의 관한 체력 시스템 인터페이스

public interface IHealthSystem
{
    // 체력 데미지 함수
    public void TakeDamage(float damage, int? pen); // 관통력 매개변수 추가

    // 관통력 계산 함수
    public int RetPen(int pen);

    public void KnockBack(Vector2 dir);

    public void PlayerEffectDir(Vector2 dir);

}
