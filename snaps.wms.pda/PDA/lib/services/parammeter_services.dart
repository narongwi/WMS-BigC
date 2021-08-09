import 'dart:async';
import 'dart:convert';

import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'package:wms/models/parameter_model.dart';
import '../constants.dart';

class ParameterService {
  Future<List<Parameters>> getAllParameter() async {
    final preferences = await SharedPreferences.getInstance();
    final baseUrl = "$adminApiUrl/admparameter/getParameterList/0";
    List<Parameters> params = <Parameters>[];
    print("======>$baseUrl");
    final response = await http.post(baseUrl, body: "", headers: {
      "Content-Type": "application/json",
      "Authorization": "Bearer " + preferences.getString('tokenkey'),
      "accncode": preferences.getString('username'),
      "accscode": preferences.getString('tokenid'),
      "depot": preferences.getString('depot'),
      "lang": preferences.getString('lang'),
      "orgcode": preferences.getString('orgcode'),
      "site": preferences.getString('site'),
    });

    print("get unit ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        params.add(new Parameters.fromJson(v));
      });
      return params;
    } else {
      throw Exception("${response.body}");
    }
  }

  Future<List<Parameters>> getParameter(String module, String type) async {
    final preferences = await SharedPreferences.getInstance();
    final baseUrl = "$adminApiUrl/admparameter/getParameter/$module/$type/0";
    List<Parameters> params = <Parameters>[];
    print("======>$baseUrl");
    final response = await http.post(baseUrl, body: "", headers: {
      "Content-Type": "application/json",
      "Authorization": "Bearer " + preferences.getString('tokenkey'),
      "accncode": preferences.getString('username'),
      "accscode": preferences.getString('tokenid'),
      "depot": preferences.getString('depot'),
      "lang": preferences.getString('lang'),
      "orgcode": preferences.getString('orgcode'),
      "site": preferences.getString('site'),
    });

    print("get parameter ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        params.add(new Parameters.fromJson(v));
      });
      return params;
    } else {
      throw Exception("${response.body}");
    }
  }
}
