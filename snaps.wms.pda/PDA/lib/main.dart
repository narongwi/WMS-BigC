import 'package:flutter/material.dart';
import 'package:wms/routes.dart';
import 'package:wms/screens/splashscreen/splash_screen.dart';
import 'package:wms/theme.dart';

void main() {
  runApp(WMSAPP());
}

class WMSAPP extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Warehouse Management System',
      debugShowCheckedModeBanner: false,
      theme: theme(),
      routes: routes,
      home: SplashScreenApp(),
    );
  }
}
