#if UNITY_ANDROID && !UNITY_EDITOR
#define CRITTERCISM_ENABLED
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class CrittercismAndroid
{
	/// <summary>
	/// Show debug and log messaged in the console in release mode.
	/// If true CrittercismIOS logs will not appear in the console.
	/// </summary>
	static bool _ShowDebugOnOnRelease = true;
	private static bool isInitialized = false;
	private static readonly string CRITTERCISM_CLASS = "com.crittercism.app.Crittercism";
#if CRITTERCISM_ENABLED
	private static AndroidJavaClass mCrittercismsPlugin = null;
#endif

	/// <summary>
	/// Description:
	/// Start Crittercism for Unity, will start crittercism for android if it is not already active.
	/// Parameters:
	/// appID: Crittercisms Provided App ID for this application
	/// </summary>
	public static void Init (string appID)
	{
		Init (appID, new CrittercismConfig ());
	}

	public static void Init (string appID, CrittercismConfig config)
	{
#if CRITTERCISM_ENABLED
		if (isInitialized) {
			UnityEngine.Debug.Log ("CrittercismAndroid is already initialized.");
			return;
		}
		UnityEngine.Debug.Log ("Initializing Crittercism with app id " + appID);
		mCrittercismsPlugin = new AndroidJavaClass (CRITTERCISM_CLASS);
		if (mCrittercismsPlugin == null) {
			UnityEngine.Debug.Log ("CrittercismAndroid failed to initialize.  Unable to find class " + CRITTERCISM_CLASS);
			return;
		}
		using (var cls_UnityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
			using (var objActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity")) {
				_CallPluginStatic ("initialize", objActivity, appID, config.GetAndroidConfig ());
			}
		}
		System.AppDomain.CurrentDomain.UnhandledException += _OnUnresolvedExceptionHandler;
		Application.RegisterLogCallback (_OnDebugLogCallbackHandler);
		isInitialized = true;
#else
		UnityEngine.Debug.Log ("CrittercismAndroid only supports the Android platform. Crittercism will not be enabled");
#endif
	}

	private static string StackTrace (System.Exception e)
	{
		// Allowing for the fact that the "name" and "reason" of the outermost
		// exception e are already shown in the Crittercism portal, we don't
		// need to repeat that bit of info.  However, for InnerException's, we
		// will include this information in the StackTrace .  The horizontal
		// lines (hyphens) separate InnerException's from each other and the
		// outermost Exception e .
		string answer = e.StackTrace;
		// Using seen for cycle detection to break cycling.
		List<System.Exception> seen = new List<System.Exception> ();
		seen.Add (e);
		if (answer != null) {
			// There has to be some way of telling where InnerException ie stacktrace
			// ends and main Exception e stacktrace begins.  This is it.
			answer = ((e.GetType ().FullName + " : " + e.Message + "\r\n")
				+ answer);
			System.Exception ie = e.InnerException;
			while ((ie != null) && (seen.IndexOf(ie) < 0)) {
				seen.Add (ie);
				answer = ((ie.GetType ().FullName + " : " + ie.Message + "\r\n")
					+ (ie.StackTrace + "\r\n")
					+ answer);
				ie = ie.InnerException;
			}
		} else {
			answer = "";
		}
		return answer;
	}
	
	/// <summary>
	/// Log an exception that has been handled in code.
	/// This exception will be reported to the Crittercism portal.
	/// </summary>
	public static void LogHandledException (System.Exception e)
	{
		string name = e.GetType ().FullName;
		string message = e.Message;
		string stack = StackTrace (e);
		_CallPluginStatic ("_logHandledException", name, message, stack);
	}

	private static void LogUnhandledException (System.Exception e)
	{
		string name = e.GetType ().FullName;
		string message = e.Message;
		string stack = StackTrace (e);
		_CallPluginStatic ("_logCrashException", name, message, stack);
	}

	/// <summary>
	/// Retrieve whether the user is optted out of Crittercism.
	/// </summary>
	public static bool GetOptOut ()
	{
		if (!isInitialized) {
			return false;
		}
		return _CallPluginStatic<bool> ("getOptOutStatus");
	}


	/// <summary>
	/// Set if whether the user is opting to use crittercism
	/// </summary></param>
	public static void SetOptOut (bool optOutStatus)
	{
		if (!isInitialized) {
			return;
		}
		_CallPluginStatic<bool> ("setOptOutStatus", optOutStatus);
	}

	/// <summary>
	/// Set the Username of the user
	/// This will be reported in the Crittercism Meta.
	/// </summary>
	static public void SetUsername (string username)
	{
		if (!isInitialized) {
			return;
		}
		_CallPluginStatic ("setUsername", username);
	}

	/// <summary>
	/// Add a custom value to the Crittercism Meta.
	/// </summary>
	static public void SetMetadata (string[] keys, string[] values)
	{
		if (!isInitialized) {
			return;
		}
		if (keys.Length != values.Length) {
			UnityEngine.Debug.Log ("Crittercism.SetMetadata given arrays of different lengths");
			return;
		}
		for (int i = 0; i < keys.Length; i++) {
			SetValue (keys [i], values [i]);
		}
	}

	static public void SetValue (string key, string value)
	{
		if (!isInitialized) {
			return;
		}
#if CRITTERCISM_ENABLED
		using (var jsonObject = new AndroidJavaObject ("org.json.JSONObject")) {
			jsonObject.Call<AndroidJavaObject> ("put", key, value);

			//TODO: using AndroidJavaClass and AndroidJavaObject can be really expensive in C#
			//consider add a overload method void setMetadata(string key, string value) in java side
			_CallPluginStatic ("setMetadata", jsonObject);
		}
#endif
	}

	/// <summary>
	/// Leave a breadcrumb for tracking.
	/// </summary>
	static public void LeaveBreadcrumb (string breadcrumb)
	{
		if (!isInitialized) {
			return;
		}
		_CallPluginStatic ("leaveBreadcrumb", breadcrumb);
	}
		
	/// <summary>
	/// Begin a transaction to track ex. login
	/// </summary>
	static public void BeginTransaction (string transactionName)
	{
		if (!isInitialized) {
			return;
		}
		_CallPluginStatic ("beginTransaction", transactionName);
	}
		
	/// <summary>
	/// Ends a tracked transaction ex. login was successful
	/// </summary>
	static public void EndTransaction (string transactionName)
	{
		if (!isInitialized) {
			return;
		}
		_CallPluginStatic ("endTransaction", transactionName);
	}
		
	/// <summary>
	/// Fails a tracked transaction ex. login error
	/// </summary>
	static public void FailTransaction (string transactionName)
	{
		if (!isInitialized) {
			return;
		}
		_CallPluginStatic ("failTransaction", transactionName);
	}
		
	/// <summary>
	/// Set a value for a transaction ex. shopping cart value
	/// </summary>
	static public void SetTransactionValue (string transactionName, int value)
	{
		if (!isInitialized) {
			return;
		}
		_CallPluginStatic ("setTransactionValue", transactionName, value);
	}
		
	/// <summary>
	/// Get the current value of the tracked transaction
	/// </summary>
	static public int GetTransactionValue (string transactionName)
	{
		if (!isInitialized) {
			return -1;
		}
		return _CallPluginStatic<int> ("getTransactionValue", transactionName);
	}
		
	static private void _OnUnresolvedExceptionHandler (object sender, System.UnhandledExceptionEventArgs args)
	{
		if (!isInitialized || args == null || args.ExceptionObject == null) {
			return;
		}
		if (args.ExceptionObject.GetType () != typeof(System.Exception)) {
			return;
		}
		LogUnhandledException ((System.Exception)args.ExceptionObject);
	}

	static private void _OnDebugLogCallbackHandler (string name, string stack, LogType type)
	{
		if (LogType.Assert != type && LogType.Exception != type) {
			return;
		}
		if (!isInitialized) {
			return;
		}
#if CRITTERCISM_ENABLED
		if (LogType.Exception == type || LogType.Assert == type) {
			if (Application.platform == RuntimePlatform.Android) {
				_CallPluginStatic ("_logCrashException", name, name, stack);
			}
		}
#endif
	}

	static private void _CallPluginStatic (string methodName, params object[] args)
	{
#if CRITTERCISM_ENABLED
		mCrittercismsPlugin.CallStatic (methodName, args);
#endif
	}

	static private RetType _CallPluginStatic<RetType> (string methodName, params object[] args)
	{
#if CRITTERCISM_ENABLED
		return mCrittercismsPlugin.CallStatic<RetType> (methodName, args);
#else
		return default (RetType);
#endif
	}
}
