#if UNITY_ANDROID && !UNITY_EDITOR
#define CRITTERCISM_ENABLED
#endif
using UnityEngine;
using System.Collections;

public class CrittercismConfig
{
	private static readonly string CRITTERCISM_CONFIG_CLASS = "com.crittercism.app.CrittercismConfig";
#if CRITTERCISM_ENABLED
	private AndroidJavaObject mCrittercismConfig = null;

	public CrittercismConfig ()
	{
		mCrittercismConfig = new AndroidJavaObject (CRITTERCISM_CONFIG_CLASS);
	}

	public AndroidJavaObject GetAndroidConfig ()
	{
		return mCrittercismConfig;
	}
#endif

	public string GetCustomVersionName ()
	{
		return CallConfigMethod<string> ("getCustomVersionName");
	}

	public void SetCustomVersionName (string customVersionName)
	{
		CallConfigMethod ("setCustomVersionName", customVersionName);
	}

	public bool IsLogcatReportingEnabled ()
	{
		return CallConfigMethod<bool> ("isLogcatReportingEnabled");
	}

	public void SetLogcatReportingEnabled (bool shouldCollectLogcat)
	{
		CallConfigMethod ("setLogcatReportingEnabled", shouldCollectLogcat);
	}

	public bool IsServiceMonitoringEnabled ()
	{
		return CallConfigMethod<bool> ("isServiceMonitoringEnabled");
	}

	public void SetServiceMonitoringEnabled (bool isServiceMonitoringEnabled)
	{
		CallConfigMethod ("setServiceMonitoringEnabled", isServiceMonitoringEnabled);
	}

	void CallConfigMethod (string methodName, params object[] args)
	{
#if CRITTERCISM_ENABLED
		mCrittercismConfig.Call (methodName, args);
#endif
	}

	RetType CallConfigMethod<RetType> (string methodName, params object[] args)
	{
#if CRITTERCISM_ENABLED
		return mCrittercismConfig.Call<RetType> (methodName, args);
#else
		return default(RetType);
#endif
	}
}
