import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'package:wms/screens/distribution/models/empty_model.dart';
import 'package:wms/screens/receive/models/product_active.dart';
import 'package:wms/screens/stockcount/models/countline_model.dart';
import 'package:wms/screens/stockcount/models/countplan_model.dart';
import 'package:wms/screens/stockcount/models/counttask_model.dart';
import 'package:wms/screens/stockcount/models/findcount_model.dart';
import '../../../constants.dart';

class CountServices {
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

  Future<List<CountTask>> countTask() async {
    final baseUrl = "$countApiUrl/count/listTask/0";
    final tasks = <CountTask>[];
    final response = await http.post(
      baseUrl,
      body: jsonEncode({}),
      headers: await header(),
    );
    print("CountTask ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        tasks.add(new CountTask.fromJson(v));
      });
      return tasks;
    } else {
      throw Exception(getMessage(response.body));
    }
  }

  Future<List<Countplan>> countList(String countcode) async {
    final baseUrl = "$countApiUrl/count/listPlan/0";
    final preps = <Countplan>[];
    final data = jsonEncode({"countcode": countcode});
    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("Countplan ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        preps.add(new Countplan.fromJson(v));
      });
      return preps;
    } else {
      throw Exception(getMessage(response.body));
    }
  }

  Future<List<Countline>> countLine(Countplan plan) async {
    final baseUrl = "$countApiUrl/count/listLine/0";
    final data = jsonEncode(plan);
    final lines = <Countline>[];
    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("countLine ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        lines.add(new Countline.fromJson(v));
      });
      return lines;
    } else {
      throw Exception(getMessage(response.body));
    }
  }

  Future<List<Countline>> findCountLine(FindCountLine find) async {
    final baseUrl = "$countApiUrl/count/getLine/0";
    final data = jsonEncode(find);
    final lines = <Countline>[];
    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print(response.body);
    print("findCountLine ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        lines.add(new Countline.fromJson(v));
      });
      return lines;
    } else {
      throw Exception(getMessage(response.body));
    }
  }

  Future<bool> saveCount(List<Countline> countLine) async {
    final baseUrl = "$countApiUrl/count/upsertLine/0";
    final data = jsonEncode(countLine);
    print(baseUrl);
    print(data);

    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("saveCount ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      return true;
    } else {
      throw Exception(getMessage(response.body));
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

  String getMessage(String resbody) {
    if (resbody.indexOf("errid") == -1) {
      throw Exception(resbody);
    } else {
      // for custom api response message
      var data = json.decode(resbody);
      throw Exception(data["message"]);
    }
  }

  Future<List<Empty>> findHU(String scanhuno) async {
    final hufilter = EmptyFilter(huno: scanhuno);
    final baseUrl = "$prepApiUrl/ouhanderlingunit/list/0";
    final data = jsonEncode(hufilter.toJson());
    final resp = await http.post(baseUrl, body: data, headers: await header());
    final empty = <Empty>[];
    print("findHU ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      final _json = (jsonDecode(resp.body) as List);
      _json.forEach((v) => empty.add(new Empty.fromJson(v)));
      return empty;
    } else {
      throw Exception(getMessage(resp.body));
    }
  }
}


// http://bgcdwmsasn-ap01:4520/count/listPlan/17a03eee-0080-snaps-890b-d0de99db0900
// http://bgcdwmsasn-ap01:4520/count/listLine/17a03eee-0080-snaps-890b-d0de99db0900
// /count/upsertPlan/"

