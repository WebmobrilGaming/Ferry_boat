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

    [Header("SecondCams:")]
    [SerializeField] List<Camera> secondCams = new List<Camera>();

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

    public void EnableSecondCams(bool enable)
    {
        secondCams.ForEach(cam =>
        {
            if (enable)
            {
                cam.enabled = true;
                // Restore these if needed
                cam.clearFlags = CameraClearFlags.Skybox; // or your original value
                cam.cullingMask = ~0; // Render all layers
            }
            else
            {
                cam.enabled = true; // Keep camera active so it renders black
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Color.black;
                cam.cullingMask = 0; // Render nothing
            }
        });

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
