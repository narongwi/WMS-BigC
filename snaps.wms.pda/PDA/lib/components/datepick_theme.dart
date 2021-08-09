import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:wms/constants.dart';

class DatePickTheme extends StatelessWidget {
  final Widget picker;
  const DatePickTheme({Key key, @required this.picker}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Theme(
      data: ThemeData.dark().copyWith(
        colorScheme: ColorScheme.dark(
          primary: successColor,
          onPrimary: Colors.white,
          surface: primaryColor,
          onSurface: Colors.white,
        ),
        dialogBackgroundColor: secondaryColor,
      ),
      child: picker,
    );
  }
}
