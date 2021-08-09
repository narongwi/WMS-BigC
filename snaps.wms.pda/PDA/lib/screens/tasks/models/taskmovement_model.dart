class TaskMovement {
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String tasktype;
  String taskno;
  String iopromo;
  String iorefno;
  String priority;
  String taskdate;
  String tflow;
  String datemodify;
  dynamic datestart;
  dynamic dateend;
  dynamic datecreate;
  String accncreate;
  String accnmodify;
  String procmodify;
  String taskname;
  List<MovementLines> lines;
  String confirmdigit;
  String devid;
  String routeno;
  String routethcode;
  String opscode;
  String intype;

  TaskMovement(
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
      this.lines,
      this.confirmdigit,
      this.devid,
      this.routeno,
      this.routethcode,
      this.opscode,
      this.intype});

  TaskMovement.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    tasktype = json['tasktype'];
    taskno = json['taskno'];
    iopromo = json['iopromo'];
    iorefno = json['iorefno'];
    priority = json['priority'];
    taskdate = json['taskdate'];
    tflow = json['tflow'];
    datemodify = json['datemodify'];
    datestart = json['datestart'];
    dateend = json['dateend'];
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    taskname = json['taskname'];
    if (json['lines'] != null) {
      lines = <MovementLines>[];
      json['lines'].forEach((v) {
        lines.add(new MovementLines.fromJson(v));
      });
    }
    confirmdigit = json['confirmdigit'];
    devid = json['devid'];
    routeno = json['routeno'];
    routethcode = json['routethcode'];
    opscode = json['opscode'];
    intype = json['intype'];
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
    data['taskdate'] = this.taskdate;
    data['tflow'] = this.tflow;
    data['datemodify'] = this.datemodify;
    data['datestart'] = this.datestart;
    data['dateend'] = this.dateend;
    data['datecreate'] = this.datecreate;
    data['accncreate'] = this.accncreate;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    data['taskname'] = this.taskname;
    if (this.lines != null) {
      data['lines'] = this.lines.map((v) => v.toJson()).toList();
    }
    data['confirmdigit'] = this.confirmdigit;
    data['devid'] = this.devid;
    data['routeno'] = this.routeno;
    data['routethcode'] = this.routethcode;
    data['opscode'] = this.opscode;
    data['intype'] = this.intype;
    return data;
  }
}

class MovementLines {
  int pv;
  int lv;
  String targetloc;
  int targetqty;
  String collectloc;
  String collecthuno;
  int collectqty;
  String accnfill;
  String accncollect;
  dynamic dateassign;
  dynamic datework;
  dynamic datefill;
  dynamic datecollect;
  String lotno;
  DateTime datemfg;
  DateTime dateexp;
  String serialno;
  DateTime datecreate;
  String accncreate;
  String accnmodify;
  String procmodify;
  String descalt;
  double sourceqty;
  double sourcevolume;
  double sourceweight;
  double stockid;
  String ouorder;
  int ouln;
  String ourefno;
  String ourefln;
  String thcode;
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String taskno;
  int taskseq;
  String article;
  String sourceloc;
  String sourcehuno;
  String targetadv;
  String targethuno;
  String iopromo;
  String ioreftype;
  String iorefno;
  String datemodify;
  String tflow;
  String accnassign;
  String accnwork;
  String skipdigit;

  MovementLines(
      {this.pv,
      this.lv,
      this.targetloc,
      this.targetqty,
      this.collectloc,
      this.collecthuno,
      this.collectqty,
      this.accnfill,
      this.accncollect,
      this.dateassign,
      this.datework,
      this.datefill,
      this.datecollect,
      this.lotno,
      this.datemfg,
      this.dateexp,
      this.serialno,
      this.datecreate,
      this.accncreate,
      this.accnmodify,
      this.procmodify,
      this.descalt,
      this.sourceqty,
      this.sourcevolume,
      this.sourceweight,
      this.stockid,
      this.ouorder,
      this.ouln,
      this.ourefno,
      this.ourefln,
      this.thcode,
      this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.taskno,
      this.taskseq,
      this.article,
      this.sourceloc,
      this.sourcehuno,
      this.targetadv,
      this.targethuno,
      this.iopromo,
      this.ioreftype,
      this.iorefno,
      this.datemodify,
      this.tflow,
      this.accnassign,
      this.accnwork,
      this.skipdigit});

