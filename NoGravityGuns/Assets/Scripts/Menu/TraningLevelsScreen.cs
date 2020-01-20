using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TraningLevelsScreen : MonoBehaviour
{
    public List<BTT_RoomSO> BTTRooms;
    public LevelSelectData levelSelectDataPrefab;
    public int columns;

    Button[,] LSDs;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowTrainingLevelsScreen()
    {
        gameObject.SetActive(true);

        //get a number of rows from num rooms / columns
        float rows = BTTRooms.Count / columns;
        rows = Mathf.FloorToInt(rows);
        //setup 2d array
        LSDs = new Button[(int)rows, columns];
        Transform bg = transform.GetChild(0);
        //set column nums
        bg.GetComponent<GridLayoutGroup>().constraintCount = columns;

        int iterator = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {

                var room = BTTRooms[iterator];
                room.playOnLoad = false;
                LevelSelectData LSD = GameObject.Instantiate(levelSelectDataPrefab, bg).GetComponent<LevelSelectData>();
                LSD.SetRoomData(room);

                LSDs[r, c] = LSD.GetComponent<Button>();

                //set OnClick for the button
                LSDs[r, c].onClick.AddListener(() => LSD.OnClick());

                if (iterator == 0)
                    LSDs[r, c].Select();

                    if (iterator >= BTTRooms.Count)
                        break;
                iterator++;
            }
        }

        for (int r = 0; r < LSDs.GetLength(0); r++)
        {
            for (int c = 0; c < LSDs.GetLength(1); c++)
            {


                var nav = LSDs[r, c].navigation;

                //check if you can go left and set pos
                if (c - 1 >= 0)
                    nav.selectOnLeft = LSDs[r, c - 1];

                //check if you can go right and set pos
                if (c + 1 < LSDs.GetLength(1))
                    nav.selectOnRight = LSDs[r, c + 1];

                //check if you can go down and set pos
                if (r + 1 < LSDs.GetLength(0))
                    nav.selectOnDown = LSDs[r + 1, c];

                //check if you can go up and set pos
                if (r - 1 >= 0)
                    nav.selectOnUp = LSDs[r - 1, c];

                LSDs[r, c].navigation = nav;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
