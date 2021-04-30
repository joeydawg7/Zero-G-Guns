using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResolutionSelectScript : MonoBehaviour
{
    public enum directionType { left, right };
    public directionType direction;

    ResolutionScript resScript;

    // Start is called before the first frame update
    void Awake()
    {
        resScript = FindObjectOfType<ResolutionScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //if selected
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            ChangeResolutionInDirection(direction);
        }
    }

    void ChangeResolutionInDirection(directionType direction)
    {
        switch (direction)
        {
            case directionType.left:
                EventSystem.current.SetSelectedGameObject(resScript.gameObject);
                Debug.Log("left");
                resScript.SetRes(-1);
                break;
            case directionType.right:
                Debug.Log("right");
                EventSystem.current.SetSelectedGameObject(resScript.gameObject);
                resScript.SetRes(1);

                break;
            default:
                break;
        }
    }
}
