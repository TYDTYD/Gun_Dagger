using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArm : MonoBehaviour 
{
    [SerializeField]
    Player GetPlayer;
    [SerializeField]
    public Transform armPosition;
    public Transform shoulder;
    SpriteRenderer armSprite;
    public SpriteRenderer gun;
    Vector2 mouse;
    // 회전 축 위치 오프셋
    Vector3 Offset = new Vector3(0, 0.12f);
    float angle,x,y,dist;
    Vector3 rightPosition = new Vector3(0.35f, 0.97f, 0);
    Vector3 leftPosition = new Vector3(-0.35f, 0.97f, 0);
    // Start is called before the first frame update
    void Start()
    {
        dist = Vector3.Distance(armPosition.position+Offset, armPosition.position);
        armSprite = armPosition.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어 방향따라 좌우 반전
        if (GetPlayer.GetSpriteRenderer.flipX)
        {
            // 왼쪽 방향
            armPosition.localPosition = leftPosition;
            shoulder.localPosition = leftPosition;
            
            if (Vector3.Dot(Vector3.up, armPosition.right) > 0f)
            {
                armSprite.flipX = true;
                gun.flipY = true;
            }
                
            else
            {
                armSprite.flipX = false;
                gun.flipY = false;
            }
                
                
        }
        else
        {
            // 오른쪽 방향
            armPosition.localPosition = rightPosition;
            shoulder.localPosition= rightPosition;
            
            if (Vector3.Dot(Vector3.up, armPosition.right) < 0f)
            {
                armSprite.flipX = false;
                gun.flipY = false;
            }
            else
            {
                armSprite.flipX = true;
                gun.flipY= true;
            }
                
        }
            
        
        // 마우스 방향따라 회전
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mouse.y - (armPosition.position.y+Offset.y), mouse.x - armPosition.position.x) * Mathf.Rad2Deg;

        x = dist * Mathf.Cos(angle * Mathf.PI / 180f);
        y = dist * Mathf.Sin(angle * Mathf.PI / 180f);

        //armPosition.position = new Vector3(armPosition.position.x + x, armPosition.position.y + Offset.y + y);
        armPosition.rotation = Quaternion.AngleAxis(angle+90f, Vector3.forward);
    }
}
