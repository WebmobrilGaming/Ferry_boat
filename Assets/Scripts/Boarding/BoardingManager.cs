

using DebugUtils;
using FerryBoat.Store;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardingManager : MonoBehaviour,IBoardCal
{
    [SerializeField] TMP_Text mScore;
    [SerializeField] TMP_Text mTime;
    [SerializeField] Button mBoardingStartBtn;
    [SerializeField] GameObject mBoardingPanel;

    [Header("Boardings")]
    [SerializeField] List<BoardingCal> boardings = new List<BoardingCal>();

    [Header("NPC")]
    [SerializeField] NPCStore mNPCStore;
    [SerializeField] Transform mNPCTransform;

    int score = 0;
    int time = 0;

    int passengerCount= 0;
    int carCount = 0;
    int truckCount = 0;

    private CancellationTokenSource _cts;


    private void Awake()
    {
        foreach (var cal in boardings)
            cal.callback = this;

        score = 0;
        time = 0;

        _cts = new CancellationTokenSource();
    }

    private void OnEnable()
    {
        mBoardingStartBtn.onClick.AddListener(() =>
        {
            SetBoard(BoardType.onBoard);
        });

        mBoardingStartBtn.interactable = false;
    }

    private void OnDisable()
    {
        mBoardingStartBtn.onClick.RemoveAllListeners();

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public void SetBoard(BoardType type)
    {
       switch(type)
        {
            case BoardType.onBoard:

                Data.passengerCount = passengerCount;
                Data.carCount = carCount;
                Data.truckCount = truckCount;

                Score_System.Instance.Set(score);
                mBoardingPanel.SetActive(false);

                PassengerBaording(passengerCount);

                break;


            case BoardType.offBoard:
                break;
        }
    }

    public async void PassengerBaording(int count)
    {
        if (count <= 0)
            return;

        for (int i = 0;  i < count; i++)
        {
            GameObject go = Instantiate(mNPCStore.GetRandomNPC().gameObject);

            NPC npc = go.GetComponent<NPC>();   
            go.SetActive(true);

            await npc.WaitForPathComplete(_cts.Token);

            Destroy(go.gameObject,0.2f);

            DevDebug.Log($"NPC:{i} is reached ", DebugColor.Orange);
        }
    }

    public void UpdateBoarding(BoardCharType boardCharType,bool onBoard)
    {
        score = boardCharType switch
        {
            BoardCharType.passenger => onBoard? score + 2 : score -2,
            BoardCharType.car => onBoard? score + 10 : score -10,
            BoardCharType.truck => onBoard ? score + 25 : score - 25
        };

        time = boardCharType switch
        {
            BoardCharType.passenger => onBoard ? time + 5 : time - 5,
            BoardCharType.car => onBoard ? time +30 : time - 30,
            BoardCharType.truck => onBoard ? time + 60 : time - 60
        };

        passengerCount  = boardCharType == BoardCharType.passenger ? onBoard? passengerCount +1 : passengerCount - 1 : passengerCount;
        carCount = boardCharType == BoardCharType.car ? onBoard? carCount + 1 : carCount -1: carCount;
        truckCount = boardCharType == BoardCharType.truck ? onBoard ? truckCount +1 :truckCount -1 : truckCount;

        if (passengerCount <= 0) passengerCount = 0;
        if(carCount <= 0) carCount = 0;
        if(truckCount <=0) truckCount = 0;

        if (score <= 0)
            score = 0;

        mScore.text = $"Score:{score}";

        if(time <=0)
            time = 0;

        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        mTime.text = string.Format($"Boarding time: {m:00}:{s:00} mins");

        mBoardingStartBtn.interactable = time <= 120 && time > 0;
    }
   
}

public enum BoardType { onBoard,offBoard}

public enum BoardCharType {passenger,car,truck }
