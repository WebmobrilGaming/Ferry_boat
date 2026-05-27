using FerryBoat.Actions;
using System.Collections;
using UnityEngine;

public class GameCam : MonoBehaviour
{
    private bool isHit;
    private void OnEnable()
    {
        isHit = false;
        Act.HitAction += Hit;
    }

    private void OnDisable()
    {
        Act.HitAction -= Hit;
    }

    void Hit()
    {
        Debug.LogWarning("Playing Vibration Functionality");
        int vib = PlayerPrefs.GetInt(GamePrefs.isVibrationOn,1);
        if(vib == 1)
        {
            StartCoroutine(Shake(0.2f, 0.1f));
            isHit = true;
        }
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        if (!isHit)
        {
            
            Vector3 originalPos = transform.localPosition;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(x, y, originalPos.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPos;
        }
        
    }
}
