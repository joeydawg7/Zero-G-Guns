using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TraningLevelsScreen : MonoBehaviour
{
    public List<BTT_RoomSO> BTTRooms;
    public LevelSelectData levelSelectDataPrefab;
    //public int columns;

    public Button[] LSDs;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach(Button b in LSDs)
        {          
            var room = BTTRooms[i];
            room.playOnLoad = false;
            //LevelSelectData LSD = GameObject.Instantiate(levelSelectDataPrefab, b.gameObject.transform).GetComponent<LevelSelectData>();
            LevelSelectData LSD = new LevelSelectData();
            LSD.SetRoomData(room, b);
            b.onClick.AddListener(() => LSD.OnClick());           
            i++;                        
        }
        gameObject.SetActive(false);        
    }

    public void ShowTrainingLevelsScreen()
    {
        gameObject.SetActive(true);
        LSDs[0].Select();

        ////get a number of rows from num rooms / columns
        //float rows = BTTRooms.Count / columns;
        //rows = Mathf.FloorToInt(rows);
        ////setup 2d array
        //LSDs = new Button[(int)rows, columns];
        //Transform bg = transform.GetChild(0);
        ////set column nums
        //bg.GetComponent<GridLayoutGroup>().constraintCount = columns;

        //int iterator = 0;
        ////fill 2d array based on rooms list
        //for (int r = 0; r < rows; r++)
        //{
        //    for (int c = 0; c < columns; c++)
        //    {
        //        //iterate thru rooms list linearly
        //        var room = BTTRooms[iterator];
        //        //btt_manager plays the first room it finds with this set to true, so all should be false until one is selected
        //        room.playOnLoad = false;
        //        //spawn level selection box prefab
         //           LevelSelectData LSD = GameObject.Instantiate(levelSelectDataPrefab, bg).GetComponent<LevelSelectData>();
          //          LSD.SetRoomData(room);

        //        LSDs[r, c] = LSD.GetComponent<Button>();

        //        //set OnClick for the button
        //         LSDs[r, c].onClick.AddListener(() => LSD.OnClick());

        //        //select the first in the list by default
        //        if (iterator == 0)
        //            LSDs[r, c].Select();

        //        //if we have iterated past the number of created rooms no reason to keep looping
        //        if (iterator >= BTTRooms.Count)
        //            break;

        //        iterator++;
        //    }
        //}

        //for (int r = 0; r < LSDs.GetLength(0); r++)
        //{
        //    for (int c = 0; c < LSDs.GetLength(1); c++)
        //    {
        //        //get our nav from the button
        //        var nav = LSDs[r, c].navigation;

        //        //check if you can go left and set pos
        //        if (c - 1 >= 0)
        //            nav.selectOnLeft = LSDs[r, c - 1];

        //        //check if you can go right and set pos
        //        if (c + 1 < LSDs.GetLength(1))
        //            nav.selectOnRight = LSDs[r, c + 1];

        //        //check if you can go down and set pos
        //        if (r + 1 < LSDs.GetLength(0))
        //            nav.selectOnDown = LSDs[r + 1, c];

        //        //check if you can go up and set pos
        //        if (r - 1 >= 0)
        //            nav.selectOnUp = LSDs[r - 1, c];

        //        //overwrite the old buttons nav with the new one
        //        LSDs[r, c].navigation = nav;
        //    }
        //}
    }

    
}
