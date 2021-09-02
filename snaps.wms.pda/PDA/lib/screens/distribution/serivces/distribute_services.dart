import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'package:wms/screens/distribution/models/baseclose_model.dart';
import 'package:wms/screens/distribution/models/empty_model.dart';
import 'package:wms/screens/distribution/models/prepdistribute_model.dart';
import 'package:wms/screens/receive/models/product_active.dart';
import '../../../constants.dart';

class DistributeServices {
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

  Future<List<Distribution>> listprep(DistributeFilter filter) async {
    final baseUrl = "$prepApiUrl/ouprep/list/0";
    final preps = <Distribution>[];
    final data = jsonEncode(filter);
    final resp = await http.post(baseUrl, body: data, headers: await header());
    print("listprep ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      final _json = (jsonDecode(resp.body) as List);
      _json.forEach((v) => preps.add(new Distribution.fromJson(v)));
      return preps;
    } else {
      throw Exception(getMessage(resp.body));
    }
  }

  Future<DistrbInfo> getprep(Distribution prepitem) async {
    final baseUrl = "$prepApiUrl/ouprep/get/0";
    final data = jsonEncode(prepitem);
    final resp = await http.post(baseUrl, body: data, headers: await header());
    print("getprep ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      return DistrbInfo.fromJson(jsonDecode(resp.body));
    } else {
      throw Exception(getMessage(resp.body));
    }
  }

  Future<bool> setstart(DistrbInfo prepinfo) async {
    final baseUrl = "$prepApiUrl/ouprep/setStart/0";
    final data = jsonEncode(prepinfo);
    final resp = await http.post(baseUrl, body: data, headers: await header());
    print("setstart ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      return true;
    } else {
      throw Exception(getMessage(resp.body));
    }
  }

  Future<List<Empty>> listEmpty(EmptyFilter filter) async {
    final baseUrl = "$prepApiUrl/ouhanderlingunit/list/0";
    final data = jsonEncode(filter);
    final resp = await http.post(baseUrl, body: data, headers: await header());
    final empty = <Empty>[];
    print("getprep ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      final _json = (jsonDecode(resp.body) as List);
      _json.forEach((v) => empty.add(new Empty.fromJson(v)));
      return empty;
    } else {
      throw Exception(getMessage(resp.body));
    }
  }

  Future<Empty> getEmpty(EmptyFilter filter) async {
    final baseUrl = "$prepApiUrl/ouhanderlingunit/get/0";
    final data = jsonEncode(filter);
    final resp = await http.post(baseUrl, body: data, headers: await header());
    print("getprep ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      return Empty.fromJson(jsonDecode(resp.body));
    } else {
      throw Exception(getMessage(resp.body));
    }
  }

  Future<bool> genEmpty(EmptyGen empty) async {
    final baseUrl = "$prepApiUrl/ouhanderlingunit/genereate/0";
    final data = jsonEncode(empty);
    final resp = await http.post(baseUrl, body: data, headers: await header());
    print("genEmpty ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      return true;
    } else {
      throw Exception(getMessage(resp.body));
    }
  }

  Future<bool> putline(DistrbLine distrbLine) async {
    final baseUrl = "$prepApiUrl/ouprep/opsPut/0";
    final data = jsonEncode(distrbLine);
    final resp = await http.post(baseUrl, body: data, headers: await header());
    print("putline ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      return true;
    } else {
      throw Exception(getMessage(resp.body));
    }
  }

  Future<bool> finishDistr(DistrbInfo prepdt) async {
    final baseUrl = "$prepApiUrl/ouprep/setEnd/0";
    final data = jsonEncode(prepdt);
    print("==> Set end");
    print("==> $baseUrl");
    final resp = await http.post(baseUrl, body: data, headers: await header());
    print("setend ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      return true;
    } else {
      throw Exception(getMessage(resp.body));
    }
  }

  Future<List<BaseClose>> baseCloselist(BaseCloseFilter filter) async {
    final baseUrl = "$prepApiUrl/ouhanderlingunit/list/0";
    final data = jsonEncode(filter);
    final resp = await http.post(baseUrl, body: data, headers: await header());
    final empty = <BaseClose>[];
    print("baseCloselist ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      final _json = (jsonDecode(resp.body) as List);
      _json.forEach((v) => empty.add(new BaseClose.fromJson(v)));
      return empty;
    } else {
      throw Exception(getMessage(resp.body));
    }
  }

  Future<List<BaseCloseLine>> baseCloseLine(BaseClose baseClose) async {
    final baseUrl = "$prepApiUrl/ouhanderlingunit/linesnonsum/0";
    final data = jsonEncode(baseClose);
    final resp = await http.post(baseUrl, body: data, headers: await header());
    final lines = <BaseCloseLine>[];
    print("baseCloseLine ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      final _json = (jsonDecode(resp.body) as List);
      _json.forEach((v) => lines.add(new BaseCloseLine.fromJson(v)));
      return lines;
    } else {
      throw Exception(getMessage(resp.body));
    }
  }

  Future<bool> baseCLoseHU(BaseClose baseClose) async {
    final baseUrl = "$prepApiUrl/ouhanderlingunit/close/0";
    final data = jsonEncode(baseClose);
    final resp = await http.post(baseUrl, body: data, headers: await header());
    print("baseCLoseHU ==> ${resp.statusCode}");
    if (resp.statusCode == 200) {
      return true;
    } else {
      throw Exception(getMessage(resp.body));
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
    final response = await http.get(baseUrl, headers: await header());
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
