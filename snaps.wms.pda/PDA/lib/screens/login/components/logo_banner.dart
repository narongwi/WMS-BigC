import 'package:flutter/widgets.dart';
import 'package:wms/constants.dart';

class WMSBanner extends StatelessWidget {
  const WMSBanner({Key key}) : super(key: key);
  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text("BGC WMS", style: TextStyle(fontSize: 32, color: primaryColor)),
        Text("Warehouse Management System",
            style: TextStyle(fontSize: 12, color: secondaryColor)),
        SizedBox(height: 10.0),
        ClipRRect(
          borderRadius: BorderRadius.all(Radius.circular(20.0)),
          child: Image.asset("assets/images/wms-banner_circle.png"),
        ),
      ],
    );
  }
}
