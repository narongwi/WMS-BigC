class DistributeFilter {
  String spcarea;
  String routeno;
  String huno;
  String preptype;
  String prepno;
  DateTime prepdate;
  String thcode;
  String spcorder;
  String spcarticle;
  DateTime dateassign;
  String tflow;
  String deviceID;

  DistributeFilter({this.spcarea, this.routeno, this.huno, this.preptype, this.prepno, this.prepdate, this.thcode, this.spcorder, this.spcarticle, this.dateassign, this.tflow, this.deviceID});

  DistributeFilter.fromJson(Map<String, dynamic> json) {
    spcarea = json['spcarea'];
    routeno = json['routeno'];
    huno = json['huno'];
    preptype = json['preptype'];
    prepno = json['prepno'];
    prepdate = json['prepdate'] == null ? null : DateTime.parse(json["prepdate"]);
    thcode = json['thcode'];
    spcorder = json['spcorder'];
    spcarticle = json['spcarticle'];
    dateassign = json['dateassign'] == null ? null : DateTime.parse(json["dateassign"]);
    tflow = json['tflow'];
    deviceID = json['deviceID'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['spcarea'] = this.spcarea;
    data['routeno'] = this.routeno;
    data['huno'] = this.huno;
    data['preptype'] = this.preptype;
    data['prepno'] = this.prepno;
    data['prepdate'] = this.prepdate != null ? prepdate.toIso8601String() : prepdate;
    data['thcode'] = this.thcode;
    data['spcorder'] = this.spcorder;
    data['spcarticle'] = this.spcarticle;
    data['dateassign'] = this.dateassign != null ? prepdate.toIso8601String() : prepdate;
    data['tflow'] = this.tflow;
    data['deviceID'] = this.deviceID;
    return data;
  }
}

class Distribution {
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String routeno;
  String huno;
  String preptype;
  String prepno;
  DateTime prepdate;
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

  Distribution({this.orgcode, this.site, this.depot, this.spcarea, this.routeno, this.huno, this.preptype, this.prepno, this.prepdate, this.priority, this.thcode, this.spcorder, this.spcarticle, this.tflow, this.capacity, this.thname, this.picker, this.preppct, this.preptypename, this.przone});

  Distribution.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    routeno = json['routeno'];
    huno = json['huno'];
    preptype = json['preptype'];
    prepno = json['prepno'];
    prepdate = json['prepdate'] == null ? null : DateTime.parse(json["prepdate"]);
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
    data['prepdate'] = this.prepdate != null ? prepdate.toIso8601String() : prepdate;
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

class DistrbInfo {
  DateTime dateassign;
  DateTime datestart;
  DateTime dateend;
  String deviceID;
  DateTime datecreate;
  String accncreate;
  DateTime datemodify;
  String accnmodify;
  String procmodify;
  List<DistrbLine> lines;
  String setno;
  String hutype;
  double mxweight;
  double crweight;
  double mxvolume;
  double crvolume;
  int mxsku;
  int crsku;
  String loccode;
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String routeno;
  String huno;
  String preptype;
  String prepno;
  DateTime prepdate;
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

  DistrbInfo(
      {this.dateassign,
      this.datestart,
      this.dateend,
      this.deviceID,
      this.datecreate,
      this.accncreate,
      this.datemodify,
      this.accnmodify,
      this.procmodify,
      this.lines,
      this.setno,
      this.hutype,
      this.mxweight,
      this.crweight,
      this.mxvolume,
      this.crvolume,
      this.mxsku,
      this.crsku,
      this.loccode,
      this.orgcode,
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

  DistrbInfo.fromJson(Map<String, dynamic> json) {
    dateassign = json['dateassign'] == null ? null : DateTime.parse(json["dateassign"]);
    datestart = json['datestart'] == null ? null : DateTime.parse(json["datestart"]);
    dateend = json['dateend'] == null ? null : DateTime.parse(json["dateend"]);
    deviceID = json['deviceID'];
    datecreate = json['datecreate'] == null ? null : DateTime.parse(json["datecreate"]);
    accncreate = json['accncreate'];
    datemodify = json['datemodify'] == null ? null : DateTime.parse(json["datemodify"]);
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    if (json['lines'] != null) {
      lines = <DistrbLine>[];
      json['lines'].forEach((v) {
        lines.add(new DistrbLine.fromJson(v));
      });
    }
    setno = json['setno'];
    hutype = json['hutype'];
    mxweight = json['mxweight'];
    crweight = json['crweight'];
    mxvolume = json['mxvolume'];
    crvolume = json['crvolume'];
    mxsku = json['mxsku'];
    crsku = json['crsku'];
    loccode = json['loccode'];
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    routeno = json['routeno'];
    huno = json['huno'];
    preptype = json['preptype'];
    prepno = json['prepno'];
    prepdate = json['prepdate'] == null ? null : DateTime.parse(json["prepdate"]);
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
    data['dateassign'] = this.dateassign != null ? dateassign.toIso8601String() : dateassign;
    data['datestart'] = this.datestart != null ? datestart.toIso8601String() : datestart;
    data['dateend'] = this.dateend != null ? dateend.toIso8601String() : dateend;
    data['deviceID'] = this.deviceID;
    data['datecreate'] = this.datecreate != null ? datecreate.toIso8601String() : datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] = this.datemodify != null ? datemodify.toIso8601String() : datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    if (this.lines != null) {
      data['lines'] = this.lines.map((v) => v.toJson()).toList();
    }
    data['setno'] = this.setno;
    data['hutype'] = this.hutype;
    data['mxweight'] = this.mxweight;
    data['crweight'] = this.crweight;
    data['mxvolume'] = this.mxvolume;
    data['crvolume'] = this.crvolume;
    data['mxsku'] = this.mxsku;
    data['crsku'] = this.crsku;
    data['loccode'] = this.loccode;
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['routeno'] = this.routeno;
    data['huno'] = this.huno;
    data['preptype'] = this.preptype;
    data['prepno'] = this.prepno;
    data['prepdate'] = this.prepdate != null ? prepdate.toIso8601String() : prepdate;
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

class DistrbLine {
  String huno;
  String hunosource;
  String loczone;
  String loccode;
  int locseq;
  String locdigit;
  String ouorder;
  String ouln;
  String inorder;
  int inln;
  String barcode;
  int pv;
  int lv;
  double stockid;
  double qtyweightorder;
  double qtyvolumeorder;
  double qtyweightops;
  double qtyvolumeops;
  String batchno;
  String lotno;
  DateTime datemfg;
  DateTime dateexp;
  String serialno;
  String picker;
  DateTime datepick;
  String devicecode;
  DateTime datecreate;
  String accncreate;
  String accnmodify;
  String procmodify;
  int rtoskuofpu;
  String thcode;
  String taskno;
  DateTime daterec;
  String inagrn;
  String ingrno;
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String article;
  String description;
  int qtyskuorder;
  int qtypuorder;
  int qtypuops;
  String tflow;
  DateTime datemodify;
  String unitprep;
  String unitname;
  int qtyskuops;
  String prepno;
  int prepln;

  DistrbLine(
      {this.huno,
      this.hunosource,
      this.loczone,
      this.loccode,
      this.locseq,
      this.locdigit,
      this.ouorder,
      this.ouln,
      this.inorder,
      this.inln,
      this.barcode,
      this.pv,
      this.lv,
      this.stockid,
      this.qtyweightorder,
      this.qtyvolumeorder,
      this.qtyweightops,
      this.qtyvolumeops,
      this.batchno,
      this.lotno,
      this.datemfg,
      this.dateexp,
      this.serialno,
      this.picker,
      this.datepick,
      this.devicecode,
      this.datecreate,
      this.accncreate,
      this.accnmodify,
      this.procmodify,
      this.rtoskuofpu,
      this.thcode,
      this.taskno,
      this.daterec,
      this.inagrn,
      this.ingrno,
      this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.article,
      this.description,
      this.qtyskuorder,
      this.qtypuorder,
      this.qtypuops,
      this.tflow,
      this.datemodify,
      this.unitprep,
      this.unitname,
      this.qtyskuops,
      this.prepno,
      this.prepln});

  DistrbLine.fromJson(Map<String, dynamic> json) {
    huno = json['huno'];
    hunosource = json['hunosource'];
    loczone = json['loczone'];
    loccode = json['loccode'];
    locseq = json['locseq'];
    locdigit = json['locdigit'];
    ouorder = json['ouorder'];
    ouln = json['ouln'];
    inorder = json['inorder'];
    inln = json['inln'];
    barcode = json['barcode'];
    pv = json['pv'];
    lv = json['lv'];
    stockid = json['stockid'];
    qtyweightorder = json['qtyweightorder'];
    qtyvolumeorder = json['qtyvolumeorder'];
    qtyweightops = json['qtyweightops'];
    qtyvolumeops = json['qtyvolumeops'];
    batchno = json['batchno'];
    lotno = json['lotno'];
    datemfg = json['datemfg'] == null ? null : DateTime.parse(json["datemfg"]);
    dateexp = json['dateexp'] == null ? null : DateTime.parse(json["dateexp"]);
    serialno = json['serialno'];
    picker = json['picker'];
    datepick = json['datepick'];
    devicecode = json['devicecode'];
    datecreate = json['datecreate'] == null ? null : DateTime.parse(json["datecreate"]);
    accncreate = json['accncreate'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    rtoskuofpu = json['rtoskuofpu'];
    thcode = json['thcode'];
    taskno = json['taskno'];
    daterec = json['daterec'] == null ? null : DateTime.parse(json["daterec"]);
    inagrn = json['inagrn'];
    ingrno = json['ingrno'];
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    article = json['article'];
    description = json['description'];
    qtyskuorder = json['qtyskuorder'];
    qtypuorder = json['qtypuorder'];
    qtypuops = json['qtypuops'];
    tflow = json['tflow'];
    datemodify = json['datemodify'] == null ? null : DateTime.parse(json["datemodify"]);
    unitprep = json['unitprep'];
    unitname = json['unitname'];
    qtyskuops = json['qtyskuops'];
    prepno = json['prepno'];
    prepln = json['prepln'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['huno'] = this.huno;
    data['hunosource'] = this.hunosource;
    data['loczone'] = this.loczone;
    data['loccode'] = this.loccode;
    data['locseq'] = this.locseq;
    data['locdigit'] = this.locdigit;
    data['ouorder'] = this.ouorder;
    data['ouln'] = this.ouln;
    data['inorder'] = this.inorder;
    data['inln'] = this.inln;
    data['barcode'] = this.barcode;
    data['pv'] = this.pv;
    data['lv'] = this.lv;
    data['stockid'] = this.stockid;
    data['qtyweightorder'] = this.qtyweightorder;
    data['qtyvolumeorder'] = this.qtyvolumeorder;
    data['qtyweightops'] = this.qtyweightops;
    data['qtyvolumeops'] = this.qtyvolumeops;
    data['batchno'] = this.batchno;
    data['lotno'] = this.lotno;
    data['datemfg'] = this.datemfg != null ? datemfg.toIso8601String() : datemfg;
    data['dateexp'] = this.dateexp != null ? dateexp.toIso8601String() : dateexp;
    data['serialno'] = this.serialno;
    data['picker'] = this.picker;
    data['datepick'] = this.datepick != null ? datepick.toIso8601String() : datepick;
    data['devicecode'] = this.devicecode;
    data['datecreate'] = this.datecreate != null ? datecreate.toIso8601String() : datecreate;
    data['accncreate'] = this.accncreate;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    data['rtoskuofpu'] = this.rtoskuofpu;
    data['thcode'] = this.thcode;
    data['taskno'] = this.taskno;
    data['daterec'] = this.daterec != null ? daterec.toIso8601String() : daterec;
    data['inagrn'] = this.inagrn;
    data['ingrno'] = this.ingrno;
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['article'] = this.article;
    data['description'] = this.description;
    data['qtyskuorder'] = this.qtyskuorder;
    data['qtypuorder'] = this.qtypuorder;
    data['qtypuops'] = this.qtypuops;
    data['tflow'] = this.tflow;
    data['datemodify'] = this.datemodify != null ? datemodify.toIso8601String() : datemodify;
    data['unitprep'] = this.unitprep;
    data['unitname'] = this.unitname;
    data['qtyskuops'] = this.qtyskuops;
    data['prepno'] = this.prepno;
    data['prepln'] = this.prepln;
    return data;
  }
}
