class PrepLists {
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String routeno;
  String huno;
  String preptype;
  String prepno;
  String prepdate;
  int priority;
  String thcode;
  String spcorder;
  String spcarticle;
  String tflow;
  double capacity;
  String thname;
  String picker;
  double preppct;
  String preptypename;
  String przone;

  PrepLists(
      {this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.routeno,
      this.huno,
      this.preptype,
      this.prepno,
      this.prepdate,
      this.priority,
      this.thcode,
      this.spcorder,
      this.spcarticle,
      this.tflow,
      this.capacity,
      this.thname,
      this.picker,
      this.preppct,
      this.preptypename,
      this.przone});

  PrepLists.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    routeno = json['routeno'];
    huno = json['huno'];
    preptype = json['preptype'];
    prepno = json['prepno'];
    prepdate = json['prepdate'];
    priority = json['priority'];
    thcode = json['thcode'];
    spcorder = json['spcorder'];
    spcarticle = json['spcarticle'];
    tflow = json['tflow'];
    capacity = json['capacity'];
    thname = json['thname'];
    picker = json['picker'];
    preppct = json['preppct'];
    preptypename = json['preptypename'];
    przone = json['przone'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['routeno'] = this.routeno;
    data['huno'] = this.huno;
    data['preptype'] = this.preptype;
    data['prepno'] = this.prepno;
    data['prepdate'] = this.prepdate;
    data['priority'] = this.priority;
    data['thcode'] = this.thcode;
    data['spcorder'] = this.spcorder;
    data['spcarticle'] = this.spcarticle;
    data['tflow'] = this.tflow;
    data['capacity'] = this.capacity;
    data['thname'] = this.thname;
    data['picker'] = this.picker;
    data['preppct'] = this.preppct;
    data['preptypename'] = this.preptypename;
    data['przone'] = this.przone;
    return data;
  }
}
