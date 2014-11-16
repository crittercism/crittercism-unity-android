using UnityEngine;
using System.Collections;

public class CrittercismInit : MonoBehaviour
{
    /// <summary>
    /// Your Crittercism App ID.  Every app has a special identifier that allows Crittercism
    /// to associate error monitoring data with your app.  Your app ID can be found on the
    /// "App Settings" page of the app you are trying to monitor.
    /// See the Crittercism portal https://app.crittercism.com/
    /// </summary>
    /// <example>A real app ID looks like this:  5671d3b6f7c78a7243000a05</example>
	private const string CrittercismAppID = "5212a00d4002056b0b000004";

    void Awake ()
    {
        CrittercismAndroid.Init (CrittercismAppID);
        Destroy (this);
    }
}
