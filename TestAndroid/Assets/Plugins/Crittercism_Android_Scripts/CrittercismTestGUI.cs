using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CrittercismTestGUI : MonoBehaviour
{
	void OnGUI()
	{
		if(Screen.height == 0 || Screen.width == 0)	{	return;	}
		
		int screenButtonHeight	= Screen.height / 8;
	
		if(GUI.Button(new Rect(0,0,Screen.width,screenButtonHeight), "Null Reference"))
		{
			CrittercismAndroid.LeaveBreadcrumb("Null Reference incoming?!");
			string crash = null;
			crash	= crash.ToLower();
		}
		
		if(GUI.Button(new Rect(0,screenButtonHeight,Screen.width,screenButtonHeight), "Divide By Zero"))
		{
			
			CrittercismAndroid.LeaveBreadcrumb("Lets divide by zero!");
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
			
			Debug.Log("User Test");
			CrittercismAndroid.SetUsername("Eddie Freeman");

			Debug.Log("Metadata Test 1");

			List<string> arrayOfKeys = new List<string>();
			List<string> arrayOfValues = new List<string>();
			
			arrayOfKeys.Add("Locale");
			arrayOfKeys.Add("playerID");
			arrayOfKeys.Add("playerLVL");
			arrayOfValues.Add("en");
			arrayOfValues.Add("23958");
			arrayOfValues.Add("34");

			CrittercismAndroid.SetMetadata(arrayOfKeys.ToArray(), arrayOfValues.ToArray());
			
			Debug.Log("Breadcrumb Test");
			CrittercismAndroid.LeaveBreadcrumb("BreadCrumb");
			
			Debug.Log("Metadata Test 2");
			CrittercismAndroid.SetMetadata(new string[] {"Age", "Email", "Extra"}, new string[] {"26", "email@test.com", "Data"} );
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
