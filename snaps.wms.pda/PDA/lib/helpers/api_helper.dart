import 'dart:convert';
import 'dart:io';

import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'package:wms/models/response_model.dart';

class API {
  static Future<ResponseData> getData(String url) async {
    ResponseData result = ResponseData();
    try {
      final preferences = await SharedPreferences.getInstance();
      final httpResponse = await http.get(url, headers: {
        "Content-Type": "application/json;charset=utf-8",
        "Authorization": "Bearer " + preferences.getString('tokenkey'),
      });

      print(url);
      print("Status ==> ${httpResponse.statusCode}");
      print("httpMessage ==> ${result.message}");
      if (httpResponse.statusCode == 200 || httpResponse.statusCode == 400) {
        result = ResponseData.fromJson(jsonDecode(httpResponse.body));
      } else {
        try {
          result = ResponseData.fromJson(jsonDecode(httpResponse.body));
        } catch (e) {
          result.success = false;
          result.message = 'Data Not Found';
        }
      }
    } on SocketException {
      result.success = false;
      result.message = 'No Internet';
    } on HttpException {
      result.success = false;
      result.message = 'No Service Found';
    } on FormatException {
      result.success = false;
      result.message = 'Invalid Data Format';
    } catch (e) {
      result.success = false;
      result.message = e.toString();
    }
    return result;
  }

  static Future<ResponseData> postData(String url, dynamic data) async {
    ResponseData result = ResponseData();
    try {
      final httpReqbody = jsonEncode(data);
      print("header data $data");

      print("==========================>");
      print(httpReqbody);

      final preferences = await SharedPreferences.getInstance();
      final httpResponse = await http.post(url, body: httpReqbody, headers: {
        "Content-Type": "application/json;charset=utf-8",
        "Authorization": "Bearer " + preferences.getString('tokenkey'),
      });

      print("Status ==> ${httpResponse.statusCode}");
      print("httpMessage ==> ${result.message}");

      if (httpResponse.statusCode == 200 || httpResponse.statusCode == 400) {
        result = ResponseData.fromJson(jsonDecode(httpResponse.body));
      } else {
        try {
          print("httpResponse : ${httpResponse.statusCode}");
          result = ResponseData.fromJson(jsonDecode(httpResponse.body));
        } catch (e) {
          result.success = false;
          result.message = 'Error ${httpResponse.statusCode}';
        }
      }
    } on SocketException {
      result.success = false;
      result.message = 'No Internet';
    } on HttpException {
      result.success = false;
      result.message = 'No Service Found';
    } on FormatException {
      result.success = false;
      result.message = 'Invalid Data Format';
    } catch (e) {
      result.success = false;
      result.message = e.toString();
    }
    return result;
  }


  static Future<ResponseData> postParam(String url, dynamic data) async {
    ResponseData result = ResponseData();
    try {
      final httpReqbody = jsonEncode(data);
      print("header data $data");

      print("==========================>");
      print(httpReqbody);

      final preferences = await SharedPreferences.getInstance();
      final httpResponse = await http.post(url, body: httpReqbody, headers: {
        "Content-Type": "application/json;charset=utf-8",
        "Authorization": "Bearer " + preferences.getString('tokenkey'),
        "accncode":"",
        "accscode":"",
        "depot":"",
        "lang":"",
        "orgcode":"",
        "site":""
      });

      print("Status ==> ${httpResponse.statusCode}");
      print("httpMessage ==> ${result.message}");

      if (httpResponse.statusCode == 200 || httpResponse.statusCode == 400) {
        result = ResponseData.fromJson(jsonDecode(httpResponse.body));
      } else {
        try {
          print("httpResponse : ${httpResponse.statusCode}");
          result = ResponseData.fromJson(jsonDecode(httpResponse.body));
        } catch (e) {
          result.success = false;
          result.message = 'Error ${httpResponse.statusCode}';
        }
      }
    } on SocketException {
      result.success = false;
      result.message = 'No Internet';
    } on HttpException {
      result.success = false;
      result.message = 'No Service Found';
    } on FormatException {
      result.success = false;
      result.message = 'Invalid Data Format';
    } catch (e) {
      result.success = false;
      result.message = e.toString();
    }
    return result;
  }
}
