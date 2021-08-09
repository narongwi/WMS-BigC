import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

import '../constants.dart';

// ignore: must_be_immutable
class DialogConfirm extends StatelessWidget {
  //When creating please recheck 'context' if there is an error!

  // Color _color = Color.fromARGB(220, 117, 218, 255);

  String _title;
  String _content;
  String _yes;
  String _no;
  Function _yesOnPressed;
  Function _noOnPressed;

  DialogConfirm(
      {String title,
      String content,
      Function onYes,
      Function onNo,
      String yes = "Yes",
      String no = "No"}) {
    this._title = title;
    this._content = content;
    this._yesOnPressed = onYes;
    this._noOnPressed = onNo;
    this._yes = yes;
    this._no = no;
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Center(
        child: new Text(
          this._title,
          style: TextStyle(
              color: colorBlue, fontSize: 16, fontWeight: FontWeight.normal),
        ),
      ),
      content: Container(
        padding: EdgeInsets.all(5),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: new BorderRadius.circular(5),
        ),
        child: Row(
          children: [
            Icon(
              CupertinoIcons.question_circle,
              color: colorStem,
              size: 30,
            ),
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
      shape: RoundedRectangleBorder(borderRadius: new BorderRadius.circular(8)),
      actions: <Widget>[
        Container(
            width: MediaQuery.of(context).size.width,
            child: Row(
              children: [
                Spacer(),
                ElevatedButton.icon(
                  onPressed: () {
                    Navigator.of(context).pop();
                    this._noOnPressed();
                  },
                  label: Text(this._no),
                  icon: Icon(
                    CupertinoIcons.multiply,
                    color: Colors.grey,
                    size: 20.0,
                  ),
                  style: ElevatedButton.styleFrom(
                    elevation: 0,
                    primary: Colors.white, // background
                    onPrimary: Colors.grey, // foreground
                    shape: RoundedRectangleBorder(
                      borderRadius: new BorderRadius.circular(8.0),
                    ),
                  ),
                ),
                SizedBox(
                  width: 40,
                ),
                ElevatedButton.icon(
                  onPressed: () {
                    Navigator.of(context).pop();
                    this._yesOnPressed();
                  },
                  label: Text(
                    this._yes,
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                  icon: Icon(
                    CupertinoIcons.checkmark_alt,
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
                Spacer(),
              ],
            ))
      ],
    );
  }
}
