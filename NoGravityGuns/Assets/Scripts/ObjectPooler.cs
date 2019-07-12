using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }


    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    bool resetDisableTimer;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        resetDisableTimer = false;

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);

        }

    }

    public GameObject SpawnFromPool (string tag, Vector3 position, Quaternion rotation)
    {

        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("Pool with tag " + tag + " doesn't exist");
            return null;
        }
        GameObject objectToSpawn =  poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;


        IPooledObject pooledObject =  objectToSpawn.GetComponent<IPooledObject>();

        if(pooledObject!=null)
        {
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        //resetDisableTimer = true;

        return objectToSpawn;

    }

    /*
    public void DisableOverTime(GameObject go, float time)
    {
        StartCoroutine(DisableOverTimeCoroutine(go, time));
    }
    */
    IEnumerator DisableOverTimeCoroutine(GameObject go, float time)
    {
        for (float timer = time; timer >= 0; timer -= Time.deltaTime)
        {
            if(resetDisableTimer)
            {
                resetDisableTimer = false;
                yield break;
            }
        }

        go.SetActive(false);
    }


}
