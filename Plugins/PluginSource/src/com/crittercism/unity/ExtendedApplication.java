package com.crittercism.unity;

import android.app.Application;

public class ExtendedApplication extends Application {
	
	@Override
	public void onCreate() {
		super.onCreate();
		
		CrittercismAndroid.Init(this);
	}
}
