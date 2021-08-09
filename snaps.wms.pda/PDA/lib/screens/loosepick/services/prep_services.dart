import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'package:wms/screens/loosepick/models/prep_detail.dart';
import 'package:wms/screens/loosepick/models/prep_filter.dart';
import 'package:wms/screens/loosepick/models/prep_lists.dart';
import 'package:wms/screens/receive/models/product_active.dart';
import '../../../constants.dart';

class LoosePickServices {
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

  Future<List<PrepLists>> listprep(PrepFilter filter) async {
    final baseUrl = "$prepApiUrl/ouprep/list/0";
    final preps = <PrepLists>[];
    final data = jsonEncode(filter.toJson());
    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("listprep ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        preps.add(new PrepLists.fromJson(v));
      });
      return preps;
    } else {
      throw Exception(getMessage(response.body));
    }
  }

  Future<PrepDetails> getprep(PrepLists prepitem) async {
    final baseUrl = "$prepApiUrl/ouprep/get/0";
    final data = jsonEncode(prepitem.toJson());
    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("getprep ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      return PrepDetails.fromJson(jsonDecode(response.body));
    } else {
      throw Exception(getMessage(response.body));
    }
  }

  Future<bool> setstart(PrepLines prepline) async {
    final baseUrl = "$prepApiUrl/ouprep/setStart/0";
    final data = jsonEncode(prepline.toJson());
    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("setstart ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      return true;
    } else {
      throw Exception(getMessage(response.body));
    }
  }

  Future<bool> setpick(List<PrepLines> prepline) async {
    final baseUrl = "$prepApiUrl/ouprep/opsPick/0";
    // final baseUrl = "$prepApiUrl/ouprep/opsPick/0";
    final data = jsonEncode(prepline);
    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("setpick ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      return true;
    } else {
      throw Exception(getMessage(response.body));
    }
  }

  Future<bool> setend(PrepDetails prepdt) async {
    final baseUrl = "$prepApiUrl/ouprep/setEnd/0";
    final data = jsonEncode(prepdt.toJson());
    print("finsih=>$baseUrl");

    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("setend ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      return true;
    } else {
      throw Exception(getMessage(response.body));
    }
  }

  String getMessage(String resbody) {
    if (resbody.indexOf("errid") == -1) {
      throw Exception(resbody);
    } else {
      // for custom api response message
      var data = json.decode(resbody);
      throw Exception(data["message"]);
    }
  }

  Future<Product> getProduct(String productCode) async {
    final baseUrl = "$pdaApiUrl/api/product/info/$productCode";
    print("======>$baseUrl");
    final response = await http.get(
      baseUrl,
      headers: await header(),
    );
    print("Product Active ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      return Product.fromJson(jsonDecode(response.body));
    } else {
      throw Exception("Data not Found");
    }
  }

  Future<Product> productInfo(String article, String lv) async {
    final baseUrl = "$pdaApiUrl/api/product/article/$article/$lv";
    print("======>$baseUrl");
    final response = await http.get(
      baseUrl,
      headers: await header(),
    );
    print("Product Active ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      return Product.fromJson(jsonDecode(response.body));
    } else {
      throw Exception("Data not Found");
    }
  }
}
