#if UNITY_ANDROID && !UNITY_EDITOR
#define CRITTERCISM_ENABLED
#endif

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

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

		static private void doLogError (string crittercismMethod, System.Exception e)
		{
				if (!isInitialized) {
						return;
				}

				StackTrace stackTrace = new StackTrace (e, true);
				string[] classes = new string[stackTrace.FrameCount];
				string[] methods = new string[stackTrace.FrameCount];
				string[] files = new string[stackTrace.FrameCount];
				int[] lineNumbers = new int[stackTrace.FrameCount];

				for (int i = 0; i < stackTrace.FrameCount; i++) {
						StackFrame frame = stackTrace.GetFrame (i);
						classes [i] = frame.GetMethod ().DeclaringType.Name;
						methods [i] = frame.GetMethod ().Name;
						files [i] = frame.GetFileName ();
						lineNumbers [i] = frame.GetFileLineNumber ();
				}

				_CallPluginStatic (crittercismMethod,
					e.GetType ().Name, e.Message, classes, methods, files, lineNumbers);
		}

		static private void logCrash (System.Exception e)
		{
				doLogError ("_logCrashException", e);
		}

		/// <summary>
		/// Log an exception that has been handled in code.
		/// This exception will be reported to the Crittercism portal.
		/// </summary>
		static public void LogHandledException (System.Exception e)
		{
				doLogError ("_logHandledException", e);
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

				logCrash ((System.Exception)args.ExceptionObject);
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
				try {
						using (var pluginExceptionClass = new AndroidJavaClass ("com.crittercism.integrations.PluginException")) {
								using (var exception = pluginExceptionClass.CallStatic<AndroidJavaObject> ("createUnityException", name, stack)) {
										//TODO: using AndroidJavaClass and AndroidJavaObject can be really expensive in C#
										//consider add a overload method void _logCrashException(string name, string stack) in java side
										_CallPluginStatic ("_logCrashException", exception);
								}
						}
				}
				catch (System.Exception e) {
						UnityEngine.Debug.Log ("Unable to log a crash exception to Crittercism to to an unexpected error: " + e.ToString ());
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
