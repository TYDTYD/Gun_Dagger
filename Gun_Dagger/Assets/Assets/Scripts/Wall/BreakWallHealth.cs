using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWallHealth : Wall, IHealthSystem
{
    public int curDef = 10;
    public float curHealth = 100;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (curHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void TakeDamage(float damage, int? pen)
    {
        if (pen == null)
        {
            curHealth -= damage;
            return;
        }

        if (pen > curDef)
        {
            curHealth -= damage;
        }
        else
        {
            curHealth -= damage / (1 + curDef - (int)pen);
        }
    }

    public int RetPen(int pen)
    {
        int currentPen = pen - curDef;
        if (currentPen > 0)
            return currentPen;
        else
            return 0;
    }

    public void KnockBack(Vector2 dir)
    {

    }
    public void PlayerEffectDir(Vector2 dir)
    {

    }
}
