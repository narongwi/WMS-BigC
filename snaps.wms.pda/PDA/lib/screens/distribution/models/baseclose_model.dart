class BaseCloseFilter {
  String routeno;
  String huno;
  String thcode;
  String tflow;
  String spcarea;
  String hutype;

  BaseCloseFilter(
      {this.routeno,
      this.huno,
      this.thcode,
      this.tflow,
      this.spcarea,
      this.hutype});

  BaseCloseFilter.fromJson(Map<String, dynamic> json) {
    routeno = json['routeno'];
    huno = json['huno'];
    thcode = json['thcode'];
    tflow = json['tflow'];
    spcarea = json['spcarea'];
    hutype = json['hutype'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['routeno'] = this.routeno;
    data['huno'] = this.huno;
    data['thcode'] = this.thcode;
    data['tflow'] = this.tflow;
    data['spcarea'] = this.spcarea;
    data['hutype'] = this.hutype;
    return data;
  }
}

class BaseClose {
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String hutype;
  String huno;
  String loccode;
  String thcode;
  String routeno;
  double mxsku;
  double mxweight;
  double mxvolume;
  int crsku;
  double crweight;
  double crvolume;
  double crcapacity;
  String tflow;
  DateTime datecreate;
  String accncreate;
  DateTime datemodify;
  String accnmodify;
  String procmodfiy;
  int priority;
  String promo;
  String thname;
  String przone;

  BaseClose(
      {this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.hutype,
      this.huno,
      this.loccode,
      this.thcode,
      this.routeno,
      this.mxsku,
      this.mxweight,
      this.mxvolume,
      this.crsku,
      this.crweight,
      this.crvolume,
      this.crcapacity,
      this.tflow,
      this.datecreate,
      this.accncreate,
      this.datemodify,
      this.accnmodify,
      this.procmodfiy,
      this.priority,
      this.promo,
      this.thname,
      this.przone});

  BaseClose.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    hutype = json['hutype'];
    huno = json['huno'];
    loccode = json['loccode'];
    thcode = json['thcode'];
    routeno = json['routeno'];
    mxsku = json['mxsku'];
    mxweight = json['mxweight'];
    mxvolume = json['mxvolume'];
    crsku = json['crsku'];
    crweight = json['crweight'];
    crvolume = json['crvolume'];
    crcapacity = json['crcapacity'];
    tflow = json['tflow'];
    datecreate =
        json['datecreate'] == null ? null : DateTime.parse(json["datecreate"]);
    accncreate = json['accncreate'];
    datemodify =
        json['datemodify'] == null ? null : DateTime.parse(json["datemodify"]);
    accnmodify = json['accnmodify'];
    procmodfiy = json['procmodfiy'];
    priority = json['priority'];
    promo = json['promo'];
    thname = json['thname'];
    przone = json['przone'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['hutype'] = this.hutype;
    data['huno'] = this.huno;
    data['loccode'] = this.loccode;
    data['thcode'] = this.thcode;
    data['routeno'] = this.routeno;
    data['mxsku'] = this.mxsku;
    data['mxweight'] = this.mxweight;
    data['mxvolume'] = this.mxvolume;
    data['crsku'] = this.crsku;
    data['crweight'] = this.crweight;
    data['crvolume'] = this.crvolume;
    data['crcapacity'] = this.crcapacity;
    data['tflow'] = this.tflow;
    data['datecreate'] =
        this.datecreate != null ? datecreate.toIso8601String() : datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] =
        this.datemodify != null ? datemodify.toIso8601String() : datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodfiy'] = this.procmodfiy;
    data['priority'] = this.priority;
    data['promo'] = this.promo;
    data['thname'] = this.thname;
    data['przone'] = this.przone;
    return data;
  }
}

class BaseCloseLine {
  String prepno;
  String inorder;
  String ouorder;
  String loccode;
  String article;
  int pv;
  int lv;
  int qtysku;
  double qtypu;
  double qtyweight;
  double qtyvolume;
  String descalt;
  String batchno;
  String lotno;
  DateTime datemfg;
  DateTime dateexp;
  String serialno;
  String unitprep;

  BaseCloseLine(
      {this.prepno,
      this.inorder,
      this.ouorder,
      this.loccode,
      this.article,
      this.pv,
      this.lv,
      this.qtysku,
      this.qtypu,
      this.qtyweight,
      this.qtyvolume,
      this.descalt,
      this.batchno,
      this.lotno,
      this.datemfg,
      this.dateexp,
      this.serialno,
      this.unitprep});

  BaseCloseLine.fromJson(Map<String, dynamic> json) {
    prepno = json['prepno'];
    inorder = json['inorder'];
    ouorder = json['ouorder'];
    loccode = json['loccode'];
    article = json['article'];
    pv = json['pv'];
    lv = json['lv'];
    qtysku = json['qtysku'];
    qtypu = json['qtypu'];
    qtyweight = json['qtyweight'];
    qtyvolume = json['qtyvolume'];
    descalt = json['descalt'];
    batchno = json['batchno'];
    lotno = json['lotno'];
    datemfg = json['datemfg'] == null ? null : DateTime.parse(json["datemfg"]);
    dateexp = json['dateexp'] == null ? null : DateTime.parse(json["dateexp"]);
    serialno = json['serialno'];
    unitprep = json['unitprep'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['prepno'] = this.prepno;
    data['inorder'] = this.inorder;
    data['ouorder'] = this.ouorder;
    data['loccode'] = this.loccode;
    data['article'] = this.article;
    data['pv'] = this.pv;
    data['lv'] = this.lv;
    data['qtysku'] = this.qtysku;
    data['qtypu'] = this.qtypu;
    data['qtyweight'] = this.qtyweight;
    data['qtyvolume'] = this.qtyvolume;
    data['descalt'] = this.descalt;
    data['batchno'] = this.batchno;
    data['lotno'] = this.lotno;
    data['datemfg'] =
        this.datemfg != null ? datemfg.toIso8601String() : datemfg;
    data['dateexp'] =
        this.dateexp != null ? dateexp.toIso8601String() : dateexp;
    data['serialno'] = this.serialno;
    data['unitprep'] = this.unitprep;
    return data;
  }
}
