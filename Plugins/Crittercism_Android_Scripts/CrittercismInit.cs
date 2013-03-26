using UnityEngine;
using System.Collections;

public class CrittercismInit : MonoBehaviour {
#if UNITY_ANDROID
	private const string CrittercismAppID	= ""; /*Your App ID Here (NOTE! This is a hexadecimal hash, not the name of your app!) */
	private const bool bDelaySendingAppLoad = false;
	private const bool bShouldCollectLogcat = false;
	private const string CustomVersionName = ""; /*Your Custom Version Name Here*/
	void Awake () {
		
#if UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2
#else
		CrittercismAndroid.Init(CrittercismAppID, bDelaySendingAppLoad, bShouldCollectLogcat, CustomVersionName, "Main Camera", "CrittercismInitComplete");
#endif

#if UNITY_3_3 || UNITY_3_4 || UNITY_3_4_1 || UNITY_3_4_2
#else
		Destroy(this);
#endif
	}
	
	void Update() {
		CrittercismAndroid.Update();
	}

	void CrittercismInitComplete(string token) {
		CrittercismAndroid.InitComplete(token);
	}
#endif
}
