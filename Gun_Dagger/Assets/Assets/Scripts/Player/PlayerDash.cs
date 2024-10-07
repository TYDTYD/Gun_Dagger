using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private bool isFunAllow = true;

    int count = 1;
    private float DashDistance = 20f;
    private float DashSpeed = 0.3f;
    private float DashCool = 0.3f;
    private int DashDamage = 3;
    private float DashKnockBack = 0.15f;

    private Vector2 forward = new Vector2(0, 1);
    private Vector2 backward = new Vector2(0, -1);
    private Vector2 right = new Vector2(1, 0);
    private Vector2 left = new Vector2(-1, 0);

    private Vector2 curPos;

    Player player;
    PlayerMovement pos;

    void Start()
    {
        player = GetComponent<Player>();
        pos = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        Dash();
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (pos.GetClickValue > 1 && count == 1)
        {
            count = 0;
            if (other.gameObject.TryGetComponent<Monster>( out Monster monster))
            {
                switch (pos.GetClickValue)
                {
                    case 2: curPos = forward; break;
                    case 3: curPos = left; break;
                    case 5: curPos = backward; break;
                    case 7: curPos = right; break;
                    case 6: curPos = forward + left; break;
                    case 14: curPos = forward + right; break;
                    case 15: curPos = backward + left; break;
                    case 35: curPos = backward + right; break;
                }
                pos.GetDir = Vector2.zero;
                monster.GetMonsterHealth.TakeDamage(DashDamage,null);
                Vector2 dir = new Vector2(transform.position.x - other.collider.ClosestPoint(transform.position).x, transform.position.y - other.collider.ClosestPoint(transform.position).y);
                monster.GetMonsterHealth.KnockBack(dir.normalized * DashKnockBack);

            }
            else if (other.gameObject.TryGetComponent(out BreakWallHealth wallHealth))
            {
                pos.GetDir = Vector2.zero;
                wallHealth.TakeDamage(DashDamage,null);
            }
        }
    }
    public void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && pos.GetClickValue > 1 && isFunAllow)
        {
            count = 1;
            isFunAllow = false;

            player.GetHealth.SetInvincible(true);
            switch (pos.GetClickValue)
            {
                case 2: curPos = forward; break;
                case 3: curPos = left; break;
                case 5: curPos = backward; break;
                case 7: curPos = right; break;
                case 6: curPos = forward + left; break;
                case 14: curPos = forward + right; break;
                case 15: curPos = backward + left; break;
                case 35: curPos = backward + right; break;
            }
            pos.GetDir = curPos * DashSpeed * DashDistance;
            player.GetHealth.SetInvincible(false);
            Invoke("EnableFun", DashCool);
            Invoke("CountInit", DashSpeed);
        }
    }
    void CountInit()
    {
        count = 0;
        pos.GetDir = Vector2.zero;
    }

    void EnableFun()
    {
        isFunAllow = true;
    }
}
