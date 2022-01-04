using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class RemoteConfig : MonoBehaviour {
	Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    private static RemoteConfig _instance;
    public static RemoteConfig instance
    {
        get{
            if(_instance == null){
                _instance = FindObjectOfType<RemoteConfig>();
            }
            return _instance;
        }
    }
    // public bool checkFetchData=false;
	public void ShowData() {
        try{
            Admob.instance.TimeShowAdmob = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("TimeShowAdmob").LongValue;
        }
        catch(Exception e){
            Admob.instance.TimeShowAdmob = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("TimeShowAdmob").LongValue;
        }
	}

	// Start a fetch request.
	public Task FetchDataAsync() {
		System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
	}

	void FetchComplete(Task fetchTask) {
        if (fetchTask.IsCanceled) {
            Debug.Log("Fetch canceled.");
        } else if (fetchTask.IsFaulted) {
            Debug.Log("Fetch encountered an error.");
        } else if (fetchTask.IsCompleted) {
            Debug.Log("Fetch completed successfully!");
        }
        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus) {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                    .ContinueWithOnMainThread(task => {
                        Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",info.FetchTime));
                        ShowData();
                        // checkFetchData = true;
                    });
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason) {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            Debug.Log("Fetch failed for unknown reason");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                    }
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    Debug.Log("Latest Fetch call still pending.");
                    break;
            }
    }
}
