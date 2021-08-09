class FindCountLine {
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String countcode;
  String plancode;
  String loccode;
  String tflow;

  FindCountLine(
      {this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.countcode,
      this.plancode,
      this.loccode,
      this.tflow});

  FindCountLine.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    countcode = json['countcode'];
    plancode = json['plancode'];
    loccode = json['loccode'];
    tflow = json['tflow'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['countcode'] = this.countcode;
    data['plancode'] = this.plancode;
    data['loccode'] = this.loccode;
    data['tflow'] = this.tflow;
    return data;
  }
}
