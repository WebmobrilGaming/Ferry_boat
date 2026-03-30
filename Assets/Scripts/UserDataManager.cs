using System;
using Ferry_boat.Assets.Scripts.Web;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager _instance;
    public static UserDataManager Instance=>_instance;
    private UserDetails userDetails;
    public UserDetails UserDetails=>userDetails;
    public static event Action<string> OnLogin;
    void Awake()
    {
        if(_instance==null)
            _instance=this;
    }

    public void SignIn(UserDetails userDetails)
    {
        this.userDetails=userDetails;
        OnLogin?.Invoke(userDetails.data.username);
    }
}
