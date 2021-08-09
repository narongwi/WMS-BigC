import 'dart:convert';

import 'package:shared_preferences/shared_preferences.dart';
// import 'package:uuid/uuid.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:http/http.dart' as http;
import '../../../constants.dart';

class ProfileService {
  // find supplier po header
  Future<Profiles> getAccount() async {
    // final uuid = Uuid();
    // final requid = uuid.v4();
    Profiles profiles = Profiles();

    // get parameter
    final preferences = await SharedPreferences.getInstance();

    // header
    // final baseUrl = "$accnApiUrl/account/getprofile/" + requid;
    final baseUrl = "$accnApiUrl/account/getPdaProfile/0";

    print("$baseUrl");
    // call api
    final response = await http.post(baseUrl, body: null, headers: {
      "Content-Type": "application/json;charset=utf-8",
      "Authorization": "Bearer " + preferences.getString('tokenkey'),
      "accncode": preferences.getString('tokenkey'),
      "accscode": preferences.getString('tokenid'),
      "depot": "",
      "lang": "EN",
      "orgcode": "",
      "site": ""
    });

    print("get profile ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      profiles = Profiles.fromJson(jsonDecode(response.body));
      preferences.setString('depot', profiles.depot);
      preferences.setString('site', profiles.site);
      preferences.setString('orgcode', profiles.orgcode);
      preferences.setString('lang', profiles.lang);
      return profiles;
    } else {
      throw Exception("${response.body}");
    }
  }

  Future<void> logout() async {
    final preferences = await SharedPreferences.getInstance();
    preferences.remove('tokenid');
    preferences.remove('tokenkey');
    preferences.remove('username');
    preferences.remove('fullname');
    preferences.remove('depot');
    preferences.remove('site');
    preferences.remove('orgcode');
    preferences.remove('lang');
  }
}
