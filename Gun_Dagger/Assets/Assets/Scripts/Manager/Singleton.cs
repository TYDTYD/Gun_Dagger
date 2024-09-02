using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 싱글톤 디자인 패턴을 쓰기 위해 만든 클래스
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object _lock = new object();

    // 인스턴스 getter 함수
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<T>();

                        if (instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                }
            }
            return instance;
        }
    }


    // 객체 로드시 하나의 인스턴스만을 생성하기 위한 로직
    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
            return;
        }
        instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }
}
