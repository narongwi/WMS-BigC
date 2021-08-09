import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

class WHCard extends StatelessWidget {
  final Color color;
  final String label;
  final String value;
  final double size;
  final Color textColor;

  const WHCard(
      {Key key,
      this.label,
      this.value,
      this.color,
      this.size = 12,
      this.textColor = Colors.grey})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.symmetric(vertical: 5, horizontal: 20),
      decoration: BoxDecoration(
          color: Colors.white, borderRadius: BorderRadius.circular(20)),
      child: Row(
        children: [
          // Icon(
          //   CupertinoIcons.app_fill,
          //   color: color,
          //   size: 10,
          // ),
          SizedBox(width: 10),
          Text(
            "$label $value",
            style: TextStyle(fontSize: size, color: textColor),
          ),
        ],
      ),
    );
  }
}
