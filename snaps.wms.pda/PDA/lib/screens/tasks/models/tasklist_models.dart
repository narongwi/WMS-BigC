class TaskList {
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String tasktype;
  String taskno;
  String iopromo;
  String iorefno;
  String priority;
  DateTime taskdate;
  String tflow;
  DateTime datemodify;
  DateTime datestart;
  DateTime dateend;
  DateTime datecreate;
  String accncreate;
  String accnmodify;
  String procmodify;
  String taskname;
  String article;
  int pv;
  int lv;
  String sourceloc;
  String sourcehuno;
  String targetadv;
  String descalt;
  String accnwork;
  DateTime dateremarks;

  TaskList(
      {this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.tasktype,
      this.taskno,
      this.iopromo,
      this.iorefno,
      this.priority,
      this.taskdate,
      this.tflow,
      this.datemodify,
      this.datestart,
      this.dateend,
      this.datecreate,
      this.accncreate,
      this.accnmodify,
      this.procmodify,
      this.taskname,
      this.article,
      this.pv,
      this.lv,
      this.sourceloc,
      this.sourcehuno,
      this.targetadv,
      this.descalt,
      this.accnwork,
      this.dateremarks});

  TaskList.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    tasktype = json['tasktype'];
    taskno = json['taskno'];
    iopromo = json['iopromo'];
    iorefno = json['iorefno'];
    priority = json['priority'];
    taskdate =
        json['taskdate'] == null ? null : DateTime.parse(json["taskdate"]);
    tflow = json['tflow'];
    datemodify =
        json['datemodify'] == null ? null : DateTime.parse(json["datemodify"]);
    datestart =
        json['datestart'] == null ? null : DateTime.parse(json["datestart"]);
    dateend = json['dateend'] == null ? null : DateTime.parse(json["dateend"]);
    datecreate =
        json['datecreate'] == null ? null : DateTime.parse(json["datecreate"]);
    accncreate = json['accncreate'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    taskname = json['taskname'];
    article = json['article'];
    pv = json['pv'];
    lv = json['lv'];
    sourceloc = json['sourceloc'];
    sourcehuno = json['sourcehuno'];
    targetadv = json['targetadv'];
    descalt = json['descalt'];
    accnwork = json['accnwork'];
    dateremarks = null;
    // dateremarks = json['dateremarks'] == null
    //     ? null
    //     : DateTime.parse(json["dateremarks"]);
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['tasktype'] = this.tasktype;
    data['taskno'] = this.taskno;
    data['iopromo'] = this.iopromo;
    data['iorefno'] = this.iorefno;
    data['priority'] = this.priority;
    data['taskdate'] =
        this.taskdate != null ? taskdate.toIso8601String() : taskdate;
    data['tflow'] = this.tflow;
    data['datemodify'] =
        this.datemodify != null ? datemodify.toIso8601String() : datemodify;
    data['datestart'] =
        this.datestart != null ? datestart.toIso8601String() : datestart;
    data['dateend'] =
        this.dateend != null ? dateend.toIso8601String() : dateend;
    data['datecreate'] =
        this.datecreate != null ? datecreate.toIso8601String() : datecreate;
    data['accncreate'] = this.accncreate;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    data['taskname'] = this.taskname;
    data['article'] = this.article;
    data['pv'] = this.pv;
    data['lv'] = this.lv;
    data['sourceloc'] = this.sourceloc;
    data['sourcehuno'] = this.sourcehuno;
    data['targetadv'] = this.targetadv;
    data['descalt'] = this.descalt;
    data['accnwork'] = this.accnwork;
    data['dateremarks'] =
        this.dateremarks != null ? dateremarks.toIso8601String() : dateremarks;
    return data;
  }
}
