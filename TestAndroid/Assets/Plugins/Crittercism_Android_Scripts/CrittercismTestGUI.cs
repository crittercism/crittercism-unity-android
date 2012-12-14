using UnityEngine;

public class CrittercismTestGUI : MonoBehaviour
{
	void OnGUI()
	{
		if(Screen.height == 0 || Screen.width == 0)	{	return;	}
		
		int screenButtonHeight	= Screen.height / 8;
	
		if(GUI.Button(new Rect(0,0,Screen.width,screenButtonHeight), "Null Reference"))
		{
			string crash = null;
			crash	= crash.ToLower();
		}
		
		if(GUI.Button(new Rect(0,screenButtonHeight,Screen.width,screenButtonHeight), "Divide By Zero"))
		{
			int i = 0;
			i = 2 / i;
		}
		
		if(GUI.Button(new Rect(0,screenButtonHeight * 2,Screen.width,screenButtonHeight), "Index Out Of Range"))
		{
			string[] arr	= new string[1];
			arr[2]	= "Crash";
		}
		
		if(GUI.Button(new Rect(0,screenButtonHeight * 3,Screen.width,screenButtonHeight), "Custom Exception"))
		{	throw new System.Exception("Custom Exception");	}
		
		if(GUI.Button(new Rect(0,screenButtonHeight * 4,Screen.width,screenButtonHeight), "Coroutine Custom Exception"))
		{	StartCoroutine(MonoCorutineCrash());	}
		
		if(GUI.Button(new Rect(0,screenButtonHeight * 5,Screen.width,screenButtonHeight), "Coroutine Null Exception"))
		{	StartCoroutine(MonoCorutineNullCrash());	}
		
		if(GUI.Button(new Rect(0,screenButtonHeight * 7,Screen.width,screenButtonHeight), "Test Messages"))
		{
			Debug.Log("Breadcrumb Test");
			CrittercismAndroid.LeaveBreadcrumb("BreadCrumb");
			
			Debug.Log("Age Test");
			CrittercismAndroid.SetValue("26", "Age");
			
			Debug.Log("Email Test");
			CrittercismAndroid.SetValue("email@test.com", "Email");
			
			Debug.Log("User Test");
			CrittercismAndroid.SetUsername("Username");
		}
	}
	
	System.Collections.IEnumerator MonoCorutineNullCrash()
	{
		string crash = null;
		crash	= crash.ToLower();
		yield break;
	}
	
	System.Collections.IEnumerator MonoCorutineCrash()
	{	
		throw new System.Exception("Custom Coroutine Exception");
	}
}
