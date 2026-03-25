using DebugUtils;
using DG.Tweening;
using FerryBoat;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInterFace : MonoBehaviour
{
    [SerializeField] InputAction scrollAction;

    [Space]
    [SerializeField] float mScrollRange;

    float rangeMin = 0f;
    float rangeMax = 10f;
    [SerializeField] float sensitivity = 1f;
    public Transform Info;
    public TMP_Text InfoTxt;
    private Coroutine dockMissedRoutine;

    private void Awake()
    {
        rangeMax = -1;
        scrollAction.performed += OnScroll;
    }

    private void OnEnable()
    {
        scrollAction.Enable();
        Destination.OnFerryMissedDockEvent += OnFerryMissedDock;
        Destination.OnEnterDockEvent += OnEnterDock;
        Act.SpeedInit += SetRange;
        Act.ReachedDestination += LevelFinish;
        Act.BoatDestroyedAction += BoatDestroyed;
    }

    private void OnEnterDock()
    {
        InfoTxt.text="You entered in the dock area";
        if (dockMissedRoutine != null)
        {
            StopCoroutine(dockMissedRoutine);
            dockMissedRoutine = null;
        }
        dockMissedRoutine = StartCoroutine(ShowMissedDock());
    }

    private void OnFerryMissedDock()
    {
        InfoTxt.text="You missed the dock";
        if (dockMissedRoutine != null)
        {
            StopCoroutine(dockMissedRoutine);
            dockMissedRoutine = null;
        }
        dockMissedRoutine = StartCoroutine(ShowMissedDock());
    }
    private IEnumerator ShowMissedDock()
    {
        Info.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(2f);
        Info.DOScale(0, 0.5f).SetEase(Ease.InBounce);
    }
    private void OnDisable()
    {
        scrollAction.Disable();

        Act.SpeedInit -= SetRange;
        Destination.OnEnterDockEvent -= OnEnterDock;
        Destination.OnFerryMissedDockEvent -= OnFerryMissedDock;
        Act.ReachedDestination -= LevelFinish;
    }

    private void BoatDestroyed()
    {
        Time.timeScale = 0;
        GamePopUp.Instance.FinalPopUp("Boat destroyed !!");

        DOVirtual.DelayedCall(2.0f, () => { Time.timeScale = 1; SceneManager.Instance.LoadScene(SceneType.Home); });
    }

    public void ExitAction()
    {
        Time.timeScale = 1;
        SceneManager.Instance.LoadScene(SceneType.Home);
    }

    private void OnDestroy()
    {
        scrollAction.performed -= OnScroll;
    }

    private void LevelFinish()
    {
        Time.timeScale = 0;
        GamePopUp.Instance.FinalPopUp("Destination Reached");
    }

    public void SetRange(float range)
    {
        rangeMin = 0;
        rangeMax = range;
    }

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        if (rangeMax <= 0)
            return;

        Vector2 scroll = ctx.ReadValue<Vector2>();

        float mScrollVal = -1 * scroll.y;

        mScrollRange = Mathf.Clamp(mScrollRange + mScrollVal * sensitivity, rangeMin, rangeMax);

        Act.SpeedChange?.Invoke(mScrollRange);
    }
}
