using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class Loading : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingTxt;
    [SerializeField] private bool startLoading;

    void OnEnable()
    {
        startLoading = true;
        StartLoader();
    }

    void StartLoader()
    {
        StartCoroutine(Loader());
    }

   IEnumerator Loader()
    {
        loadingTxt.text = "Loading.";
        yield return new WaitForSecondsRealtime(0.5f);
        loadingTxt.text ="Loading..";
        yield return new WaitForSecondsRealtime(0.5f);
        loadingTxt.text = "Loading...";
        yield return new WaitForSecondsRealtime(0.5f);
        StartLoader();
    }
    void OnDisable()
    {
        StopAllCoroutines();
        
    }
}
