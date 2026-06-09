

using DebugUtils;
using FerryBoat.Actions;
using FerryBoat.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoardingManager : MonoBehaviour,IBoardCal
{
    [SerializeField] TMP_Text mScore;
    [SerializeField] TMP_Text mTime;
    [SerializeField] Button mBoardingStartBtn;
    [SerializeField] GameObject mBoardingPanel;
    [SerializeField] GameObject mEnginePanel;
    [SerializeField] GameObject mBargeObj;

    [Header("Boardings")]
    [SerializeField] List<BoardingCal> boardings = new List<BoardingCal>();

    [Header("NPC")]
    [SerializeField] NPCStore mNPCStore;

    [Header("Timer:")]
    [SerializeField] Timer mTimer;

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

        mTimer.DisplayTimer += DisplayTimerAction;
        mTimer.OnCompleteAct += TimerEndAction;

        mBargeObj.SetActive(true);
    }

    private void OnDisable()
    {
        mBoardingStartBtn.onClick.RemoveAllListeners();

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        mTimer.DisplayTimer -= DisplayTimerAction;
        mTimer.OnCompleteAct -= TimerEndAction;
    }

    private void OnDestroy()
    {
        mBoardingStartBtn.onClick.RemoveAllListeners();

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        mTimer.DisplayTimer -= DisplayTimerAction;
        mTimer.OnCompleteAct -= TimerEndAction;
    }

    private void DisplayTimerAction(string obj)
    {
        mTime.text = obj;
    }

    private void TimerEndAction()
    {
        mTime.text = "00:00 mins";

        GameCamController.Instance.SetCam(CamType.driver);
        mEnginePanel.SetActive(true);
        GamePopUp.Instance.PopStat("Please Press the <color=yellow>[Space Bar]</color> to start the engine", 3.0f);
        mBargeObj.SetActive(false);
        try
        {
            this.gameObject.SetActive(false);

        }
        catch(NullReferenceException ex)
        {
            Debug.LogWarning(ex.Message);
        }



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

                mTimer.Begin(1); // main game logic ,enable after final build
                mTime.text = mTimer.Display;

                PassengerBoarding(passengerCount, () =>
                {
                    CarBoarding(carCount, () =>
                    {
                        TruckBoarding(truckCount);
                    });
                });

                break;


            case BoardType.offBoard:
                break;
        }
    }

    public async void PassengerBoarding(int count,Action onComplete = null)
    {
        if (count <= 0)
        {
            onComplete?.Invoke();
            return;
        }

        GameCamController.Instance.SetCam(CamType.passengers);

        for (int i = 0;  i < count; i++)
        {
            mTimer.Resume();

            GameObject go = Instantiate(mNPCStore.GetRandomNPC().gameObject);

            NPC npc = go.GetComponent<NPC>();
            npc.SetPathDuration(5);

            go.SetActive(true);
            try
            {
                await npc.WaitForPathComplete(_cts.Token);
            }
            catch(TaskCanceledException)
            {
                return;
            }
           

            mTimer.Pause();

            Destroy(go.gameObject,0.2f);
            DevDebug.Log($"NPC:{i} is reached ", DebugColor.Orange);
        }

        onComplete?.Invoke();
    }

    public async void CarBoarding(int count,Action onComplete = null)
    {
        if (count <= 0)
        {
            onComplete?.Invoke();
            return;
        }

        GameCamController.Instance.SetCam(CamType.vechicle);

        for (int i = 0; i < count; i++)
        {
            mTimer.Resume();

            GameObject go = Instantiate(mNPCStore.GetRandomCar().gameObject);

            Vehicle vehicle = go.GetComponent<Vehicle>();
            vehicle.SetPathDuration(30);

            go.SetActive(true);
            try
            {
                await vehicle.WaitForPathComplete(_cts.Token);
            }
            catch(TaskCanceledException)
            {
                return;
            }
           

            mTimer.Pause();

            Destroy(go.gameObject, 0.2f);
            DevDebug.Log($"Car:{i} is reached ", DebugColor.Orange);
        }

        onComplete?.Invoke();
    }

    public async void TruckBoarding(int count, Action onComplete = null)
    {
        if (count <= 0)
        {
            onComplete?.Invoke();
            return;
        }

        GameCamController.Instance.SetCam(CamType.vechicle);

        for (int i = 0; i < count; i++)
        {
            mTimer.Resume();

            GameObject go = Instantiate(mNPCStore.GetRandomTruck().gameObject);

            Vehicle vehicle = go.GetComponent<Vehicle>();
            vehicle.SetPathDuration(60);

            go.SetActive(true);
            try
            {
                await vehicle.WaitForPathComplete(_cts.Token);
            }
            catch
            {
                return;
            }
           

            mTimer.Pause();

            Destroy(go.gameObject, 0.2f);
            DevDebug.Log($"Truck:{i} is reached ", DebugColor.Orange);
        }

        onComplete?.Invoke();
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

        if (time > 120)
        {
            StartCoroutine(OverTwoMinuteLimit());
        }
        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        mTime.text = string.Format($"Boarding time: {m:00}:{s:00} mins");

        mBoardingStartBtn.interactable = time == 120;
    }

    IEnumerator OverTwoMinuteLimit()
    {
        GamePopUp.Instance.FinalPopUp("Warning \n You have crossed the 2 min limit");
        yield return new WaitForSecondsRealtime(5f);
        GamePopUp.Instance.ClosePanel();

    }
    
}

public enum BoardType { onBoard,offBoard}

public enum BoardCharType {passenger,car,truck }
