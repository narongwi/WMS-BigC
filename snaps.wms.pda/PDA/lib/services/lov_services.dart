import 'dart:async';
import 'dart:convert';

import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import '../constants.dart';
import '../models/lov_model.dart';

class LovService {
  Future<Map<String, String>> header() async {
    final preferences = await SharedPreferences.getInstance();
    return <String, String>{
      "Content-Type": "application/json",
      "Authorization": "Bearer " + preferences.getString('tokenkey'),
      "accncode": preferences.getString('username'),
      "accscode": preferences.getString('tokenid'),
      "depot": preferences.getString('depot'),
      "lang": preferences.getString('lang'),
      "orgcode": preferences.getString('orgcode'),
      "site": preferences.getString('site'),
    };
  }

  // preplist item  task
  Future<List<Lov>> getUnit() async {
    final baseUrl = "$adminApiUrl/LOV/State/UNIT/KEEP/0";
    List<Lov> lov = <Lov>[];
    print("======>$baseUrl");
    final response = await http.post(
      baseUrl,
      body: "",
      headers: await header(),
    );

    print("get unit ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        lov.add(new Lov.fromJson(v));
      });

      return lov;
    } else {
      throw Exception("${response.body}");
    }
  }

  // preplist item  task
  Future<List<Lov>> getTflow() async {
    final baseUrl = "$adminApiUrl/LOV/State/ALL/FLOW/0";
    List<Lov> lov = <Lov>[];
    print("======>$baseUrl");
    final response = await http.post(
      baseUrl,
      body: "",
      headers: await header(),
    );

    print(response.body);
    print("get unit ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        lov.add(new Lov.fromJson(v));
      });

      return lov;
    } else {
      throw Exception("${response.body}");
    }
  }
}
