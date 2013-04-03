#define FORCE_DEBUG
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public static class CrittercismAndroid
{
#if UNITY_ANDROID
	/// <summary>
	/// Show debug and log messaged in the console in release mode.
	/// If true CrittercismIOS logs will not appear in the console.
	/// </summary>
	static bool _ShowDebugOnOnRelease		= true;
	
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
	private static bool _IsPluginInited			= false;
	private static bool _IsUnityPluginInited	= false;
	private static AndroidJavaClass mCrittercismsPlugin	= null;
#endif
	
	private static void CLog(string log)
	{
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		if(_ShowDebugOnOnRelease || Debug.isDebugBuild && log != null)
		{	Debug.Log("CrittercismAndroid: " + log);	}
#endif
	}
	
	/// <summary>
	/// Description:
	/// Start Crittercism for Unity, will start crittercism for android if it is not already active.
	/// </summary>
	public static void Init(string callbackObjectName, string callbackObjectMethod)
	{	Init("", callbackObjectName, callbackObjectMethod);	}
	
	/// <summary>
	/// Description:
	/// Start Crittercism for Unity, will start crittercism for android if it is not already active.
	/// Parameters:
	/// appID: Crittercisms Provided App ID for this application
	/// </summary>
	public static void Init(string appID, string callbackObjectName, string callbackObjectMethod)
	{
		Init (appID, false, false, null, callbackObjectName, callbackObjectMethod);
	}
		
	/// <summary>
	/// Description:
	/// Start Crittercism for Unity, will start crittercism for android if it is not already active.
	/// Parameters:
	/// appID: Crittercisms Provided App ID for this application
	/// </summary>
	public static void Init(string appID, bool bDelay, bool bSendLogcat, string customVersion, string callbackObjectName, string callbackObjectMethod)
	{
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		try
		{
			_IsPluginInited	= IsInited();
			if(_IsPluginInited && _IsUnityPluginInited)	{	return;	}
			
			if(!_IsUnityPluginInited)
			{
				AndroidJavaClass cls_UnityPlayer	= new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject objActivity		= cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				mCrittercismsPlugin	= new AndroidJavaClass("com.crittercism.unity.CrittercismAndroid");
				if(mCrittercismsPlugin == null)
				{
					CLog("To find Crittercism Plugin");
					throw new System.Exception("ExitError");
				}
				
				mCrittercismsPlugin.CallStatic("SetConfig", bDelay, bSendLogcat, customVersion);
				mCrittercismsPlugin.CallStatic("Init", objActivity, appID, callbackObjectName, callbackObjectMethod);
			}
			
			EnableDebugLog(_ShowDebugOnOnRelease);
		}
		catch(System.Exception e)
		{
			CLog("Failed to initialize Crittercisms: " + e.Message);
			_IsUnityPluginInited	= false;
		}
		CLog(string.Format("Init complete, _IsPluginInited={0}, _IsUnityPluginInited={1}", _IsPluginInited, _IsUnityPluginInited));
#endif
	}
	
	public static void InitComplete(string token) {
		_IsPluginInited	= IsInited();
		CLog(string.Format("InitComplete, _IsPluginInited={0}, _IsUnityPluginInited={1}", _IsPluginInited, _IsUnityPluginInited));
		if(_IsPluginInited)
		{
			System.AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(_OnUnresolvedExceptionHandler);
			Application.RegisterLogCallback(_OnDebugLogCallbackHandler);
			_IsUnityPluginInited	= true;

			CLog("Registered");
		}
		CLog(string.Format("InitComplete complete, _IsPluginInited={0}, _IsUnityPluginInited={1}", _IsPluginInited, _IsUnityPluginInited));
	}

	
	public static void EnableDebugLog(bool b)
	{
		if(mCrittercismsPlugin == null || _IsPluginInited == false)	{	return;	}
		
		try
		{	
			mCrittercismsPlugin.CallStatic("EnableDebugLog", b);
			_ShowDebugOnOnRelease	= b;
			
		}catch(System.Exception e) {	CLog(e.Message); }
	}
	
//	public static void MarkExceptionType(bool b)
//	{
//		if(mCrittercismsPlugin == null || _IsPluginInited == false)	{	return;	}
//		
//		try
//		{	
//			mCrittercismsPlugin.CallStatic("MarkExceptionType", b);
//			
//		}catch(System.Exception e) {	CLog(e.Message); }
//	}
	
	
	public static void Update()
	{
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		if(_IsUnityPluginInited)
		{	Application.RegisterLogCallback(_OnDebugLogCallbackHandler);	}
#endif
	}
	
	/// <summary>
	/// Log an exception that has been handled in code.
	/// This exception will be reported to the Crittercism portal.
	/// </summary>
	static public void LogHandledException(System.Exception e)
	{
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		if(mCrittercismsPlugin == null || _IsPluginInited == false)	{	return;	}
	
		try
		{	
			mCrittercismsPlugin.CallStatic("LogHandledException", e.Source, e.Message, e.StackTrace);
			
		}catch(System.Exception ex) {	CLog(ex.Message); }
#endif
	}
	
	/// <summary>
	/// Retrieve whether the user is opting out of Crittercism.
	/// </summary>
	public static bool GetOptOut()
	{
		bool bRet	= false;
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		
		if(mCrittercismsPlugin == null || _IsPluginInited == false)	{	return false;	}
		try
		{
			bRet	= mCrittercismsPlugin.CallStatic<bool>("GetOptOutStatus");
		}catch(System.Exception e) {	CLog(e.Message); }
		
#endif
		return bRet;
	}
	
	/// <summary>
	/// Set if whether the user is opting to use crittercism
	/// </summary></param>
	public static void SetOptOut(bool s)
	{
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		if(mCrittercismsPlugin == null || _IsPluginInited == false)	{	return;	}
		
		try
		{
			mCrittercismsPlugin.CallStatic<bool>("SetOptOutStatus", s);
			
		}catch(System.Exception e) {	CLog(e.Message); }
#endif
	}
	
	/// <summary>
	/// Set the Username of the user
	/// This will be reported in the Crittercism Meta.
	/// </summary>
    static public void SetUsername(string username)
	{
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		if(mCrittercismsPlugin == null || _IsPluginInited == false)	{	return;	}
		
		try
		{
			mCrittercismsPlugin.CallStatic("SetUsername", username);
			
		}catch(System.Exception e) {	CLog(e.Message); }
#endif
	}
	
	/// <summary>
	/// Add a custom value to the Crittercism Meta.
	/// </summary>
    static public void SetMetadata(string[] key, string[] v)
	{
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		if(mCrittercismsPlugin == null || _IsPluginInited == false)	{	return;	}
		
		try
		{
			mCrittercismsPlugin.CallStatic("SetMetadata", key, v);
			
		}catch(System.Exception e) {	CLog(e.Message); }
#endif
	}
	
	/// <summary>
	/// Leave a breadcrumb for tracking.
	/// </summary>
	static public void LeaveBreadcrumb(string l)
	{
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		if(mCrittercismsPlugin == null || _IsPluginInited == false)	{	return;	}
		
		try
		{
			mCrittercismsPlugin.CallStatic("LeaveBreadcrumb", l);
			
		}catch(System.Exception e) {	CLog(e.Message); }
#endif
	}
	
	/// <summary>
	/// Return if crittercism is inited
	/// </summary>
	static public bool IsInited()
	{
		bool bRet	= false;
		
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		if(mCrittercismsPlugin == null)	{	return false;	}
			
		try
		{
			bRet	= mCrittercismsPlugin.CallStatic<bool>("IsInited");
		}catch(System.Exception e) {	CLog(e.Message); }
#endif
		
		return bRet;
	}

	static private void _OnUnresolvedExceptionHandler(object sender, System.UnhandledExceptionEventArgs args)
	{
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		if(mCrittercismsPlugin == null || _IsPluginInited == false)	{	return;	}
		if(args == null || args.ExceptionObject == null)	{	return;	}
		if(args.ExceptionObject.GetType() != typeof(System.Exception))	{	return;	}
		
		try
		{
			System.Exception e	= (System.Exception)args.ExceptionObject;
			if(args.IsTerminating)
			{
				mCrittercismsPlugin.CallStatic("LogUnhandledException", e.Source, e.Message, e.StackTrace);
			}
			else
			{	LogHandledException(e);	}
		
		}catch(System.Exception e) {	CLog(e.Message); }
		
#endif
	}
	
	static private void _OnDebugLogCallbackHandler(string name, string stack, LogType type)
	{
#if (UNITY_ANDROID && !UNITY_EDITOR) || FORCE_DEBUG
		if(LogType.Assert != type && LogType.Exception != type)	{	return;	}		
		if(mCrittercismsPlugin == null || _IsPluginInited == false)	{	return;	}
		
		try
		{
			mCrittercismsPlugin.CallStatic("LogUnhandledException", name, name, stack);
			
		}catch(System.Exception e) {	CLog(e.Message); }
#endif
	}
#endif
}
