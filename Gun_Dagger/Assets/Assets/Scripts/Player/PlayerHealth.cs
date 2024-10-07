using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
// �÷��̾� ü�� ����

public class PlayerHealth : MonoBehaviour, IHealthSystem
{
    int maxHealth = 100;
    float currentHealth;
    public ReactiveProperty<float> health = new ReactiveProperty<float>(100);
    int currentDef = 3; // �÷��̾� ����
    private bool invincible = false; // ���� ��Ȱ��ȭ
    Rigidbody2D GetRigidbody2D;
    public GameObject HpBar;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        GetRigidbody2D = GetComponent<Player>().GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        HpBar.GetComponent<Slider>().value = currentHealth;
    }

    public void SetInvincible(bool value) // ���� ���� ����
    {
        invincible = value;
    }
    public void TakeDamage(float damage, int? pen)
    {
        if (pen == null)
            return;
        // ������ �ƴ� �� ����
        if(!invincible){
            if (pen > currentDef)
            {
                currentHealth -= damage;
                health.Value -= damage;
            }
            else
            {
                currentHealth -= damage / (1 + currentDef - (int)pen);
            }
        }
    }

    public void Groggi()
    {
        
    }

    public void KnockBack(Vector2 dir)
    {
        GetRigidbody2D.AddForce(dir * 500f);
    }

    public int RetPen(int pen)
    {
        int currentPen = pen - currentDef;
        if (currentPen > 0)
            return currentPen;
        else
            return 0;
    }
    public void PlayerEffectDir(Vector2 dir)
    {

    }
}
