Overview
==========

[Crittercism](http://www.crittercism.com) helps developers track and manage performance of mobile applications on several platforms. In its default configuration, the library offers two key benefits:

* **App Load Tracking** When the user begins using an instrumented application, the library records an app load event. Crittercism aggregates app loads into rolling daily and monthly active user counts using a Crittercism generated unique identifier, and delivers insight into the relative popularity of the application's released versions.

* **Unhandled Exception Tracking** When an unhandled exception occurs, the library records device state and a stack trace for immediate/delayed (depending on platform) transmission to Crittercism. Crittercism informs the developer of the error, and provides the information necessary to reproduce the issue in local development / testing.

In addition to crash reporting, the library provides calls for setting timestamped checkpoints (or Breadcrumbs), recording arbitrary user state (or Metadata), and saving stack traces with developer-provided messages on arbitrary code paths (or Handled Exceptions).

Learn more about what Crittercism provides with the [solution overview][14].

About the Plugin
==================

The plugin will capture more on Android and the most detailed exceptions when **"Script Call Optimizations"** are set to **"Slow, but Safe."**. Exception handlers are otherwise stripped from the Unity3D engine and have to be handled outside the Mono Environment. In these cases, symbols captured will be relevant to the Unity3d Engine code, _not_ the C# or Javascript code. These settings can be found in the **Android Player settings**, reachable with the menu from **“Edit->Project Settings->Player”**, then under the **“Other Settings”** category.

Supporting the Plugin
=======================

This is our first open source project here at Crittercism as with regular updates to Unity and different development scenarios a Unity Plugin can be a complicated project to maintain. That's why we'd love your help!

Getting Involved, Some Guidelines
-----------------------------------

#### Filing a Ticket

Github provides us a medium to publicly file tickets and report issues; you can do so [here][12]. As for what each ticket contains, here are some guidelines of what will make solving an issue easier for everyone:

* Items to include in a ticket:
    - Source of the issue (who reported it)
    - Version of Unity effected
    - Version of iOS/Android/etc effected
    - Version of XCode/Eclipse/Android API
    - Date discovered (versus date filed)
    - Any related Crittercism Crash Report links as examples
* Give a scenario, if one is clear, in which the issue is reproduced

#### Writing Commits

Writing a commit should be simple, but here are some guidelines that will help us all be clear:

* Be clear about what each commit contains (such as feature or process updated, specific bug fixed, etc)
* Be concise and commit often, preferably after a smaller task is complete and tested

#### Submitting a Pull Request

Pull Requests are generally related to specific bugs, features, updates and are best created via forking the existing repository and making the changes in your personal repository; learn more about [Pull Requests][13] from Github. When submitting a pull request there are a few items to keep in mind that will make everyones lives easier:

* Keep the requests small, having a large number of commits can create a large time commitment for those reviewing the request
* Sign off the code to adhere to the DCO (found below) using the following example:
    ```
    Signed-off-by: John Doe <john.doe@hisdomain.com>
    ```

Make sure the above sign-off is verbatim and uses your real name and most accurate email.

Installing the Library
========================

Installing the Crittercism Android Unity Plugin starts by downloading the latest Android Unity.

The download includes the following items:
* **Plugins/Android** - Contains the static library generated from the Plugin Source (which includes the Crittercism Jar File)
* **Plugins/Crittercism_Android_Scripts** - Contains an Init Script, the Plugin Script, and the Test GUI Script
* **TestAndroid** - An example app that provides a reference of usage for features
* **Docs** - Contains an offline reference to the docs

Adding the Library to your Project
------------------------------------

Drag and drop the following items into your project:

    Android_Unity/Plugins/Android                       --->    Assets/Plugins/Android
    Android_Unity/Plugins/Crittercism_Android_Scripts   --->    Assets/Plugins/Crittercism_Android_Scripts

If you already have other Android Plugins, the contents of these folders should be copied over.

### Modify Your Android Manifest ###

A custom AndroidManifiest.xml must be added to the {Unity Project}/Plugins/Android folder. Inside the file, verify you have the _INTERNET_ permission in between the `<manifest> ... </manifest>` tags so Crittercism can send data to our servers:

    <uses-permission android:name="android.permission.INTERNET"/>

For more granular data (optional), add the following permissions:

    <uses-permission android:name="android.permission.READ_LOGS"/>
    <uses-permission android:name="android.permission.GET_TASKS"/>

**READ_LOGS** - provides Crittercism access to device _Logcat_ information for Android OS 2.2 to 4.0
**GET_TASKS** - provides Crittercism access to information about the last two activities running; shown in Crash diagnostics.

Initializing Crittercism
--------------------------

NOTE: It is important that Crittercism Android runs from the top of your Application class extension or main activity - the same activity as your android.intent.action.MAIN intent filter to capture the most exceptions.

Within Unity, you have two options to initialize Crittercism:

1. Use the **CrittercismInit.cs** found in Plugins/Crittercism_Android_Scripts and replace the values necessary for the following variables:

        private const string CrittercismAppID    = ""; /*Your App ID Here*/
        private const bool bDelaySendingAppLoad = false; // not recommended, default false
        private const bool bShouldCollectLogcat = false;
        private const string CustomVersionName = ""; /*Your Custom Version Name Here*/

2. This method overrides the above method. Under ** Plugins/Android/res/values ** you'll find a strings.xml that contains fields for the above described variables.

        <string name="CrittercismAppID">Your_App_ID</string>
        <string name="CrittercismDelaySendingAppLoad">false</string>
        <string name="CrittercismShouldCollectLogcat">true</string>
        <string name="CrittercismCustomVersionName">Your_Version</string>

Now you've completed the basic code integration!

Test GUI within Your App
--------------------------

In Unity, drag **Plugins/Crittercism_Android_Scripts/CrittercismTestGUI.cs** file to the main scene camera. This provides a test interface for Unity3d that will throw exceptions. These exceptions will currently show up under the Handled Exceptions tab.

Getting Started with Advanced Features!
=========================================

Capturing uncaught exceptions is a powerful tool for any developer, but sometimes you want to do even more. That's why Crittercism provides several advanced features. Here are a few getting started tips:

Handled Exception
-------------------

Crittercism allows you to capture and track disruptive crashes that interrupt the flow within the app, even if the error doesn’t result in a crash by passing handled exception objects to our servers. They’ll be grouped and symbolicated much like your normal uncaught exceptions.

### How To Use Handled Exceptions ###

1. Identify potential hotspots where an error might occur.
2. Trap the potential exceptions.
3. Analyze disruptive exceptions by viewing the stacktrace, diagnostics, metadata and breadcrumbs related to that issue.
4. Adjust the user flow in your code around exceptions to ensure the best user experience.
5. Prioritize and resolve bugs to stop your app from crashing.

### Some High-Level Use Cases ###

* Accessing data (such as over network or on local storage)
* Establishing connections (for example handling a malformed request)
* Starting background services
* Third-Party initializations/integrations

### Usage ###

Here's an example of how to implement these for Java:

	try {
        CrittercismAndroid.LeaveBreadcrumb("Trying to ______");
        // Make network connection
        // or add to JSON object
        // or access local storage
        // or start background service
    } catch (System.Exception error) {
        CrittercismAndroid.LeaveBreadcrumb("Failed to ______");
        CrittercismAndroid.LogHandledException(error)
    }

Breadcrumbs
-------------

Sometimes a stack trace just isn’t enough. By placing breadcrumbs in your code, you can get a playback of events in the run-up to a crash or exception. For each session, our libraries automatically store a "session_start" breadcrumb to mark the beginning of a user session, and the most recent 99 breadcrumbs that were left before a crash. To leave a breadcrumb, simply insert an API call at points of interest in your code after instantiating Crittercism. Each breadcrumb can contain up to 140 characters.

### How To Use Breadcrumbs ###

1. Identify potential session events/state/variables to capture for debugging.
2. Place breadcrumbs surrounding these events/state/variables.
3. Analyze session events/state/variables to hasten tracking down the culprit of an issue.
4. Combine this information with stack traces, diagnostics, and metadata to prioritize and resolve bugs around these events.

### Some High-Level Use Cases ###

* Measuring time performance for UX
* Identifying hotspots within your application and/or functionality
* Tracking variables or state throughout the user flow
* Flagging events within callbacks (such as low memory warnings)

### Usage ###

Here's an example of how to use breadcrumbs for Java (accepts 140 characters):

	CrittercismAndroid.LeaveBreadcrumb("Class \\ Method \\ Activity "); // An example use, any string works

User Metadata
---------------

You can attach metadata to each user. This data will help you differentiate users on both the portal and the client. For example, if you specify a username, it will appear in the client when the user leaves feedback. On the portal, the specified username will appear both in the forum, as well as in the Crash Report tab (under Affected Users when you select a specific user), allowing you to correlate data and respond to support tickets with greater knowledge.

You can also attach an arbitrary amount of metadata to each user through a method accepting key and value parameters. The data will be stored in a dictionary and displayed in the Crittercism Portal when viewing a user profile.

**Crittercism takes user privacy very seriously** and as such if you are spotted sending Device Identifiers or other personally identifying information that isn't helpful for debugging a crash we will ask you to please remove the gathering of this information through our service.

### How To Use MetaData ###

1. Identify potential user events/state/variables to capture for debugging.
2. Assign arbitrary metadata during these events.
3. Analyze user events/state/variables to hasten tracking down the culprit of an issue.
4. Combine this information with stack traces and diagnostics to prioritize and resolve bugs around these events.

### Some High-Level Use Cases ###

* Track shopping cart or transaction information of the user at time of crash
* State within the user flow (such as level of game, location, or view of app)

### Feature Usage ###

Here's an example of how to use user metadata for Java:

    // Don't forget to include the collections for Lists
    using System.Collections;
    using System.Collections.Generic;

	// Early on in the game session or when the user changes their username (if possible)
	CrittercismAndroid.SetUsername("TheCritter");

    // Create a list then convert them to arrays to pass them through.
	List<string> arrayOfKeys = new List<string>();
            List<string> arrayOfValues = new List<string>();

            arrayOfKeys.Add("Locale");
            arrayOfKeys.Add("playerID");
            arrayOfKeys.Add("playerLVL");
            arrayOfValues.Add("en");
            arrayOfValues.Add("23958");
            arrayOfValues.Add("34");

    CrittercismAndroid.SetMetadata(arrayOfKeys.ToArray(), arrayOfValues.ToArray());

    // Another way, use straight arrays!
    CrittercismAndroid.SetMetadata(new string[] {"Age", "Email", "Extra"}, new string[] {"26", "email@test.com", "Data"} );

Developer Certificate of Origin
=================================

By making a contribution to this project, I certify that:

(a) The contribution was created in whole or in part by me and I
    have the right to submit it under the open source license
    indicated in the file; or

(b) The contribution is based upon previous work that, to the best
    of my knowledge, is covered under an appropriate open source
    license and I have the right under that license to submit that
    work with modifications, whether created in whole or in part
    by me, under the same open source license (unless I am
    permitted to submit under a different license), as indicated
    in the file; or

(c) The contribution was provided directly to me by some other
    person who certified (a), (b) or (c) and I have not modified
    it.

(d) I understand and agree that this project and the contribution
    are public and that a record of the contribution (including all
    personal information I submit with it, including my sign-off) is
    maintained indefinitely and may be redistributed consistent with
    this project or the open source license(s) involved.

Change Log
------------

2.9.2014. Refreshed the documentation!

[2]: htttp://support.crittercism.com                                "Crittercism Help"
[12]: https://github.com/crittercism/crittercism-unity-android/issues   "Unity Android Issues"
[13]: https://help.github.com/articles/using-pull-requests          "Pull Requests - Github"
[14]: https://www.crittercism.com/solution-overview/                "Solution Overview"
