using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool global = true;
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance =(T)FindObjectOfType<T>();
            }
            if (instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                DontDestroyOnLoad(go);
                instance = go.AddComponent<T>();
            }
            return instance;
        }

    }

    // Returns an existing instance without creating an unconfigured GameObject.
    public static bool TryGetInstance(out T result)
    {
        result = instance;
        if (result == null)
        {
            result = (T)FindObjectOfType<T>();
        }

        if (result != null)
        {
            instance = result;
            return true;
        }

        return false;
    }

    void Awake()
    {    
        Debug.LogWarningFormat("{0}[{1}] Awake", typeof(T), this.GetInstanceID());
        if (global)
        {
            if(instance!=null && instance!= this.gameObject.GetComponent<T>())
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject.GetComponent<T>();
        }
        this.OnStart();
    }

    protected virtual void OnStart()
    {

    }
}
