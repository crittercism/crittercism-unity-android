using UnityEngine;

public class CrittercismTestGUI : MonoBehaviour
{

    void OnGUI ()
    {
        GUIStyle customStyle = new GUIStyle (GUI.skin.button);
        customStyle.fontSize = 30;

        int screenButtonHeight = Screen.height / 8;

        if (GUI.Button (new Rect (0, 0, Screen.width, screenButtonHeight), "Leave Breadcrumb", customStyle)) {
            CrittercismAndroid.LeaveBreadcrumb ("BreadCrumb");
        }

        if (GUI.Button (new Rect (0, screenButtonHeight, Screen.width, screenButtonHeight), "Set Username", customStyle)) {
            CrittercismAndroid.SetUsername("MommaCritter");
        }

        if (GUI.Button(new Rect(0, screenButtonHeight * 2, Screen.width, screenButtonHeight), "Set Metadata", customStyle)) {
            CrittercismAndroid.SetMetadata(new string[] { "Game Level", "Status" }, new string[] { "5", "Crashes a lot" });
        }

        if (GUI.Button (new Rect (0, screenButtonHeight * 3, Screen.width, screenButtonHeight), "C# Crash", customStyle)) {
            causeDivideByZeroException ();
        }

        if (GUI.Button (new Rect (0, screenButtonHeight * 4, Screen.width, screenButtonHeight), "C# Handled Exception", customStyle)) {
            try {
                causeDivideByZeroException ();
            } catch (System.Exception e) {
                CrittercismAndroid.LogHandledException (e);
            }
        }

        if (GUI.Button (new Rect (0, screenButtonHeight * 5, Screen.width, screenButtonHeight), "C# Null Pointer Exception", customStyle)) {
            causeNullPointerException ();
        }
    }

    // Demo stacktraces by calling a few interim methods before crashing
    void causeDivideByZeroException ()
    {
        interimMethod1 ("hi mom", 42);
    }

    void causeNullPointerException ()
    {
        object o = null;
        o.GetHashCode ();
    }

    void interimMethod1 (string demoParam1, int demoParam2)
    {
        interimMethod2 (7, 7, "abc");
    }

    void interimMethod2 (byte demoParam1, int demoParam2, string demoParam3)
    {
        finallyDoTheCrash (100);
    }

    void finallyDoTheCrash (int number)
    {
		int[] numbers = { 1, 2, 3, 4, 5 };

        numbers[10] /= 0;
    }

}
