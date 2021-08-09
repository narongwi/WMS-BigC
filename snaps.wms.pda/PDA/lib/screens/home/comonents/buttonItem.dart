import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

import '../../../constants.dart';

class ButtonItem extends StatelessWidget {
  final bool isEnable;
  final IconData itemIcon;
  final IconData disableIcon = CupertinoIcons.lock_slash_fill;
  final String itemText;
  final Function onPress;

  const ButtonItem(
      {Key key,
      @required this.itemText,
      @required this.itemIcon,
      @required this.onPress,
      this.isEnable = true})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: isEnable ? onPress : null,
      style: ElevatedButton.styleFrom(
        primary: Colors.white,
        onPrimary: Color(0xFF9EAF00),
        padding: EdgeInsets.all(10),
        shape: new RoundedRectangleBorder(
          borderRadius: new BorderRadius.circular(20.0),
        ),
      ),
      child: Column(
        children: [
          Expanded(
              child: isEnable
                  ? Icon(itemIcon, size: 30, color: colorSpringGreen)
                  : Icon(disableIcon, size: 30, color: Color(0xffCCCCCC)),
              flex: 2),
          Expanded(
            child: Text(
              itemText,
              style: TextStyle(
                  color: defaultColor,
                  fontSize: 10,
                  fontWeight: FontWeight.bold),
            ),
          ),
        ],
      ),
    );
  }
}
