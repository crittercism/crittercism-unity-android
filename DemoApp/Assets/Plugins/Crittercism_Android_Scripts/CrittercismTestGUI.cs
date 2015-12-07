using System;
using System.Net;
using UnityEngine;

public class CrittercismTestGUI : MonoBehaviour
{
	private CrittercismTestGUI() {
		Debug.Log ("DidCrashOnLastLoad: " + CrittercismAndroid.DidCrashOnLastLoad ());
		CrittercismAndroid.SetLogUnhandledExceptionAsCrash (true);
	}

	private static string[] uriStrings=new string[] {
		"http://www.crittergerbil.com",
		"http://www.critterhamster.com",
		"http://www.crittermouse.com",
		"http://www.critterrabbit.com",
		"http://www.critterrobin.com",
		"http://www.crittersnake.com",
		"http://www.crittersquirrel.com",
		"http://www.mrscritter.com"
	};

	public void OnGUI ()
	{
		GUIStyle customStyle = new GUIStyle (GUI.skin.button);
		customStyle.fontSize = 30;
		const int numberOfButtons = 13;
		int screenButtonHeight = Screen.height / numberOfButtons;
		if (GUI.Button (new Rect (0, 0, Screen.width, screenButtonHeight), "Set Username", customStyle)) {
			CrittercismAndroid.SetUsername ("MommaCritter");
		}
		if (GUI.Button (new Rect (0, screenButtonHeight, Screen.width, screenButtonHeight), "Set Metadata", customStyle)) {
			CrittercismAndroid.SetMetadata (new string[] {
				"Game Level",
				"Status"
			}, new string[] {
				"5",
				"Crashes a lot"
			});
		}
		if (GUI.Button (new Rect (0, 2 * screenButtonHeight, Screen.width, screenButtonHeight), "Leave Breadcrumb", customStyle)) {
			CrittercismAndroid.LeaveBreadcrumb ("BreadCrumb");
		}
		if (GUI.Button (new Rect (0, 3 * screenButtonHeight, Screen.width, screenButtonHeight), "Log Network Request", customStyle)) {
			System.Random random=new System.Random();
			string[] methods=new string[] { "GET","POST","HEAD","PUT" };
			string method=methods[random.Next(0,methods.Length)];
			string uriString=uriStrings[random.Next(0,uriStrings.Length)];
			if (random.Next(0,2)==1) {
				uriString=uriString+"?doYouLoveCrittercism=YES";
			}
			// latency in milliseconds
			long latency=(long)Math.Floor(4000.0*random.NextDouble());
			long bytesRead=random.Next(0,10000);
			long bytesSent=random.Next(0,10000);
			int responseCode=200;
			if (random.Next(0,5)==0) {
				// Some common response other than 200 == OK .
				int[] responseCodes=new int[] { 301,308,400,401,402,403,404,405,408,500,502,503 };
				responseCode=responseCodes[random.Next(0,responseCodes.Length)];
			}
			Console.WriteLine("LogNetworkRequest: \""+uriString+"\"");
			CrittercismAndroid.LogNetworkRequest(
				method,
				uriString,
				latency,
				bytesRead,
				bytesSent,
				(HttpStatusCode)responseCode,
				WebExceptionStatus.Success);
		}
		if (GUI.Button (new Rect (0, 4 * screenButtonHeight, Screen.width, screenButtonHeight), "C# Crash", customStyle)) {
			crashInnerException ();
		}
		if (GUI.Button (new Rect (0, 5 * screenButtonHeight, Screen.width, screenButtonHeight), "C# Handled Exception", customStyle)) {
			try {
				crashInnerException ();
			} catch (System.Exception e) {
				CrittercismAndroid.LogHandledException (e);
			}
		}
		if (GUI.Button (new Rect (0, 6 * screenButtonHeight, Screen.width, screenButtonHeight), "C# Null Pointer Exception", customStyle)) {
			try {
				causeNullPointerException ();
			} catch (Exception e) {
				CrittercismAndroid.LogHandledException (e);
			}
		}
		if (GUI.Button (new Rect (0, 7 * screenButtonHeight, Screen.width, screenButtonHeight), "Begin Transaction", customStyle)) {
			CrittercismAndroid.BeginTransaction ("UnityAndroid");
		}
		if (GUI.Button (new Rect (0, 8 * screenButtonHeight, Screen.width, screenButtonHeight), "End Transaction", customStyle)) {
			CrittercismAndroid.EndTransaction ("UnityAndroid");
		}
		if (GUI.Button (new Rect (0, 9 * screenButtonHeight, Screen.width, screenButtonHeight), "Fail Transaction", customStyle)) {
			CrittercismAndroid.FailTransaction ("UnityAndroid");
		}
		if (GUI.Button (new Rect (0, 10 * screenButtonHeight, Screen.width, screenButtonHeight), "Cancel Transaction", customStyle)) {
			CrittercismAndroid.CancelTransaction ("UnityAndroid");
		}
		if (GUI.Button (new Rect (0, 11 * screenButtonHeight, Screen.width, screenButtonHeight), "Set Transaction Value", customStyle)) {
			CrittercismAndroid.SetTransactionValue ("UnityAndroid", 500);
		}
		if (GUI.Button (new Rect (0, 12 * screenButtonHeight, Screen.width, screenButtonHeight), "Get Transaction Value", customStyle)) {
			int value = CrittercismAndroid.GetTransactionValue ("UnityAndroid");
			Debug.Log ("TransactionValue is: " + value);
		}
	}
	
	public void DeepError (int n)
	{
		if (n == 0) {
			throw new Exception ("Deep Inner Exception");
		} else {
			DeepError (n - 1);
		}
	}
	
	public void crashInnerException ()
	{
		try {
			DeepError (4);
		} catch (Exception ie) {
			throw new Exception ("Outer Exception", ie);
		}
	}

	void causeNullPointerException ()
	{
		object o = null;
		o.GetHashCode ();
	}
}
