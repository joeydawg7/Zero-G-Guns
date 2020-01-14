using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Profiling;
using UnityEngine.Profiling;

public class ObjectPooler : MonoBehaviour
{

    private static ObjectPooler _instance;
    public static ObjectPooler Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        resetDisableTimer = false;

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, gameObject.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);

        }
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
    public List<Guns> potentialGunsToSpawn;
    public Guns defaultPistol;

    bool resetDisableTimer;


    private void Start()
    {

    }

    //works like instatiate but from a magic poooooool
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError("Pool with tag " + tag + " doesn't exist");
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;


        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        objectToSpawn.transform.parent = null;

        return objectToSpawn;

    }
    //overload of the above with parent settings
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("Pool with tag " + tag + " doesn't exist");
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.parent = parent;
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;

    }


    IEnumerator DisableOverTimeCoroutine(GameObject go, float time)
    {
        for (float timer = time; timer >= 0; timer -= Time.deltaTime)
        {
            if (resetDisableTimer)
            {
                resetDisableTimer = false;
                yield break;
            }
        }

        go.SetActive(false);
    }

    public bool finishedResetting = false;

    public void ResetRound()
    {
        //goes through every pooled object and turns it off at the end of the round to prevent leftover bullets
        finishedResetting = false;

        Debug.Log("resetting round");

        float t = Extensions.StartCodeTimer();


        for (int i = 0; i < poolDictionary.Keys.Count; i++)
        {
            //yeah this all makes sense... right? lol
            //basically just grabbing from the dictionary key (tag) to get our queue of values and turning that queue into a list so we can loop through each one individually and set all these objects to false
            //ez pz
            for (int j = 0; j < poolDictionary[poolDictionary.Keys.ToList()[i]].ToList().Count; j++)
            {
                GameObject go = poolDictionary[poolDictionary.Keys.ToList()[i]].ToList()[j];


                if (go == null)
                {
                    //if you see this error it means a pooled object got deleted from its parent pool somehow, which should never happen as it can lead to some incredibly fatal errors during extended play
                    Debug.LogError("Missing pooled object found in " + poolDictionary.Keys.ToList()[i] + " tag's list");
                }
                //turn off the object, make sure it wont destroy for future rounds and that it has no parent to ovveride its "don't destroy" status
                go.SetActive(false);
                go.transform.SetParent(transform);
                //only set DDOL for parent objects
                if (go.transform.parent == null)
                    DontDestroyOnLoad(go);

                //go.transform.SetParent(transform);
            }

        }

        System.GC.Collect();

        Extensions.EndCodeTimer(t, "ResettingCode");

        finishedResetting = true;
    }

}
