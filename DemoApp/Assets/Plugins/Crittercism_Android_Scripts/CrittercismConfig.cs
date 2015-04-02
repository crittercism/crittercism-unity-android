using UnityEngine;
using System.Collections;

public class CrittercismConfig : MonoBehaviour
{
	private static readonly string CRITTERCISM_CONFIG_CLASS = "com.crittercism.app.CrittercismConfig";
	private AndroidJavaObject mCrittercismConfig = null;

	public CrittercismConfig ()
	{
		mCrittercismConfig = new AndroidJavaObject(CRITTERCISM_CONFIG_CLASS);
	}

	public string GetCustomVersionName ()
	{
		return mCrittercismConfig.Call<string>("getCustomVersionName");
	}

	public void SetCustomVersionName (string customVersionName)
	{
		mCrittercismConfig.Call ("setCustomVersionName", customVersionName);
	}

	public bool IsLogcatReportingEnabled ()
	{
		return mCrittercismConfig.Call<bool> ("isLogcatReportingEnabled");
	}

	public void SetLogcatReportingEnabled (bool shouldCollectLogcat)
	{
		mCrittercismConfig.Call ("setLogcatReportingEnabled", shouldCollectLogcat);
	}

	public AndroidJavaObject GetAndroidConfig () {
		return mCrittercismConfig;
	}
}

