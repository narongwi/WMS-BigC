import 'package:flutter/material.dart';
import 'package:wms/size_config.dart';

// iis ip address
// const String urlATH = "http://192.168.1.13:5001";
// const String urlAPI = "http://192.168.1.13:5002";
// const String urlATH = "http://bgcdwmsasn-ap01:5001";
// const String urlAPI = "http://bgcdwmsasn-ap01:5002";
// http://bgcdwmsasn-ap01:5002/'

// Android Emurator localhost ip address
// const String urlAPI = "http://10.0.2.2:5720";
// Genymotion localhost ip address
// const String urlAPI = "http://10.0.3.2:5720";

/* Localhost  Config */
// const String authApiUrl = "http://192.168.1.13:5001";
// const String adminApiUrl = "http://192.168.1.13:4220";
// const String pdaApiUrl = "http://192.168.1.13:5002";
// const String accnApiUrl = "http://192.168.1.13:4220";
// const String recvApiUrl = "http://192.168.1.13:4320";
// const String taskApiUrl = "http://192.168.1.13:4420";
// const String prepApiUrl = "http://192.168.1.13:4620";
// const String countApiUrl = "http://192.168.1.13:4520";

/* Development Config*/
const String authApiUrl = "http://10.4.5.194:5001";
const String adminApiUrl = "http://10.4.5.194:4220";
const String pdaApiUrl = "http://10.4.5.194:5002";
const String accnApiUrl = "http://10.4.5.194:4220";
const String recvApiUrl = "http://10.4.5.194:4320";
const String taskApiUrl = "http://10.4.5.194:4420";
const String prepApiUrl = "http://10.4.5.194:4620";
const String countApiUrl = "http://10.4.5.194:4520";

/* Production Config*/
// const String authApiUrl = "http://172.28.8.48:5101";
// const String adminApiUrl = "http://172.28.8.48:5102";
// const String accnApiUrl = "http://172.28.8.48:5102";
// const String countApiUrl = "http://172.28.8.48:5103";
// const String recvApiUrl = "http://172.28.8.48:5107";
// const String taskApiUrl = "http://172.28.8.48:5109";
// const String prepApiUrl = "http://172.28.8.48:5108";
// const String pdaApiUrl = "http://172.28.8.48:5110";

// Colors
const primaryColor = Color(0xff153C6A);
const primaryLightColor = Color(0xff5B6672);
const primaryGradientColor = LinearGradient(
  begin: Alignment.topLeft,
  end: Alignment.bottomRight,
  colors: [Color(0xff5B6672), Color(0xff153C6A)],
);
const secondaryColor = Color(0xFF979797);
const successColor = Color(0xFF89DA59);
const warningColor = Color(0xFFEE8F22);
const dangerColor = Color(0xFFCE4100);
const dangerColor50 = Color(0xFFF98866);
const infoColor = Color(0xFF71D2FF);
const defaultColor = Color(0xFF757575);
const iconBgColor = Color(0xFFF7F7F7);
const labelBgColor = Color(0xFFEBEBEB);
const animationDuration = Duration(milliseconds: 200);
// Color schemes
const colorBlue = Color(0xFF4D85BD);
const colorStem = Color(0xFF80BD9E);
const colorSpringGreen = Color(0xFF89DA59);
const colorLeafyGreen = Color(0xffBBCE00);
const colorSunflower = Color(0xFFF5E356);
const colorPetal = Color(0xFFF98866);
const colorWatermelon = Color(0xFFFA6E59);
const colorPoppy = Color(0xFFFF420E);
const colorSeeds = Color(0xFFCB6318);

const double defaultPadding = 20;
final headingStyle = TextStyle(
  fontSize: getProportionateScreenWidth(28),
  fontWeight: FontWeight.bold,
  color: Colors.black,
  height: 1.5,
);

const defaultDuration = Duration(milliseconds: 250);

final otpInputDecoration = InputDecoration(
  contentPadding: EdgeInsets.symmetric(vertical: getProportionateScreenWidth(15)),
  border: outlineInputBorder(),
  focusedBorder: outlineInputBorder(),
  enabledBorder: outlineInputBorder(),
);

OutlineInputBorder outlineInputBorder() {
  return OutlineInputBorder(
    borderRadius: BorderRadius.circular(
      getProportionateScreenWidth(15),
    ),
    borderSide: BorderSide(color: defaultColor),
  );
}
