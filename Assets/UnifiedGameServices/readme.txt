Google Play Game Services Plugin is a Unity developer friendly wrapper for Google Play Game Services. You can find details about the services here: https://developers.google.com/games/services/
Plugin allows you to access Achievements, Leaderboards and Cloud Save functionality via C# interface in both Android and iOS applications. Multiplayer functionality is supported only for Android.

C# interface is very slim and easy to use. You can find examples in Assets/UnifiedGameServices/Examples/ folder, one for every feature.




Setup Guide

1. Install the plugin to your unity project

2. Install "Google Play Services" and "Android Support Library" (it's in Extras folder) from Android SDK Manager 

3. Download "Games SDK for iOS" and "Google+ SDK for iOS" from https://developers.google.com/games/services/downloads/

4. Unzip frameworks and bundles from downloaded iOS SDKs to Assets/Plugins/iOS folder, so that you have the following path in your project:
    Assets/Plugins/iOS/PlayGameServices.framework
    Assets/Plugins/iOS/PlayGameServices.bundle
    Assets/Plugins/iOS/GoogleOpenSource.framework
    Assets/Plugins/iOS/GooglePlus.framework
    Assets/Plugins/iOS/GooglePlus.bundle

5. Set up a game in Google Developer Console. You can follow the sample application setup here: https://developers.google.com/games/services/android/quickstart#step_2_set_up_the_game_in_the_dev_console.

6. In plugin setup wizard ("Art of Bytes > Unified Game Services" from main menu) enter your Application ID and Client ID (required for iOS builds) and apply changes. You may also need to generate manifest file and copy missing jar files into the project. There are helpers for this actions in plugin setup wizard. Just follow the instructions in the setup window, if there are any errors or warnings.


At this point you'll be able to sign in to Google Play services from example scenes, launched on Android  or iOS device. In order to submit data from examples, you will also need to enter IDs of your achievements and leaderboards:

7. Open Achievements scene, and enter achievements' IDs in the fields of AchievementsUI object.

8. Open Leaderboards scene, and enter leaderboards' IDs in the fields of LeaderboardsUI object.




Checklist

- There should be no errors or warnings in Unified Game Services window before you try the game on device.
- Correct Application ID, that you get from Developer Console should be entered in Google Play Game Setup window
- Bundle Identifier in your Player Settings should be the same as in a linked Android and iOS apps in Developer Console
- APK file should be signed with the same keystore that was used to generate Certificate fingerprint in Developer Console
- Client ID should be the same as in your iOS linked app settings in Developer Console 
- If your game is not yet published, make sure you are signing in with account listed as tester for this game
- Make sure you have an internet connection on device when trying to sign in



Release notes

Version 1.1.0
=============
	Multiplayer for Android
	
	Achievements for iOS
	Leaderboards for iOS
	CloudSave for iOS 
	
	Unified plugin setup wizard


Version 1.0.1
=============
	Cloud Save support for Android with an example


Version 1.0.0
=============
	Achievements for Android
	Leaderboards for Android
	Plugin setup wizard for Android
