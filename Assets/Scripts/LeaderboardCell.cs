using Ferry_boat.Assets.Scripts.Web;
using TMPro;
using UnityEngine;

public class LeaderboardCell : MonoBehaviour,ICell
{
    public TMP_Text rankTxt;
    public TMP_Text userNameTxt;
    public TMP_Text scoreTimeTxt;
    public TMP_Text timeText;
    [SerializeField] private float min;
    [SerializeField] private float sec;


    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Initialize(PlayerData playerData)
    {
        rankTxt.text = $"# {playerData.rank}";
        userNameTxt.text = $"{playerData.username}";
        scoreTimeTxt.text = $"{playerData.score}";
        min = Mathf.FloorToInt(playerData.totalTimeSecond/60);
        sec = Mathf.FloorToInt(playerData.totalTimeSecond%60);
        timeText.text=string.Format("{0:00}:{1:00}",min,sec);
    }
}
