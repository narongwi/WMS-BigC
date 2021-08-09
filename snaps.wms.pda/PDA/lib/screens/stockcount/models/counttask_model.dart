class CountTask {
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String counttype;
  String countcode;
  String countname;
  DateTime datestart;
  DateTime dateend;
  int isblock;
  String remarks;
  String tflow;
  String datecreate;
  String accncreate;
  String datemodify;
  String accnmodify;
  String procmodify;

  CountTask(
      {this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.counttype,
      this.countcode,
      this.countname,
      this.datestart,
      this.dateend,
      this.isblock,
      this.remarks,
      this.tflow,
      this.datecreate,
      this.accncreate,
      this.datemodify,
      this.accnmodify,
      this.procmodify});

  CountTask.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    counttype = json['counttype'];
    countcode = json['countcode'];
    countname = json['countname'];
    datestart =
        json['datestart'] == null ? null : DateTime.parse(json["datestart"]);
    dateend = json['dateend'] == null ? null : DateTime.parse(json["dateend"]);
    isblock = json['isblock'];
    remarks = json['remarks'];
    tflow = json['tflow'];
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    datemodify = json['datemodify'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['counttype'] = this.counttype;
    data['countcode'] = this.countcode;
    data['countname'] = this.countname;
    data['datestart'] =
        this.datestart != null ? datestart.toIso8601String() : datestart;
    data['dateend'] =
        this.dateend != null ? dateend.toIso8601String() : dateend;
    data['isblock'] = this.isblock;
    data['remarks'] = this.remarks;
    data['tflow'] = this.tflow;
    data['datecreate'] = this.datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] = this.datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    return data;
  }
}
