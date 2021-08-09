# wms

A new Flutter project.

## Getting Started

This project is a starting point for a Flutter application.

A few resources to get you started if this is your first Flutter project:

- [Lab: Write your first Flutter app](https://flutter.dev/docs/get-started/codelab)
- [Cookbook: Useful Flutter samples](https://flutter.dev/docs/cookbook)

For help getting started with Flutter, view our
[online documentation](https://flutter.dev/docs), which offers tutorials,
samples, guidance on mobile development, and a full API reference.


## check sdk
flutter doctor
## set skd path
flutter config --android-sdk D:\Android\sdk

## Build Apk
flutter build apk --target-platform android-arm,android-arm64 --split-per-abi
flutter build apk --release
flutter build apk

# version 
1. set version on pubspec.yaml => version: 1.0.2+1 (versionName = 1.0.2 , versionCode = 1)
2. flutter run to update android => local.properties

## how to run
1. Start Anroid Emulator
2. run command flutter run

## key store 27 Year
keytool -genkey -v -keystore snaps.wms.pda.keystore.jks -storetype JKS -keyalg RSA -keysize 2048 -validity 10000 -alias snaps.wms.pda
