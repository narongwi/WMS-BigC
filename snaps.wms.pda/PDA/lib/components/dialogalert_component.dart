import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import '../constants.dart';

// ignore: must_be_immutable
class DialogAlert extends StatelessWidget {
  // Color _color = Color.fromARGB(220, 117, 218, 255);
  String _title;
  String _content;
  String _ok;
  Function _okOnPressed;
  String _type;

  DialogAlert({
    String title,
    String content,
    Function onOk,
    String ok = "Ok",
    String type,
  }) {
    this._title = title;
    this._content = content;
    this._okOnPressed = onOk;
    this._ok = ok;
    this._type = type;
  }

  @override
  Widget build(BuildContext context) {
    Color typeColor() {
      if (_type == null) {
        return primaryColor;
      } else if (_type.toLowerCase() == "warning") {
        return warningColor;
      } else if (_type.toLowerCase() == "success") {
        return successColor;
      } else if (_type.toLowerCase() == "info") {
        return infoColor;
      } else if (_type.toLowerCase() == "error") {
        return dangerColor;
      } else {
        return primaryColor;
      }
    }

    Icon typeIcon() {
      if (_type == null) {
        return Icon(Icons.question_answer, color: primaryColor, size: 30);
      } else if (_type.toLowerCase() == "warning") {
        return Icon(Icons.warning_rounded, color: warningColor, size: 30);
      } else if (_type.toLowerCase() == "success") {
        return Icon(Icons.check_circle, color: successColor, size: 30);
      } else if (_type.toLowerCase() == "info") {
        return Icon(Icons.info, color: infoColor, size: 30);
      } else if (_type.toLowerCase() == "error") {
        return Icon(Icons.error, color: dangerColor, size: 30);
      } else {
        return Icon(Icons.question_answer, color: primaryColor, size: 30);
      }
    }

    return AlertDialog(
      title: new Text(
        this._title,
        textAlign: TextAlign.center,
        style: TextStyle(color: colorBlue, fontSize: 16, fontWeight: FontWeight.normal),
      ),
      content: Container(
        padding: EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: typeColor().withAlpha(20),
          borderRadius: new BorderRadius.circular(5),
        ),
        child: Row(
          children: [
            typeIcon(),
            SizedBox(width: 10),
            Expanded(
              child: Text(
                this._content,
                style: TextStyle(color: primaryColor, fontSize: 13),
              ),
            ),
          ],
        ),
      ),
      backgroundColor: Colors.white,
      shape: RoundedRectangleBorder(
        borderRadius: new BorderRadius.circular(8),
      ),
      actions: <Widget>[
        Container(
          padding: EdgeInsets.only(left: 80, right: 80),
          width: MediaQuery.of(context).size.width,
          child: ElevatedButton.icon(
            onPressed: () {
              Navigator.of(context).pop();
              this._okOnPressed();
            },
            label: Text(
              this._ok,
              style: TextStyle(fontWeight: FontWeight.bold),
            ),
            icon: Icon(
              CupertinoIcons.checkmark_alt_circle,
              color: colorBlue,
              size: 20.0,
            ),
            style: ElevatedButton.styleFrom(
              elevation: 0,
              primary: Colors.white, // background
              onPrimary: colorBlue, // foreground
              shape: RoundedRectangleBorder(
                borderRadius: new BorderRadius.circular(8.0),
              ),
            ),
          ),
        ),
        // new TextButton(
        //   child: new Text(this._ok),
        //   onPressed: () {
        //     Navigator.of(context).pop();
        //     this._okOnPressed();
        //   },
        // )
      ],
    );
  }
}
