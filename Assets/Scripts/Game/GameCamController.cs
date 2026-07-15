using UnityEngine;

using System.Collections.Generic;

public class GameCamController : MonoBehaviour
{
    static GameCamController instance;

    public static GameCamController Instance { get { return instance; }  }

    [SerializeField] CamType camType;
    public CamType Cam => camType;

    [Header("Cams")] 
    [SerializeField] List<Cam> cams = new List<Cam>();

    private void Awake()
    {
        if(instance == null)
            instance = this;    
    }

    private void OnEnable()
    {
        SetCam(CamType.driver);
    }

    public void SetCam(CamType camType)
    {
        cams.ForEach(x => x.SetPriority(false));
        cams.Find(x => x.Type == camType).SetPriority(true);
    }
}

public enum  CamType
{
   driver,passengerOnBoard,vehicleOnBoard,passengerOffBoard,vehicleOffBoard
}
