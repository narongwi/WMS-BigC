import 'dart:convert';
import 'dart:io';

import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'package:wms/models/response_model.dart';

class NetworkManager {
  static Future<ResponseData> getData(String url) async {
    ResponseData result = ResponseData();
    try {
      final preferences = await SharedPreferences.getInstance();
      final httpResponse = await http.get(url, headers: {
        "Content-Type": "application/json;charset=utf-8",
        "Authorization": "Bearer " + preferences.getString('tokenkey'),
      });

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
      final preferences = await SharedPreferences.getInstance();
      final httpResponse = await http.post(url, body: httpReqbody, headers: {
        "Content-Type": "application/json;charset=utf-8",
        "Authorization": "Bearer " + preferences.getString('tokenkey'),
      });

      if (httpResponse.statusCode == 200 || httpResponse.statusCode == 400) {
        result = ResponseData.fromJson(jsonDecode(httpResponse.body));
      } else {
        try {
          print("httpResponse : ${httpResponse.statusCode}");
          print(result.message);
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
