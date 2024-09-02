using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �̱��� ������ ������ ���� ���� ���� Ŭ����
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object _lock = new object();

    // �ν��Ͻ� getter �Լ�
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


    // ��ü �ε�� �ϳ��� �ν��Ͻ����� �����ϱ� ���� ����
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
