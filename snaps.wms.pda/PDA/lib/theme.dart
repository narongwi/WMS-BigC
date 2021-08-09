import 'package:flutter/material.dart';

import 'constants.dart';

ThemeData theme() {
  return ThemeData(
    scaffoldBackgroundColor: iconBgColor,
    // fontFamily: "Muli",
    appBarTheme: appBarTheme(),
    textTheme: textTheme(),
    inputDecorationTheme: inputDecorationTheme(),
    visualDensity: VisualDensity.adaptivePlatformDensity,
    elevatedButtonTheme: elevatedButtonThemeData(),
  );
}

ElevatedButtonThemeData elevatedButtonThemeData() {
  return ElevatedButtonThemeData(
    style: ElevatedButton.styleFrom(
      primary: primaryColor, // background
      onPrimary: Colors.white, // foreground
      textStyle: TextStyle(fontSize: 13),
      shape: RoundedRectangleBorder(
        borderRadius: new BorderRadius.circular(20),
        // side: BorderSide(color: Color(0xFFCCCCCC), width: 1),
      ),
    ),
  );
}

InputDecorationTheme inputDecorationTheme() {
  OutlineInputBorder outlineInputBorder = OutlineInputBorder(
    borderRadius: BorderRadius.circular(20),
    borderSide: BorderSide(color: secondaryColor),
    gapPadding: 10,
  );
  return InputDecorationTheme(
      prefixStyle: TextStyle(fontSize: 13, color: dangerColor),
      suffixStyle: TextStyle(fontSize: 13, color: warningColor),
      floatingLabelBehavior: FloatingLabelBehavior.always,
      contentPadding: EdgeInsets.symmetric(horizontal: 10, vertical: 8),
      enabledBorder: outlineInputBorder,
      focusedBorder: outlineInputBorder,
      border: outlineInputBorder,
      isDense: true);
}

TextTheme textTheme() {
  return TextTheme(
    bodyText1: TextStyle(color: secondaryColor),
    bodyText2: TextStyle(color: secondaryColor),
  );
}

AppBarTheme appBarTheme() {
  return AppBarTheme(
    color: Colors.transparent,
    centerTitle: false,
    elevation: 0,
    brightness: Brightness.light,
    iconTheme: IconThemeData(color: colorBlue),
    actionsIconTheme: IconThemeData(color: colorBlue),
    textTheme: TextTheme(
      headline6: TextStyle(
        color: colorBlue,
        fontSize: 16,
        // fontWeight: FontWeight.bold,
      ),
    ),
  );
}
