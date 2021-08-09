import 'package:flutter/material.dart';
import 'package:wms/constants.dart';

Container logoBar() {
  return Container(
    child: Row(
      children: <Widget>[
        Container(
          width: 40,
          height: 40,
          margin: EdgeInsets.all(10),
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            image: DecorationImage(
              image: AssetImage('assets/images/snaps-logo-small.jpg'),
              fit: BoxFit.fill,
            ),
          ),
        ),
        Expanded(
          child: Column(
            children: [
              Text(
                "Warehouse Management",
                style: TextStyle(
                  fontSize: 16,
                  color: primaryColor,
                  fontWeight: FontWeight.bold,
                ),
              ),
              Text(
                "Snaps Solutions Co.,Ltd.",
                style: TextStyle(
                  fontSize: 13,
                  color: secondaryColor,
                ),
              ),
            ],
          ),
        ),
        // Container(
        //   width: 40,
        //   height: 40,
        //   margin: EdgeInsets.all(10),
        //   decoration: BoxDecoration(
        //     // shape: BoxShape.circle,
        //     image: DecorationImage(
        //         image: AssetImage('assets/images/bigc-logo-small.jpg'),
        //         fit: BoxFit.fill),
        //   ),
        // ),
      ],
    ),
  );
}
