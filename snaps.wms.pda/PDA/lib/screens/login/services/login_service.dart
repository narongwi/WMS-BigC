import 'dart:async';
import 'dart:convert';
import 'package:http/http.dart' as http;
// import 'package:uuid/uuid.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:wms/constants.dart';
import 'package:wms/helpers/jwt_helper.dart';
import 'package:wms/screens/Upgrade/upgrade_model.dart';
import 'package:wms/screens/login/models/auth_model.dart';

class LoginService {
  Future<ResponseAth> verify(String username, String password) async {
    // final uuid = Uuid();
    // final requid = uuid.v4();
    final reqbaseurl = "$authApiUrl/auth/verify/0";
    final reqheader = {"Content-Type": "application/json;charset=utf-8"};
    print("Auth ==> $reqbaseurl");

    // Login Parameter
    final reqbody = jsonEncode({
      "accncode": username,
      "email": username,
      "password": password,
      "lang": "EN",
    });
    // Call
    final response = await http.post(
      reqbaseurl,
      headers: reqheader,
      body: reqbody,
    );

    print("Auth ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      final preferences = await SharedPreferences.getInstance();
      final result = ResponseAth.fromJson(jsonDecode(response.body));

      final reader = JwtReader();
      // Read JWT Result
      reader.parseToken(result.message);

      // Save to Memory
      preferences.setString('tokenid', result.reqid);
      preferences.setString('tokenkey', result.message);
      preferences.setString('username', reader.userName);
      preferences.setString('fullname', reader.fullName);
      return result;
    } else {
      print("http:status ${response.statusCode}");
      print("http:body ${response.body}");
      throw Exception("${response.body}");
    }
  }

  Future<UpgradInfo> checkVersion(String accncode) async {
    final reqbaseurl = "$pdaApiUrl/appinfo/version/$accncode";
    final response = await http.get(reqbaseurl);
    print("checkVersion ==> $reqbaseurl");
    if (response.statusCode == 200) {
      return UpgradInfo.fromJson(jsonDecode(response.body));
    } else {
      print("http:status ${response.statusCode}");
      print("http:body ${response.body}");
      throw Exception("${response.body}");
    }
  }
}
