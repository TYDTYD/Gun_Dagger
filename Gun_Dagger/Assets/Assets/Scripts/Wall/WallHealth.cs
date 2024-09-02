using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHealth : Wall,IHealthSystem
{
    public int curDef =10;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void TakeDamage(float damage, int? pen) 
    {
        
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
