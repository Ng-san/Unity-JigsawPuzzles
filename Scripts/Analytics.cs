using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class Analytics : MonoBehaviour
{
    public void LogEvent3ParamValue(string eventName, string paramName1, string paramName2, string pramName3, string paramValue, string paramValue2, string paramValue3)
    {
        var p = new Parameter[]
        {
            new Parameter(paramName1, paramValue),
            new Parameter(paramName2, paramValue2),
            new Parameter(pramName3, paramValue3)
        };
        FirebaseAnalytics.LogEvent(eventName, p);
    }
    public void AnalyticsLogin() {
      // Log an event with no parameters.
      Debug.Log("Logging a login event.");
      FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
    }
    public void AnalyticsProgress() {
      // Log an event with a float.
      Debug.Log("Logging a progress event.");
      FirebaseAnalytics.LogEvent("progress", "percent", 0.4f);
    }
    public void ResetAnalyticsData() {
      Debug.Log("Reset analytics data.");
      FirebaseAnalytics.ResetAnalyticsData();
    }

}
