using System;
using System.Collections.Generic;
using DG.Tweening;
using Ferry.Screens;
using Ferry_boat.Assets.Scripts.Web;
using GF;
using PolyAndCode.UI;
using UnityEngine.UI;

public class LeaderboardScreen : BaseScreen<ScreenType>, IRecyclableScrollRectDataSource
{
    public RecyclableScrollRect recyclableScrollRect;
    public List<PlayerData> leaderboardList;
    public Button backBtn;
    public Button easyBtn;
    public Button mediumBtn;
    public Button HardBtn;
    protected override void OnEnable()
    {
        backBtn.AddListener(null,Close);
        backBtn.interactable = false;
        easyBtn.onClick.AddListener(OnEasyCliked);
        mediumBtn.onClick.AddListener(OnMediumCliked);
        HardBtn.onClick.AddListener(OnHardCliked);
        APIManager.GetAPI<LeaderboardResponse>(new RequestData(Netconfig.RequestType.LeaderBoard_Easy, null),OnRecieveLeaderboard);
    }

    private void OnHardCliked()
    {
        APIManager.GetAPI<LeaderboardResponse>(new RequestData(Netconfig.RequestType.LeaderBoard_Hard, null), OnRecieveLeaderboard);
        backBtn.interactable = false;
    }

    private void OnMediumCliked()
    {
        APIManager.GetAPI<LeaderboardResponse>(new RequestData(Netconfig.RequestType.LeaderBoard_Medium, null), OnRecieveLeaderboard);
        backBtn.interactable = false;
    }

    private void OnEasyCliked()
    {
        APIManager.GetAPI<LeaderboardResponse>(new RequestData(Netconfig.RequestType.LeaderBoard_Easy, null), OnRecieveLeaderboard);
        backBtn.interactable = false;
    }

    private void Close()
    {
        SwitchScreen(ScreenType.Home);
    }

    private void OnRecieveLeaderboard(LeaderboardResponse data, Response response)
    {
        if (response.status)
        {
            leaderboardList=data.data;
            int rank=1;
            for(int i =0; i<leaderboardList.Count; i++)
            {
                leaderboardList[i].rank=rank;
                rank++;
            }
            Invoke(nameof(InitializeLeaderboard),0.2f);

        }
        else
        {
            Utils.ShowOkPopup("Error!",response.message,null);
        }
       
    }

    private void InitializeLeaderboard()
    {
        recyclableScrollRect.Initialize(this);
        backBtn.interactable = true;
    }
    public int GetItemCount()
    {
        return leaderboardList.Count;
    }

    public int GetStartingIndex()
    {
        return 0;
    }

    public void PageChanged(int index)
    {

    }

    public void SetCell(ICell cell, int index)
    {
        LeaderboardCell leaderboardCell = cell as LeaderboardCell;
        leaderboardCell.Initialize(leaderboardList[index]);
    }
    protected override void OnDisable()
    {
        backBtn.RemoveListener();
    }

    


}
