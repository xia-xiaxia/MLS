using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _isApplicationQuitting = false;

    public static T Instance
    {
        get
        {
            if (_isApplicationQuitting)
            {
                Debug.LogWarning($"[Singleton] ʵ�� {typeof(T)} �ѱ����٣����ʱ��ܾ�");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // ������ʵ�����־û�
                    GameObject singletonObj = new GameObject($"{typeof(T).Name} (Singleton)");
                    _instance = singletonObj.AddComponent<T>();
                    DontDestroyOnLoad(singletonObj);
                    Debug.Log($"[Singleton] ������ʵ��: {typeof(T)}");
                }
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] ��⵽�ظ�ʵ�� {typeof(T)}��������ʵ��");
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}