
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
		private static AndroidJavaClass mCrittercismsPlugin = null;

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
				if (Application.platform != RuntimePlatform.Android) {
						System.Console.Write ("CrittercismAndroid only supports the Android platform. Crittercism will not be enabled");
						return;
				}

				if (isInitialized) {
						System.Console.Write ("CrittercismAndroid is already initialized.");
						return;
				}

				UnityEngine.Debug.Log ("Initializing Crittercism with app id " + appID);

				AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
				AndroidJavaObject objActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

				mCrittercismsPlugin = new AndroidJavaClass (CRITTERCISM_CLASS);
				if (mCrittercismsPlugin == null) {
						System.Console.Write ("CrittercismAndroid failed to initialize.  Unable to find class " + CRITTERCISM_CLASS);
						return;
				}

				mCrittercismsPlugin.CallStatic ("initialize", objActivity, appID, config.GetAndroidConfig ());

				System.AppDomain.CurrentDomain.UnhandledException += _OnUnresolvedExceptionHandler;
				Application.RegisterLogCallback (_OnDebugLogCallbackHandler);

				isInitialized = true;
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
        
        
				mCrittercismsPlugin.CallStatic (
            crittercismMethod, 
            e.GetType ().Name, 
            e.Message, 
            classes, 
            methods, 
            files, 
            lineNumbers);

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

				return mCrittercismsPlugin.CallStatic<bool> ("getOptOutStatus");
		}


		/// <summary>
		/// Set if whether the user is opting to use crittercism
		/// </summary></param>
		public static void SetOptOut (bool s)
		{
				if (!isInitialized) {
						return;
				}

				mCrittercismsPlugin.CallStatic<bool> ("setOptOutStatus", s);
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

				mCrittercismsPlugin.CallStatic ("setUsername", username);
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
						System.Console.Write ("Crittercism.SetMetadata given arrays of different lengths");
						return;
				}

				for (int i = 0; i < keys.Length; i++) {
						SetValue (keys [i], values [i]);
				}
		}

		static public void SetValue (string key, string value)
		{
				AndroidJavaClass jsonObject = new AndroidJavaClass ("org.json.JSONObject");
				jsonObject.Call ("put", key, value);

				mCrittercismsPlugin.CallStatic ("setMetadata", jsonObject);
		}

		/// <summary>
		/// Leave a breadcrumb for tracking.
		/// </summary>
		static public void LeaveBreadcrumb (string l)
		{
				if (!isInitialized) {
						return;
				}

				mCrittercismsPlugin.CallStatic ("leaveBreadcrumb", l);
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

				try {
						AndroidJavaClass pluginExceptionClass = new AndroidJavaClass ("com.crittercism.integrations.PluginException");
						AndroidJavaObject exception = pluginExceptionClass.CallStatic<AndroidJavaObject> ("createUnityException", name, stack);

						mCrittercismsPlugin.CallStatic ("_logCrashException", exception);
				} catch (System.Exception e) {
						System.Console.Write ("Unable to log a crash exception to Crittercism to to an unexpected error: " + e.ToString ());
				}
		}
}
