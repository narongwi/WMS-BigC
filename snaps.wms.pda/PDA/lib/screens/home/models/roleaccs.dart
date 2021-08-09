import 'modules.dart';

class Roleaccs {
  String site;
  String depot;
  String rolecode;
  String rolename;
  List<Modules> modules;
  String roletype;

  Roleaccs(
      {this.site,
      this.depot,
      this.rolecode,
      this.rolename,
      this.modules,
      this.roletype});

  Roleaccs.fromJson(Map<String, dynamic> json) {
    site = json['site'];
    depot = json['depot'];
    rolecode = json['rolecode'];
    rolename = json['rolename'];
    if (json['modules'] != null) {
      modules = <Modules>[];
      json['modules'].forEach((v) {
        modules.add(new Modules.fromJson(v));
      });
    }
    roletype = json['roletype'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['rolecode'] = this.rolecode;
    data['rolename'] = this.rolename;
    if (this.modules != null) {
      data['modules'] = this.modules.map((v) => v.toJson()).toList();
    }
    data['roletype'] = this.roletype;
    return data;
  }
}
