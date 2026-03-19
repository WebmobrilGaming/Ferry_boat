using Ferry_boat.Assets.Scripts.Web;
using TMPro;
using UnityEngine;

public class LeaderboardCell : MonoBehaviour,ICell
{
    public TMP_Text rankTxt;
    public TMP_Text userNameTxt;
    public TMP_Text scoreTimeTxt;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Initialize(PlayerData playerData)
    {
        rankTxt.text = $"# {playerData.rank}";
        userNameTxt.text = $"{playerData.username}";
        scoreTimeTxt.text = $"{playerData.score}";
    }
}
