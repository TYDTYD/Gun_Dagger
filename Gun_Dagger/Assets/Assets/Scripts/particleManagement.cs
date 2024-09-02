using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleManagement : MonoBehaviour
{
    static particleManagement instance;

    public ParticleSystem[] particleSystems;

    private void Awake()
    {
        instance = this;
    }

    
    public static void PlayParticle(Vector3 pos, int particleNum , Vector2? dir =null , float? size=1 )=>instance._playParticle(pos ,particleNum,dir,size);

    public void _playParticle (Vector3 pos, int particleNum, Vector2? dir=null,float? size=1)
    {
        particleSystems[particleNum].transform.position = pos;
        ParticleSystem.MainModule main = particleSystems[particleNum].main;
        if (dir != null)
        {


            float angle = Vector2.SignedAngle(Vector2.right, dir.Value);
            
            main.startRotation = -angle * Mathf.Deg2Rad;
        }


        particleSystems[particleNum].Play();
    }
    
}
