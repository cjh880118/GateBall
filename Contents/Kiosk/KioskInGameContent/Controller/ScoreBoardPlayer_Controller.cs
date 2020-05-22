using System.Collections;
using System.Collections.Generic;
using JHchoi.Constants;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardPlayer_Controller : MonoBehaviour
{
    public GameObject[] objMission;
    public Text txtMissionCount;

    public void SetScore(MissionModeGame mission, int isSuccess, int chance)
    {
        objMission[(int)mission].transform.GetChild(1).gameObject.SetActive(false);
        objMission[(int)mission].transform.GetChild(2).gameObject.SetActive(false);

        if (isSuccess == 1)
            objMission[(int)mission].transform.GetChild(2).gameObject.SetActive(true);
        else if(isSuccess == 0)
            objMission[(int)mission].transform.GetChild(1).gameObject.SetActive(true);

        txtMissionCount.text = chance.ToString();
    }

}
