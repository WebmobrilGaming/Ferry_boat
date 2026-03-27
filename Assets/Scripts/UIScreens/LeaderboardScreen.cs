using System;
using System.Collections.Generic;
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
    protected override void OnEnable()
    {
        backBtn.AddListener(null,Close);
        APIManager.GetAPI<LeaderboardResponse>(new RequestData(Netconfig.RequestType.LeaderBoard, null),OnRecieveLeaderboard);
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
    }

    private void InitializeLeaderboard()
    {
        recyclableScrollRect.Initialize(this);
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
