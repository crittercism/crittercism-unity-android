#!/bin/bash
[ -d classes ] && rm -rf classes
mkdir classes
buildType="$1"
[ "$buildType" == "development" ] && export UNITYJAR=/Applications/Unity/Unity.app/Contents/PlaybackEngines/AndroidDevelopmentPlayer/bin/classes.jar
[ "$buildType" == "production" ] && export UNITYJAR=/Applications/Unity/Unity.app/Contents/PlaybackEngines/AndroidPlayer/bin/classes.jar
[ "$UNITYJAR" == "" ] && echo "Syntax: build.sh [development|production]" >&2 && echo >&2 && exit 1
javac `find . -name '*.java'` -bootclasspath $ANDROID_SDK_ROOT/platforms/android-8/android.jar  -classpath ./libs/crittercism_v3_0_8.jar:${UNITYJAR} -d classes
cp -r res classes
cp -r libs classes
cd classes
jar -cvf ../CrittercismAndroid.jar .
cd -
