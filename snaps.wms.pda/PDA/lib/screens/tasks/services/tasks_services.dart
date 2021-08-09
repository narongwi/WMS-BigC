import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:http/http.dart' as http;
import 'package:wms/screens/receive/models/product_active.dart';
import 'package:wms/screens/tasks/models/taskfilter_models.dart';
import 'package:wms/screens/tasks/models/tasklist_models.dart';
import 'package:wms/screens/tasks/models/taskmovement_model.dart';
import '../../../constants.dart';

class TaskService {
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

  Future<List<TaskList>> lists(TaskFilter filter) async {
    final baseUrl = "$taskApiUrl/task/list/0";
    List<TaskList> tasks = <TaskList>[];
    print("======>$baseUrl");
    final data = jsonEncode(filter.toJson());
    print("header==> $this.header");
    final response = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("list task ==> ${response.statusCode}");
    if (response.statusCode == 200) {
      (jsonDecode(response.body) as List).forEach((v) {
        tasks.add(new TaskList.fromJson(v));
      });
      return tasks;
    } else {
      throw Exception(getMessage(response.body));
    }
  }

  Future<TaskMovement> assignTask(TaskList tasks, String assignee) async {
    final baseUrl = "$taskApiUrl/task/get/0";
    print("======>$baseUrl");
    final data = jsonEncode(tasks.toJson());
    final resgethu = await http.post(
      baseUrl,
      body: data,
      headers: await header(),
    );
    print("get movement ==> ${resgethu.statusCode}");
    if (resgethu.statusCode == 200) {
      final movement = TaskMovement.fromJson(jsonDecode(resgethu.body));
      final baseUrl = "$taskApiUrl/task/assign/0";
      print("======>$baseUrl");
      //Call Assign Task
      movement.lines[0].accnassign = assignee;
      final resassign = await http.post(
        baseUrl,
        body: jsonEncode(movement.toJson()),
        headers: await header(),
      );
      print("task assign ==> ${resassign.statusCode}");
      return movement;
    } else {
      throw Exception(getMessage(resgethu.body));
    }
  }

  Future<bool> confirm(TaskMovement movement) async {
    final baseUrl = "$taskApiUrl/task/confirm/0";
    print("======>$baseUrl");
    final response = await http.post(
      baseUrl,
      body: jsonEncode(movement.toJson()),
      headers: await header(),
    );

    print("confirm task ==> ${response.statusCode}");
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
}
