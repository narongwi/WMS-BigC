import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:splashscreen/splashscreen.dart';
import 'package:wms/screens/login/login_screen.dart';

class SplashScreenApp extends StatefulWidget {
  @override
  _SplashScreen createState() => new _SplashScreen();
}

class _SplashScreen extends State<SplashScreenApp> {
  @override
  Widget build(BuildContext context) {
    return new SplashScreen(
      seconds: 5,
      navigateAfterSeconds: new LoginScreen(),
      image: Image.asset("assets/images/snaps-logo.jpg"),
      backgroundColor: Colors.white,
      photoSize: 60,
    );
  }
}
