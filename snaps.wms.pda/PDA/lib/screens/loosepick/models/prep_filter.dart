class PrepFilter {
  String spcarea;
  String preptype;
  String tflow;
  String prepno;

  PrepFilter({this.spcarea, this.preptype, this.tflow, this.prepno});

  PrepFilter.fromJson(Map<String, dynamic> json) {
    spcarea = json['spcarea'];
    preptype = json['preptype'];
    tflow = json['tflow'];
    prepno = json['prepno'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['spcarea'] = this.spcarea;
    data['preptype'] = this.preptype;
    data['tflow'] = this.tflow;
    data['prepno'] = this.prepno;
    return data;
  }
}
