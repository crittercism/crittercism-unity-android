package com.crittercism.unity;
import java.lang.String;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.app.Application;
import android.content.res.Resources;
import android.util.Log;

import com.crittercism.app.Crittercism;

import com.unity3d.player.UnityPlayer;


public class CrittercismAndroid {
	
	public static boolean mIsInited			= false;
	public static Activity mAppActivity		= null;
	public static String mAppID				= null;
	
	public static JSONObject mConfig		= null;
	
	public static boolean mDebugLog			= true;
	public static boolean mMarkExceptionType= false;
	
	public static void CLog(String log)
	{
		if(mDebugLog)	{	Log.w("CrittercismAndroid", log);	}
	}
	
	public static void SetConfig(boolean delaySendAppLoad, boolean collectLogcat, String versionName)
	{
		mConfig = new JSONObject();
		try{
			mConfig.put("delaySendingAppLoad", delaySendAppLoad);
			mConfig.put("shouldCollectLogcat", collectLogcat);
			if( versionName != null ) { mConfig.put("customVersionName", versionName); }
			CLog("Config was successfully set");
		}
		catch( JSONException e){
			mConfig = null;
		}
	}
	
	public static void Init(Application app) {
		CLog("Attempting to initialize Crittercism");
		if(mIsInited || app == null)	{	return;	}
		
		try
		{	
			//	Check if we need to set from resouces
			try
			{
				Resources rs	= app.getResources();
				CLog("Package: " + app.getPackageName());
					
				int nID		= rs.getIdentifier("CrittercismAppID", "string", app.getPackageName());
				mAppID		= rs.getString(nID);
				
			}catch(Exception e)	{	mAppID	= null;	}
			
			try
			{
				Resources rs	= app.getResources();
				String sPackageName = app.getPackageName();
				int nDelayID = rs.getIdentifier("CrittercismDelaySendingAppLoad", "string",sPackageName );
				boolean bDelay = rs.getString(nDelayID).compareTo("true") == 0;
				
				int nCollectLog = rs.getIdentifier("CrittercismShouldCollectLogcat", "string", sPackageName);
				boolean bCollect = rs.getString(nCollectLog).compareTo("true") == 0;
				
				int nVersionID = rs.getIdentifier("CrittercismCustomVersionName", "string", sPackageName);
				String sVersionName = rs.getString(nVersionID);
				
				SetConfig(bDelay, bCollect, sVersionName);
			} catch( Exception e) { }
			
			CLog("AppID: " + mAppID);
			if(mAppID == null || mAppID == "" || mIsInited)	{	return;	}
			
			Crittercism.init(app.getApplicationContext(), mAppID, mConfig);
			mIsInited	= true;
						
		}catch(Exception e)
		{	mIsInited	= false;	}
		
	}
	
	public static void Init(Activity activity, String appID, final String callbackObjectName, final String callbackObjectMethod)
	{
		CLog("Beginning Initialization");
		if(mIsInited)				{	CLog("Already initialized"); return;	}
		if(mAppActivity == null)	{	mAppActivity	= activity;	}
		if(mAppID == null)			{	mAppID	= appID;	}
		
		if(mAppID == null || mAppActivity == null)	{	return;	}
		
		try
		{	
			//	Check if we need to set from resouces
			if(mAppActivity != null)
			{
				try
				{
					Resources rs	= mAppActivity.getResources();
					CLog("Package: " + mAppActivity.getPackageName());
					int nID			= rs.getIdentifier("CrittercismAppID", "string", mAppActivity.getPackageName());
					String ts		= rs.getString(nID);
					CLog("TestAppID: " + ts);
					
					if(ts != null && ts != "")
					{	mAppID	= ts;	}
					
				}catch(Exception e)	{	}
			}
			
			CLog("AppID: " + mAppID);
			if(mAppID == null || mAppID == "" || mAppActivity == null || mIsInited)	{	return;	}

			//	Run on main thread to avoid crashes
			mAppActivity.runOnUiThread(new Runnable()
				{
					public void run()
					{
						CLog("Starting Crittercism.init");
						Crittercism.init(mAppActivity.getApplicationContext(), mAppID, mConfig);
						mIsInited	= true;
						UnityPlayer.UnitySendMessage(callbackObjectName, callbackObjectMethod, "done");
					}
				}
			);

		}catch(Exception e)
		{	CLog(e.getLocalizedMessage());	}
		
	}
	
	private static Exception _CreateException(String name, String reason, String callStack)
	{
		Exception ex	= new Exception(reason);
		if(callStack != null)
		{
			String[] stackObjs	= callStack.split("\n");
			
			int nLength	= stackObjs.length;
			StackTraceElement[] elements	= new StackTraceElement[nLength];
			
			for(int nI = 0; nI < nLength; nI++)
			{
				elements[nI]	= new StackTraceElement("Unity3D", stackObjs[nI], "", -1);
			}
			ex.setStackTrace(elements);
		}
		return ex;
	}
	
	public static void LeaveBreadcrumb(String crumb)
	{
		if(!mIsInited || crumb == null)	{	return;	}
		Crittercism.leaveBreadcrumb(crumb);
	}
	
	public static void SetUsername(String name)
	{
		if(!mIsInited || name == null)	{	return;	}
		Crittercism.setUsername(name);
	}
	
	public static void SetMetadata(String[] ids, String[] values)
	{
		if( !mIsInited) { CLog("Critter not inited"); return; }
		if( ids == null || values == null || ids.length != values.length) { return; }

		JSONObject metadata = new JSONObject();
		try {
			
			for(int i = 0; i < ids.length; i++) {
				metadata.put(ids[i], values[i]);
			}

			Crittercism.setMetadata(metadata);
			
		} catch (JSONException e) {
			e.printStackTrace();
		}
	}
	
	public static boolean IsInited()
	{
		return mIsInited;
	}
	
	public static void EnableDebugLog(boolean b)
	{
		if( mIsInited == false) { return; }
		mDebugLog = b;
	}
	
//	public static void SetNotificationTitle(String name)
//	{
//		if(!mIsInited || name == null)	{	return;	}
//		Crittercism.setNotificationTitle(name);
//	}
	
	public static void SetOptOutStatus(boolean optOut)
	{
		if(!mIsInited)	{	return;	}
		Crittercism.setOptOutStatus(optOut);
	}
	
	public static boolean GetOptOutStatus()
	{
		if(!mIsInited)	{	return false;	}
		return Crittercism.getOptOutStatus();
	}
	
	public static String GetUserUUID()
	{
		if(!mIsInited) { return ""; }
		return Crittercism.getUserUUID();
	}

	public static void LogHandledException(String name, String reason, String callStack)
	{
		if(!mIsInited || reason == null || name == null)	{	return;	}
		
		Exception ex	= _CreateException("Handled Exception", reason, callStack);
		Crittercism.logHandledException(ex);
		
		CLog("Handled Exception Logged");
	}
	
	public static void LogUnhandledException(String name, String reason, String callStack)
	{
		if(!mIsInited || reason == null || name == null)	{	return;	}
		
		Exception ex	= _CreateException("Unhandled Exception", reason, callStack);
		Crittercism.logHandledException(ex);
		
		CLog("Unandled Exception Logged");
	}
}
