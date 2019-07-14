using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EndGameScript : MonoBehaviour
{

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI winOrTie;
    public TextMeshProUGUI Winners;

    public string mainSceneName;
    public string mainMenuScene;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
        }

        if(Input.GetButtonDown("AnyB"))
        {
            SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
        }


    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void StartEndGame(List<PlayerScript> winners)
    {
        gameOverText.alpha = 0;
        winOrTie.alpha = 0;
        winOrTie.text = "";
        Winners.alpha = 0;
        Winners.text = "";
        gameObject.SetActive(true);

        StartCoroutine(EndGame(winners));
    }


    IEnumerator EndGame(List<PlayerScript> winnersList)
    {
        gameOverText.alpha = 1;
        yield return new WaitForSeconds(0.5f);

        winOrTie.alpha = 1;
        Winners.alpha = 1;

        if (winnersList.Count > 1)
        {
            winOrTie.text = "Its a tie!";


            foreach (var winner in GameManager.Instance.players)
            {
                yield return new WaitForSeconds(0.5f);
                Winners.text += "<" + winner.hexColorCode + ">" + winner.playerName + ": " + winner.numKills + "</color> \n";

            }
        }
        //one winner
        else
        {

            winOrTie.text = "<" + winnersList[0].hexColorCode + ">" + winnersList[0].playerName + " Is the winner!" + "</color>";
            yield return new WaitForSeconds(0.5f);

            foreach (var winner in GameManager.Instance.players)
            {
                yield return new WaitForSeconds(0.5f);
                Winners.text += "<" + winner.hexColorCode + ">" + winner.playerName + ": " + winner.numKills + "</color> \n";

            }


        }

    }

}
