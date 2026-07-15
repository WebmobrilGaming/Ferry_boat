using UnityEngine;

using System.Collections.Generic;
using System;
using Unity.Cinemachine;
using UnityEngine.Events;

using Cysharp.Threading.Tasks;

public class GameCamController : MonoBehaviour
{
    static GameCamController instance;

    public static GameCamController Instance { get { return instance; }  }

    [SerializeField] CamType camType;
    public CamType Cam => camType;

    [Header("Cams")] 
    [SerializeField] List<Cam> cams = new List<Cam>();

    [Space]
    [SerializeField] CinemachineBrain brain;

    bool isBlended = false;

    private void Awake()
    {
        if(instance == null)
            instance = this;    
    }

    private void OnEnable()
    {
       SetCam(CamType.driver);
    }

    public async UniTask SetCam(CamType camType)
    {
        isBlended = false;

        cams.ForEach(x => x.SetPriority(false));
        cams.Find(x => x.Type == camType).SetPriority(true);

        await UniTask.WaitUntil(() => isBlended);
    }

    private void LateUpdate()
    {
        // Wait until blending is finished
        if (!brain.IsBlending )
        {
           isBlended = true;
        }
    }
}

public enum  CamType
{
   driver,passengerOnBoard,vehicleOnBoard,passengerOffBoard,vehicleOffBoard
}
