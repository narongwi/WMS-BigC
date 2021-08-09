class UpgradInfo {
  String name;
  String package;
  String version;
  String information;
  String downloaduri;
  String apkname;

  UpgradInfo(
      {this.name,
      this.package,
      this.version,
      this.information,
      this.downloaduri,
      this.apkname});

  UpgradInfo.fromJson(Map<String, dynamic> json) {
    name = json['name'];
    package = json['package'];
    version = json['version'];
    information = json['information'];
    downloaduri = json['downloaduri'];
    apkname = json['apkname'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['name'] = this.name;
    data['package'] = this.package;
    data['version'] = this.version;
    data['information'] = this.information;
    data['downloaduri'] = this.downloaduri;
    data['apkname'] = this.apkname;
    return data;
  }
}
