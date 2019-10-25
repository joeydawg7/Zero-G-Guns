using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistenceGiverScript : MonoBehaviour
{
    public List<GameObject> ListToGivePersistenceTo;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var go in ListToGivePersistenceTo)
        {
            DontDestroyOnLoad(go);
        }


        SceneManager.LoadScene("FanRoom");

    }


}
