using System;
using UnityEngine;

public class CrittercismTestGUI : MonoBehaviour
{
	void OnGUI ()
	{
		CrittercismAndroid.SetLogUnhandledExceptionAsCrash (true);
		GUIStyle customStyle = new GUIStyle (GUI.skin.button);
		customStyle.fontSize = 30;
		const int numberOfButtons = 11;
		int screenButtonHeight = Screen.height / numberOfButtons;
		if (GUI.Button (new Rect (0, 0, Screen.width, screenButtonHeight), "Leave Breadcrumb", customStyle)) {
			CrittercismAndroid.LeaveBreadcrumb ("BreadCrumb");
		}
		if (GUI.Button (new Rect (0, screenButtonHeight, Screen.width, screenButtonHeight), "Set Username", customStyle)) {
			CrittercismAndroid.SetUsername ("MommaCritter");
		}
		if (GUI.Button (new Rect (0, screenButtonHeight * 2, Screen.width, screenButtonHeight), "Set Metadata", customStyle)) {
			CrittercismAndroid.SetMetadata (new string[] {
				"Game Level",
				"Status"
			}, new string[] {
				"5",
				"Crashes a lot"
			});
		}
		if (GUI.Button (new Rect (0, screenButtonHeight * 3, Screen.width, screenButtonHeight), "C# Crash", customStyle)) {
			crashInnerException ();
		}
		if (GUI.Button (new Rect (0, screenButtonHeight * 4, Screen.width, screenButtonHeight), "C# Handled Exception", customStyle)) {
			try {
				crashInnerException ();
			} catch (System.Exception e) {
				CrittercismAndroid.LogHandledException (e);
			}
		}
		if (GUI.Button (new Rect (0, screenButtonHeight * 5, Screen.width, screenButtonHeight), "C# Null Pointer Exception", customStyle)) {
			try {
				causeNullPointerException ();
			} catch (Exception e) {
				CrittercismAndroid.LogHandledException (e);
			}
		}
		if (GUI.Button (new Rect (0, screenButtonHeight * 6, Screen.width, screenButtonHeight), "Begin Transaction", customStyle)) {
			CrittercismAndroid.BeginTransaction ("UnityAndroid");
		}
		if (GUI.Button (new Rect (0, screenButtonHeight * 7, Screen.width, screenButtonHeight), "End Transaction", customStyle)) {
			CrittercismAndroid.EndTransaction ("UnityAndroid");
		}
		if (GUI.Button (new Rect (0, screenButtonHeight * 8, Screen.width, screenButtonHeight), "Fail Transaction", customStyle)) {
			CrittercismAndroid.FailTransaction ("UnityAndroid");
		}
		if (GUI.Button (new Rect (0, screenButtonHeight * 9, Screen.width, screenButtonHeight), "Set Transaction Value", customStyle)) {
			CrittercismAndroid.SetTransactionValue ("UnityAndroid", 500);
		}
		if (GUI.Button (new Rect (0, screenButtonHeight * 10, Screen.width, screenButtonHeight), "Get Transaction Value", customStyle)) {
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
