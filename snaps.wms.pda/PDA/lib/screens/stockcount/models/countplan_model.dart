class Countplan {
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String countcode;
  String plancode;
  String planname;
  String accnassign;
  String accnwork;
  String szone;
  String ezone;
  String saisle;
  String eaisle;
  String sbay;
  String ebay;
  String slevel;
  String elevel;
  int isroaming;
  String tflow;
  int cntpercentage;
  int cnterror;
  int cntlines;
  String cnttime;
  DateTime datestart;
  int pctstart;
  String datevld;
  int pctvld;
  String accnvld;
  String devicevld;
  String remarksvld;
  String datecreate;
  String accncreate;
  String datemodify;
  String accnmodify;
  String procmodify;
  int isblock;
  int isdatemfg;
  int isdateexp;
  int isbatchno;
  int isserialno;
  int allowscanhu;
  String countType;
  String planorigin;

  Countplan(
      {this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.countcode,
      this.plancode,
      this.planname,
      this.accnassign,
      this.accnwork,
      this.szone,
      this.ezone,
      this.saisle,
      this.eaisle,
      this.sbay,
      this.ebay,
      this.slevel,
      this.elevel,
      this.isroaming,
      this.tflow,
      this.cntpercentage,
      this.cnterror,
      this.cntlines,
      this.cnttime,
      this.datestart,
      this.pctstart,
      this.datevld,
      this.pctvld,
      this.accnvld,
      this.devicevld,
      this.remarksvld,
      this.datecreate,
      this.accncreate,
      this.datemodify,
      this.accnmodify,
      this.procmodify,
      this.isblock,
      this.isdatemfg,
      this.isdateexp,
      this.isbatchno,
      this.isserialno,
      this.allowscanhu,
      this.planorigin});

  Countplan.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    countcode = json['countcode'];
    plancode = json['plancode'];
    planname = json['planname'];
    accnassign = json['accnassign'];
    accnwork = json['accnwork'];
    szone = json['szone'];
    ezone = json['ezone'];
    saisle = json['saisle'];
    eaisle = json['eaisle'];
    sbay = json['sbay'];
    ebay = json['ebay'];
    slevel = json['slevel'];
    elevel = json['elevel'];
    isroaming = json['isroaming'];
    tflow = json['tflow'];
    cntpercentage = json['cntpercentage'];
    cnterror = json['cnterror'];
    cntlines = json['cntlines'];
    cnttime = json['cnttime'];
    datestart =
        json['datestart'] == null ? null : DateTime.parse(json["datestart"]);
    pctstart = json['pctstart'];
    datevld = json['datevld'];
    pctvld = json['pctvld'];
    accnvld = json['accnvld'];
    devicevld = json['devicevld'];
    remarksvld = json['remarksvld'];
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    datemodify = json['datemodify'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    isblock = json['isblock'];
    isdatemfg = json['isdatemfg'];
    isdateexp = json['isdateexp'];
    isbatchno = json['isbatchno'];
    isserialno = json['isserialno'];
    allowscanhu = json['allowscanhu'];
    planorigin = json['planorigin'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['countcode'] = this.countcode;
    data['plancode'] = this.plancode;
    data['planname'] = this.planname;
    data['accnassign'] = this.accnassign;
    data['accnwork'] = this.accnwork;
    data['szone'] = this.szone;
    data['ezone'] = this.ezone;
    data['saisle'] = this.saisle;
    data['eaisle'] = this.eaisle;
    data['sbay'] = this.sbay;
    data['ebay'] = this.ebay;
    data['slevel'] = this.slevel;
    data['elevel'] = this.elevel;
    data['isroaming'] = this.isroaming;
    data['tflow'] = this.tflow;
    data['cntpercentage'] = this.cntpercentage;
    data['cnterror'] = this.cnterror;
    data['cntlines'] = this.cntlines;
    data['cnttime'] = this.cnttime;
    data['datestart'] =
        this.datestart != null ? datestart.toIso8601String() : datestart;
    data['pctstart'] = this.pctstart;
    data['datevld'] = this.datevld;
    data['pctvld'] = this.pctvld;
    data['accnvld'] = this.accnvld;
    data['devicevld'] = this.devicevld;
    data['remarksvld'] = this.remarksvld;
    data['datecreate'] = this.datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] = this.datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    data['isblock'] = this.isblock;
    data['isdatemfg'] = this.isdatemfg;
    data['isdateexp'] = this.isdateexp;
    data['isbatchno'] = this.isbatchno;
    data['isserialno'] = this.isserialno;
    data['allowscanhu'] = this.allowscanhu;
    data['planorigin'] = this.planorigin;
    return data;
  }
}
