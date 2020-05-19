using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetScorePlayer : MonoBehaviour
{
    public Text txtRound;
    public GameObject nonePlayer;
    public GameObject alive;
    public GameObject die;
    public GameObject order;

    public void Init()
    {
        txtRound.text = "1R";
        nonePlayer.SetActive(true);
        alive.SetActive(false);
        die.SetActive(false);
        order.SetActive(false);
    }

    public void SetBoard(string round, bool isAlive, string player)
    {
        txtRound.text = round;
        if (isAlive)
        {
            alive.SetActive(true);
            die.SetActive(false);
        }
        else
        {
            alive.SetActive(false);
            die.SetActive(true);
        }
    }

    public void SetOrder(bool isOrder)
    {
        if (isOrder)
            order.SetActive(true);
        else
            order.SetActive(false);
    }
}