  MovementLines.fromJson(Map<String, dynamic> json) {
    pv = json['pv'];
    lv = json['lv'];
    targetloc = json['targetloc'];
    targetqty = json['targetqty'];
    collectloc = json['collectloc'];
    collecthuno = json['collecthuno'];
    collectqty = json['collectqty'];
    accnfill = json['accnfill'];
    accncollect = json['accncollect'];
    dateassign = json['dateassign'];
    datework = json['datework'];
    datefill = json['datefill'];
    datecollect = json['datecollect'];
    lotno = json['lotno'];
    datemfg = json['datemfg'] == null ? null : DateTime.parse(json["datemfg"]);
    dateexp = json["dateexp"] == null ? null : DateTime.parse(json["dateexp"]);
    serialno = json['serialno'];
    datecreate =
        json['datecreate'] == null ? null : DateTime.parse(json["datecreate"]);
    accncreate = json['accncreate'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    descalt = json['descalt'];
    sourceqty = json['sourceqty'];
    sourcevolume = json['sourcevolume'];
    sourceweight = json['sourceweight'];
    stockid = json['stockid'];
    ouorder = json['ouorder'];
    ouln = json['ouln'];
    ourefno = json['ourefno'];
    ourefln = json['ourefln'];
    thcode = json['thcode'];
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    taskno = json['taskno'];
    taskseq = json['taskseq'];
    article = json['article'];
    sourceloc = json['sourceloc'];
    sourcehuno = json['sourcehuno'];
    targetadv = json['targetadv'];
    targethuno = json['targethuno'];
    iopromo = json['iopromo'];
    ioreftype = json['ioreftype'];
    iorefno = json['iorefno'];
    datemodify = json['datemodify'];
    tflow = json['tflow'];
    accnassign = json['accnassign'];
    accnwork = json['accnwork'];
    skipdigit = json['skipdigit'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['pv'] = this.pv;
    data['lv'] = this.lv;
    data['targetloc'] = this.targetloc;
    data['targetqty'] = this.targetqty;
    data['collectloc'] = this.collectloc;
    data['collecthuno'] = this.collecthuno;
    data['collectqty'] = this.collectqty;
    data['accnfill'] = this.accnfill;
    data['accncollect'] = this.accncollect;
    data['dateassign'] = this.dateassign;
    data['datework'] = this.datework;
    data['datefill'] = this.datefill;
    data['datecollect'] = this.datecollect;
    data['lotno'] = this.lotno;
    data['datemfg'] =
        this.datemfg != null ? datemfg.toIso8601String() : datemfg;
    data['dateexp'] =
        this.dateexp != null ? dateexp.toIso8601String() : dateexp;
    data['serialno'] = this.serialno;
    data['datecreate'] =
        this.datecreate != null ? datecreate.toIso8601String() : datecreate;
    data['accncreate'] = this.accncreate;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    data['descalt'] = this.descalt;
    data['sourceqty'] = this.sourceqty;
    data['sourcevolume'] = this.sourcevolume;
    data['sourceweight'] = this.sourceweight;
    data['stockid'] = this.stockid;
    data['ouorder'] = this.ouorder;
    data['ouln'] = this.ouln;
    data['ourefno'] = this.ourefno;
    data['ourefln'] = this.ourefln;
    data['thcode'] = this.thcode;
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['taskno'] = this.taskno;
    data['taskseq'] = this.taskseq;
    data['article'] = this.article;
    data['sourceloc'] = this.sourceloc;
    data['sourcehuno'] = this.sourcehuno;
    data['targetadv'] = this.targetadv;
    data['targethuno'] = this.targethuno;
    data['iopromo'] = this.iopromo;
    data['ioreftype'] = this.ioreftype;
    data['iorefno'] = this.iorefno;
    data['datemodify'] = this.datemodify;
    data['tflow'] = this.tflow;
    data['accnassign'] = this.accnassign;
    data['accnwork'] = this.accnwork;
    data['skipdigit'] = this.skipdigit;
    return data;
  }
}
