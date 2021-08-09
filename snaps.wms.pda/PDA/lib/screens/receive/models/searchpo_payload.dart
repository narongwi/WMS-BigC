class SearchPayload {
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String thcode;
  String subtype;
  String inorder;
  String dateorder;
  String dateplan;
  String dateexpire;
  Null slotdate;
  String slotno;
  int inpriority;
  String inpromo;
  String tflow;
  Null daterec;
  String thname;
  String remarkrec;
  int isreqmeasurement;
  String dateremarks;
  int opsprogress;

  SearchPayload(
      {this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.thcode,
      this.subtype,
      this.inorder,
      this.dateorder,
      this.dateplan,
      this.dateexpire,
      this.slotdate,
      this.slotno,
      this.inpriority,
      this.inpromo,
      this.tflow,
      this.daterec,
      this.thname,
      this.remarkrec,
      this.isreqmeasurement,
      this.dateremarks,
      this.opsprogress});

  SearchPayload.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    thcode = json['thcode'];
    subtype = json['subtype'];
    inorder = json['inorder'];
    dateorder = json['dateorder'];
    dateplan = json['dateplan'];
    dateexpire = json['dateexpire'];
    slotdate = json['slotdate'];
    slotno = json['slotno'];
    inpriority = json['inpriority'];
    inpromo = json['inpromo'];
    tflow = json['tflow'];
    daterec = json['daterec'];
    thname = json['thname'];
    remarkrec = json['remarkrec'];
    isreqmeasurement = json['isreqmeasurement'];
    dateremarks = json['dateremarks'];
    opsprogress = json['opsprogress'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['thcode'] = this.thcode;
    data['subtype'] = this.subtype;
    data['inorder'] = this.inorder;
    data['dateorder'] = this.dateorder;
    data['dateplan'] = this.dateplan;
    data['dateexpire'] = this.dateexpire;
    data['slotdate'] = this.slotdate;
    data['slotno'] = this.slotno;
    data['inpriority'] = this.inpriority ?? 0;
    data['inpromo'] = this.inpromo;
    data['tflow'] = this.tflow;
    data['daterec'] = this.daterec;
    data['thname'] = this.thname;
    data['remarkrec'] = this.remarkrec;
    data['isreqmeasurement'] = this.isreqmeasurement ?? 0;
    data['dateremarks'] = this.dateremarks;
    data['opsprogress'] = this.opsprogress ?? 0;
    return data;
  }
}
