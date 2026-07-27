

using Cysharp.Threading.Tasks;
using DebugUtils;
using Ferry.Loading;
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
    [SerializeField] int no_VehiclesParked;
    [SerializeField] GameObject mBargeObj;
    public static bool GameStarted;

    [Header("Boardings")]
    [SerializeField] List<BoardingCal> boardings = new List<BoardingCal>();

    [Header("NPC")]
    [SerializeField] NPCStore mNPCStore;

    [Header("OFFBoarding")]
    [SerializeField] BoardPlace boardPlace;

    int score = 0;
    int time = 0;

    int passengerCount= 0;
    int carCount = 0;
    int truckCount = 0;

    private CancellationTokenSource _cts;
    BoardData currentboardData;

    [Header("SideCams:")]
    [SerializeField] List<GameObject> mCamObjs = new List<GameObject>();

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

        //mTimer.DisplayTimer += DisplayTimerAction;
        //mTimer.OnCompleteAct += TimerEndAction;

        Act.OffBoardAction += () =>
        {
            SetBoard(BoardType.offBoard);
        };


        GameStarted = false;
        mBargeObj.SetActive(true);
        no_VehiclesParked = 0;
    }

    private void OnDisable()
    {
        mBoardingStartBtn.onClick.RemoveAllListeners();

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        //mTimer.DisplayTimer -= DisplayTimerAction;
        //mTimer.OnCompleteAct -= TimerEndAction;
    }

    private void DisplayTimerAction(string obj)
    {
        mTime.text = obj;
    }

    private async void TimerEndAction()
    {
        mTime.text = "00:00 mins";
        GameStarted = true;
        //Act.EnableScore();

        await GameCamController.Instance.SetCam(CamType.driver);

        GameTimer.Instance.Resume();

        //mEnginePanel.SetActive(true);
        GamePopUp.Instance.PopStat("Please Press the <color=yellow>[Space Bar]</color> to start the engine", 3.0f);
        mBargeObj.SetActive(false);
        try
        {
           // this.gameObject.SetActive(false);

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

                currentboardData = new BoardData();
                currentboardData.vehicleDatas = new List<Vehicle>();

                Data.passengerCount = passengerCount;
                Data.carCount = carCount;
                Data.truckCount = truckCount;
    
                mBoardingPanel.SetActive(false);

                //mTimer.Begin(120); // main game logic ,enable after final build
                //mTime.text = mTimer.Display;

                GameTimer.Instance.EnableTime();

                PassengerBoarding(passengerCount, () =>
                {
                    CarBoarding(carCount, () =>
                    {
                        TruckBoarding(truckCount, () =>
                        {
                            Score_System.Instance.Set(score);
                            Act.EnableUser(true);

                            GameCamController.Instance.EnableSecondCams(true);

                            currentboardData.vehicleDatas.ForEach(vehicle => vehicle.gameObject.SetActive(false));

                            TimerEndAction();
                        });
                    });
                });

                break;


            case BoardType.offBoard:

                Time.timeScale = 1.0f;
                GameCamController.Instance.EnableSecondCams(false);

                GameStarted = false;
                Act.EnableUser(false);

                mBargeObj.SetActive(true);

                mBargeObj.transform.SetPositionAndRotation(boardPlace.offBoardBarge.position, boardPlace.offBoardBarge.rotation);
                mBargeObj.transform.localScale = boardPlace.offBoardBarge.transform.localScale;

                //boardPlace.boatObj.transform.position = boardPlace.offBoat.position;
                //boardPlace.boatObj.transform.rotation = boardPlace.offBoat.rotation;
                //boardPlace.boatObj.localScale = boardPlace.offBoat.transform.localScale;

                boardPlace.boatObj.SetActive(false);
                boardPlace.boatObjOFF.SetActive(true);

                PassengerOffBoarding(currentboardData.passengerData, () =>
                {
                    DevDebug.Log("All passengers are off boarded !!", DebugColor.Green);

                    VehicleOffBoarding(currentboardData.vehicleDatas, () =>
                    {
                        DevDebug.Log("All vehicles are off boarded !!", DebugColor.Green);

                        Score_System.Instance.UploadFinalScore(Data.time, () =>
                        {
                            LoadingScreen.Instance.LoadSceneAsync(SceneEnum.Home, SceneEnum.Game);
                        });
                    });

                });

                break;
        }
    }


    #region ON_BOARDING
    public async void PassengerBoarding(int count,Action onComplete = null)
    {
        if (count <= 0)
        {
            onComplete?.Invoke();
            return;
        }

        currentboardData.passengerData = new List<NPC>();

        // Data.boardData.passengerData.Clear();

       GameCamController.Instance.SetCam(CamType.passengerOnBoard);

        for (int i = 0;  i < count; i++)
        {
            GameTimer.Instance.Resume();
            var npcPrefab = mNPCStore.GetRandomNPC();
            if(npcPrefab == null)
            {
                Debug.LogWarning("NPC RECIEVED WAS NULL");
                continue;
            }

            _cts = new CancellationTokenSource();
            DevDebug.Log($"Loading the passenger: [{i}]", DebugColor.Brown);

            //GameObject go = Instantiate(mNPCStore.GetRandomNPC().gameObject);
            GameObject go = Instantiate(npcPrefab.gameObject);

            NPC npc = go.GetComponent<NPC>();
            npc.SetBoard(BoardType.onBoard);

            npc.SetPathDuration(5);

            go.SetActive(true);

            await npc.WaitForPathComplete(_cts.Token);

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            GameTimer.Instance.Pause();

            DevDebug.Log($"NPC:{i} is reached ", DebugColor.Orange);
            go.SetActive(false);

           // score += 2;
           // DisplayScore();

            currentboardData.passengerData.Add(npc);

            //try
            //{
            //    await npc.WaitForPathComplete(_cts.Token);

            //    mTimer.Pause();

            //    Destroy(go.gameObject, 0.2f);
            //    DevDebug.Log($"NPC:{i} is reached ", DebugColor.Orange);
            //}
            //catch(TaskCanceledException)
            //{
            //    Debug.LogWarning($"NPC:{i} Error");
            //    return;
            //}  
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


       GameCamController.Instance.SetCam(CamType.vehicleOnBoard);

        for (int i = 0; i < count; i++)
        {
            GameTimer.Instance.Resume();

            GameObject go = Instantiate(mNPCStore.GetRandomCar().gameObject);


            Transform target_location = VehicleDockLoadingLocations.instance.vehicle_Loc[i];

            Vehicle vehicle = go.GetComponent<Vehicle>();
            PathFollower vehicle_loc = go.GetComponent<PathFollower>();
            vehicle.BookParkingArea(target_location);

            vehicle_loc._orientToPath = true;
             // this is for total number of pre-fixed waypoints , excluding the ones that gets added during gameplay for each vehicle 
            vehicle_loc.InsertWaypoint(i);
            vehicle_loc.FixedWayPoints();

            vehicle.SetPathDuration(30);

            go.SetActive(true);

            _cts = new CancellationTokenSource();

            //try
            //{
            //    await vehicle.WaitForPathComplete(_cts.Token);
            //}
            //catch(TaskCanceledException)
            //{
            //    return;
            //}

            await vehicle.WaitForPathComplete(_cts.Token);

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            GameTimer.Instance.Pause();
            //Destroy(go.gameObject, 0.2f);
            DevDebug.Log($"Car:{i} is reached ", DebugColor.Orange);
            no_VehiclesParked += 1;
            Debug.LogWarning($"Vehicles Parked {no_VehiclesParked}");

           // score += 10;
            //DisplayScore();

            currentboardData.vehicleDatas.Add(vehicle);
           // vehicle.gameObject.SetActive(false);

            // vehicle_loc.RemoveWayPoint();

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

        await GameCamController.Instance.SetCam(CamType.vehicleOnBoard);

        for (int i = 0; i < count; i++)
        {
            GameTimer.Instance.Resume();

            GameObject go = Instantiate(mNPCStore.GetRandomTruck().gameObject);

            Vehicle vehicle = go.GetComponent<Vehicle>();
            PathFollower tvehicle_loc = go.GetComponent<PathFollower>();
        // this is for total number of pre-fixed waypoints , excluding the ones that gets added during gameplay for each vehicle 
            tvehicle_loc.InsertWaypoint(i + no_VehiclesParked);
            tvehicle_loc.FixedWayPoints();
            vehicle.SetPathDuration(60);

            _cts = new CancellationTokenSource();

            await vehicle.WaitForPathComplete(_cts.Token);

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            GameTimer.Instance.Pause();

            //Destroy(go.gameObject, 0.2f);
            DevDebug.Log($"Truck:{i} is reached ", DebugColor.Orange);

            currentboardData.vehicleDatas.Add(vehicle);
            //vehicle.gameObject.SetActive(false);


            //score += 25;
            //DisplayScore();
            //tvehicle_loc.RemoveWayPoint();
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

        DisplayScore();

        if (time <=0)
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

    #endregion

    #region OFF_BOARDING

    public async void PassengerOffBoarding(List<NPC> nPCs, Action onComplete)
    {
        if ( nPCs == null || nPCs.Count <=0)
        {
            Debug.LogError("Passeingers cant be found or set");
            onComplete?.Invoke();
            return;
        }

        await GameCamController.Instance.SetCam(CamType.passengerOffBoard);

        for (int i = 0; i < nPCs.Count; i++)
        {
            GameTimer.Instance.Resume();

            nPCs[i].Path.Stop();
            nPCs[i].SetBoard(BoardType.offBoard);

            //npcPlaces[i].passenger = nPCs[i];
            //npcPlaces[i].isBooked = true;

           // nPCs[i].transform.SetLocalPositionAndRotation(npcPlaces[i].place.position, npcPlaces[i].place.rotation);
        }

        for (int i = 0; i < nPCs.Count; i++)
        {
            GameTimer.Instance.Resume();

            NPC npc = nPCs[i];

            npc.gameObject.SetActive(true);

            npc.SetPathDuration(5);
            npc.Path.Play();

            _cts = new CancellationTokenSource();

            DevDebug.Log($"Passenger:{i} unloading..", DebugColor.Green);

            await npc.WaitForPathComplete(_cts.Token);

            int val = 2;

            Score_System.Instance.Set(val, Data.time);

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            GameTimer.Instance.Pause();

            //  place.passenger
        }

        onComplete?.Invoke();
    }

    public async void VehicleOffBoarding(List<Vehicle> vehicles, Action onComplete)
    {
        if (vehicles == null || vehicles.Count <= 0)
        {
            Debug.LogError("Vehicles cant be found or set");
            onComplete?.Invoke();
            return;
        }

        await GameCamController.Instance.SetCam(CamType.vehicleOffBoard);

        for (int i = 0; i < vehicles.Count; ++i)
        {
            GameTimer.Instance.Resume();

            Vehicle vehicle = vehicles[i];

            vehicle.Path.Stop();
            vehicle.SetBoard(BoardType.offBoard);
            vehicle.gameObject.SetActive(true);

            vehicle.Path.Play();

            _cts = new CancellationTokenSource();

            await vehicle.WaitForPathComplete(_cts.Token);

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            int val = vehicle.Type switch
            { 
                BoardCharType.passenger => 2,
                BoardCharType.car => 10,
                BoardCharType.truck => 25
             };

            Score_System.Instance.Set(val, Data.time);

            GameTimer.Instance.Pause();
        }

        onComplete?.Invoke();
    }

    #endregion

    public void DisplayScore() { mScore.text = $"{score}"; }

    IEnumerator OverTwoMinuteLimit()
    {
        GamePopUp.Instance.FinalPopUp("Warning \n You have crossed the 2 min limit");
        yield return new WaitForSecondsRealtime(5f);
        GamePopUp.Instance.ClosePanel();
    }

    private void OnDestroy()
    {
        mBoardingStartBtn.onClick.RemoveAllListeners();

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        //mTimer.DisplayTimer -= DisplayTimerAction;
        //mTimer.OnCompleteAct -= TimerEndAction;
    }

}

[Serializable]
public class BoardPlace
{
    public Transform onBoardBarge;
    public Transform offBoardBarge;

    [Space]
    public GameObject boatObj;
    public GameObject boatObjOFF;

    public Transform onBoat;
    public Transform offBoat;
}

[Serializable]
public class NPCPlace
{
    public NPC passenger;
    public bool isBooked;
    public Transform place;
}


public enum BoardType { onBoard,offBoard}

public enum BoardCharType {passenger,car,truck }
