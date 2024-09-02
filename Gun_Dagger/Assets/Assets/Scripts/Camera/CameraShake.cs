
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public float ShakeAmount;
    float ShakeTime;
    
    public CinemachineVirtualCamera virtualCamera;
    CinemachineBasicMultiChannelPerlin Cnoise;
    public void VibrateForTime(float time)
    {
        ShakeTime = time;
    }
    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        Cnoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ShakeTime > 0)
        {
            Cnoise.m_AmplitudeGain = (ShakeTime*100) + ShakeAmount;
            

            ShakeTime -= Time.deltaTime;
        }
        else
        {
            ShakeTime = 0.0f;
            Cnoise.m_AmplitudeGain = 0;
            
        }
    }
}
