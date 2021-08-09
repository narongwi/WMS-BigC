import 'package:flutter/material.dart';

class CardLabel extends StatelessWidget {
  CardLabel({Key key, @required this.title, @required this.subTitle})
      : super(key: key);
  final dynamic title;
  final String subTitle;

  @override
  Widget build(BuildContext context) {
    var titleStyle = TextStyle(fontSize: 12, color: Colors.blue);
    var subtitleStyle = TextStyle(fontSize: 9, color: Colors.grey);
    return Column(
      children: [
        Text("${title ?? ""}", style: titleStyle),
        Text("$subTitle", style: subtitleStyle),
      ],
    );
  }
}
