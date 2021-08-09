import 'permission.dart';

class Modules {
  String modulecode;
  String modulename;
  String moduleicon;
  List<Permission> permission;

  Modules({this.modulecode, this.modulename, this.moduleicon, this.permission});

  Modules.fromJson(Map<String, dynamic> json) {
    modulecode = json['modulecode'];
    modulename = json['modulename'];
    moduleicon = json['moduleicon'];
    if (json['permission'] != null) {
      permission = <Permission>[];
      json['permission'].forEach((v) {
        permission.add(new Permission.fromJson(v));
      });
    }
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['modulecode'] = this.modulecode;
    data['modulename'] = this.modulename;
    data['moduleicon'] = this.moduleicon;
    if (this.permission != null) {
      data['permission'] = this.permission.map((v) => v.toJson()).toList();
    }
    return data;
  }
}
