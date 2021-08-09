class Empty {
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
  String datecreate;
  String accncreate;
  String datemodify;
  String accnmodify;
  String procmodfiy;
  int priority;
  String promo;
  String thname;
  String przone;

  Empty(
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

  Empty.fromJson(Map<String, dynamic> json) {
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
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    datemodify = json['datemodify'];
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
    data['datecreate'] = this.datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] = this.datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodfiy'] = this.procmodfiy;
    data['priority'] = this.priority;
    data['promo'] = this.promo;
    data['thname'] = this.thname;
    data['przone'] = this.przone;
    return data;
  }
}

class EmptyFilter {
  String loccode;
  String hutype;
  String tflow;
  String huno;

  EmptyFilter({this.loccode, this.huno, this.hutype, this.tflow});

  EmptyFilter.fromJson(Map<String, dynamic> json) {
    loccode = json['loccode'];
    hutype = json['hutype'];
    tflow = json['tflow'];
    huno = json['huno'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['loccode'] = this.loccode;
    data['hutype'] = this.hutype;
    data['tflow'] = this.tflow;
    data['huno'] = this.huno;
    return data;
  }
}

class EmptyGen {
  String thcode;
  int quantity;
  String hutype;
  String spcarea;
  int crsku;
  double crvolume;
  double crweight;
  String huno;
  int mxsku;
  int mxweight;
  String loccode;
  int priority;
  String tflow;
  String routeno;

  EmptyGen(
      {this.thcode,
      this.quantity,
      this.hutype,
      this.spcarea,
      this.crsku,
      this.crvolume,
      this.crweight,
      this.huno,
      this.loccode,
      this.priority,
      this.tflow,
      this.routeno,
      this.mxsku,
      this.mxweight});

  EmptyGen.fromJson(Map<String, dynamic> json) {
    thcode = json['thcode'];
    quantity = json['quantity'];
    hutype = json['hutype'];
    spcarea = json['spcarea'];
    crsku = json['crsku'];
    crvolume = json['crvolume'];
    crweight = json['crweight'];
    huno = json['huno'];
    loccode = json['loccode'];
    priority = json['priority'];
    tflow = json['tflow'];
    routeno = json['routeno'];
    mxsku = json['mxsku'];
    mxweight = json['mxweight'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['thcode'] = this.thcode;
    data['quantity'] = this.quantity;
    data['hutype'] = this.hutype;
    data['spcarea'] = this.spcarea;
    data['crsku'] = this.crsku;
    data['crvolume'] = this.crvolume;
    data['crweight'] = this.crweight;
    data['huno'] = this.huno;
    data['loccode'] = this.loccode;
    data['priority'] = this.priority;
    data['tflow'] = this.tflow;
    data['routeno'] = this.routeno;
    data['mxsku'] = this.mxsku;
    data['mxweight'] = this.mxweight;
    return data;
  }
}
