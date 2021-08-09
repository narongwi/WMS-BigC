import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'package:wms/screens/receive/models/product_active.dart';
import 'package:wms/screens/receive/models/product_radio.dart';
import 'package:wms/screens/receive/models/save_payload.dart';
import 'package:wms/screens/receive/models/searchpo_model.dart';
import 'package:wms/screens/receive/models/searchpo_payload.dart';
import '../../../constants.dart';

class ReceiveService {
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

  Future<SearchPO> searchPO(String inorder) async {
    SearchPayload payload = SearchPayload();
    payload.inorder = inorder;
    final baseUrl = "$recvApiUrl/inbound/get/0";
    print("======>$baseUrl");
    final data = jsonEncode(payload);
    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("search po ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      var result = jsonDecode(response.body);
      var isorder = result["inorder"];
      if (isorder == null) {
        throw Exception("Data not Found");
      } else {
        return SearchPO.fromJson(result);
      }
    } else {
      throw Exception("Data not Found");
    }
  }

  Future<List<ProductRadio>> getRadio(String art, int pv, int lv) async {
    final baseUrl = "$recvApiUrl/inbound/getproductratio/$art/$pv/$lv";
    List<ProductRadio> radios = <ProductRadio>[];
    print("======>$baseUrl");
    final response = await http.post(
      baseUrl,
      body: "tron a live",
      headers: await header(),
    );
    print("Product Radio ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        radios.add(new ProductRadio.fromJson(v));
      });
      return radios;
    } else {
      throw Exception("${response.body}");
    }
  }

  Future<Product> getProductInfo(String productCode) async {
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

  Future<bool> setStaging(String pono, String staging) async {
    String baseUrl = "$recvApiUrl/inbound/setstaging/$pono/$staging";
    print("======>$baseUrl");
    final response = await http.post(
      baseUrl,
      body: "tron a live",
      headers: await header(),
    );

    print("set Staging ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      try {
        baseUrl = "$recvApiUrl/inbound/setunloadstart/$pono";
        print("======>$baseUrl");
        final setunloadstart = await http.post(
          baseUrl,
          body: "tron a live",
          headers: await header(),
        );
        print("start loading ==> ${setunloadstart.statusCode}");
        return true;
      } catch (se) {
        throw Exception("start loading Failed!");
      }
    } else {
      throw Exception("Set Staging Failed!");
    }
  }

  Future<bool> setStart(String pono) async {
    String baseUrl = "$recvApiUrl/inbound/setunloadstart/$pono";
    print("======>$baseUrl");
    final response = await http.post(
      baseUrl,
      body: "tron a live",
      headers: await header(),
    );

    print("start loading ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      return true;
    } else {
      throw Exception("start loading Failed!");
    }
  }

  Future<bool> confirm(SavePlayload playload) async {
    final baseUrl = "$recvApiUrl/inbound/upsertlx/0";
    print("======>$baseUrl");
    final response = await http.post(
      baseUrl,
      body: jsonEncode(playload.toJson()),
      headers: await header(),
    );

    print("upsertlx ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      var data = json.decode(response.body);
      final baseUrl = "$recvApiUrl/inbound/commitlx/0";
      print("======>$baseUrl");
      await http.post(
        baseUrl,
        body: jsonEncode(data[0]),
        headers: await header(),
      );
      print("commitlx ==> ${response.statusCode}");
      return true;
    } else {
      if (response.body.indexOf("errid") == -1) {
        throw Exception(response.body);
      } else {
        // for custom api response message
        var data = json.decode(response.body);
        throw Exception(data["message"]);
      }
    }
  }

  Future<bool> setFinished(String pono) async {
    String baseUrl = "$recvApiUrl/inbound/setunloadend/$pono";
    print("======>$baseUrl");
    final response = await http.post(
      baseUrl,
      body: "tron a live",
      headers: await header(),
    );

    print("set Unload ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      return true;
    } else {
      throw Exception("set Unload Failed!");
    }
  }

  Future<bool> setClose(String pono) async {
    String baseUrl = "$recvApiUrl/inbound/setfinish/$pono";
    print("======>$baseUrl");
    final response = await http.post(
      baseUrl,
      body: "tron a live",
      headers: await header(),
    );

    print("set Finish ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      return true;
    } else {
      throw Exception("setFinish Failed!");
    }
  }
}
