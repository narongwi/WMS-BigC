class Countline {
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String countcode;
  String plancode;
  String loccode;
  int locseq;
  String unitcount;
  String stbarcode;
  String starticle;
  int stpv;
  int stlv;
  int stqtysku;
  int stqtypu;
  String stlotmfg;
  DateTime stdatemfg;
  DateTime stdateexp;
  String stserialno;
  String sthuno;
  String cnbarcode;
  String cnarticle;
  int cnpv;
  int cnlv;
  int cnqtysku;
  int cnqtypu;
  String cnlotmfg;
  DateTime cndatemfg;
  DateTime cndateexp;
  String cnserialno;
  String cnhuno;
  String cnflow;
  String cnmsg;
  int isskip;
  int isrgen;
  int iswrgln;
  String countdevice;
  String countdate;
  String corcode;
  int corqty;
  String coraccn;
  String cordevice;
  dynamic cordate;
  String tflow;
  String datecreate;
  String accncreate;
  String datemodify;
  String accnmodify;
  String procmodify;
  String productdesc;
  String locctype;
  Countline(
      {this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.countcode,
      this.plancode,
      this.loccode,
      this.locseq,
      this.unitcount,
      this.stbarcode,
      this.starticle,
      this.stpv,
      this.stlv,
      this.stqtysku,
      this.stqtypu,
      this.stlotmfg,
      this.stdatemfg,
      this.stdateexp,
      this.stserialno,
      this.sthuno,
      this.cnbarcode,
      this.cnarticle,
      this.cnpv,
      this.cnlv,
      this.cnqtysku,
      this.cnqtypu,
      this.cnlotmfg,
      this.cndatemfg,
      this.cndateexp,
      this.cnserialno,
      this.cnhuno,
      this.cnflow,
      this.cnmsg,
      this.isskip,
      this.isrgen,
      this.iswrgln,
      this.countdevice,
      this.countdate,
      this.corcode,
      this.corqty,
      this.coraccn,
      this.cordevice,
      this.cordate,
      this.tflow,
      this.datecreate,
      this.accncreate,
      this.datemodify,
      this.accnmodify,
      this.procmodify,
      this.productdesc,
      this.locctype});

  Countline.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    countcode = json['countcode'];
    plancode = json['plancode'];
    loccode = json['loccode'];
    locseq = json['locseq'];
    unitcount = json['unitcount'];
    stbarcode = json['stbarcode'];
    starticle = json['starticle'];
    stpv = json['stpv'];
    stlv = json['stlv'];
    stqtysku = json['stqtysku'];
    stqtypu = json['stqtypu'];
    stlotmfg = json['stlotmfg'];
    stdatemfg =
        json['stdatemfg'] == null ? null : DateTime.parse(json["stdatemfg"]);
    stdateexp =
        json['stdateexp'] == null ? null : DateTime.parse(json["stdateexp"]);
    stserialno = json['stserialno'];
    sthuno = json['sthuno'];
    cnbarcode = json['cnbarcode'];
    cnarticle = json['cnarticle'];
    cnpv = json['cnpv'];
    cnlv = json['cnlv'];
    cnqtysku = json['cnqtysku'];
    cnqtypu = json['cnqtypu'];
    cnlotmfg = json['cnlotmfg'];
    cndatemfg =
        json['cndatemfg'] == null ? null : DateTime.parse(json["cndatemfg"]);
    cndateexp =
        json['cndateexp'] == null ? null : DateTime.parse(json["cndateexp"]);
    cnserialno = json['cnserialno'];
    cnhuno = json['cnhuno'];
    cnflow = json['cnflow'];
    cnmsg = json['cnmsg'];
    isskip = json['isskip'];
    isrgen = json['isrgen'];
    iswrgln = json['iswrgln'];
    countdevice = json['countdevice'];
    countdate = json['countdate'];
    corcode = json['corcode'];
    corqty = json['corqty'];
    coraccn = json['coraccn'];
    cordevice = json['cordevice'];
    cordate = json['cordate'];
    tflow = json['tflow'];
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    datemodify = json['datemodify'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    productdesc = json['productdesc'];
    locctype = json['locctype'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['countcode'] = this.countcode;
    data['plancode'] = this.plancode;
    data['loccode'] = this.loccode;
    data['locseq'] = this.locseq;
    data['unitcount'] = this.unitcount;
    data['stbarcode'] = this.stbarcode;
    data['starticle'] = this.starticle;
    data['stpv'] = this.stpv;
    data['stlv'] = this.stlv;
    data['stqtysku'] = this.stqtysku;
    data['stqtypu'] = this.stqtypu;
    data['stlotmfg'] = this.stlotmfg;
    data['stdatemfg'] =
        this.stdatemfg != null ? stdatemfg.toIso8601String() : stdatemfg;
    data['stdateexp'] =
        this.stdateexp != null ? stdateexp.toIso8601String() : stdateexp;
    data['stserialno'] = this.stserialno;
    data['sthuno'] = this.sthuno;
    data['cnbarcode'] = this.cnbarcode;
    data['cnarticle'] = this.cnarticle;
    data['cnpv'] = this.cnpv;
    data['cnlv'] = this.cnlv;
    data['cnqtysku'] = this.cnqtysku;
    data['cnqtypu'] = this.cnqtypu;
    data['cnlotmfg'] = this.cnlotmfg;
    data['cndatemfg'] =
        this.cndatemfg != null ? cndatemfg.toIso8601String() : cndatemfg;
    data['cndateexp'] =
        this.cndateexp != null ? cndateexp.toIso8601String() : cndateexp;
    data['cnserialno'] = this.cnserialno;
    data['cnhuno'] = this.cnhuno;
    data['cnflow'] = this.cnflow;
    data['cnmsg'] = this.cnmsg;
    data['isskip'] = this.isskip;
    data['isrgen'] = this.isrgen;
    data['iswrgln'] = this.iswrgln;
    data['countdevice'] = this.countdevice;
    data['countdate'] = this.countdate;
    data['corcode'] = this.corcode;
    data['corqty'] = this.corqty;
    data['coraccn'] = this.coraccn;
    data['cordevice'] = this.cordevice;
    data['cordate'] = this.cordate;
    data['tflow'] = this.tflow;
    data['datecreate'] = this.datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] = this.datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    data['productdesc'] = this.productdesc;
    data['locctype'] = this.locctype;
    return data;
  }
}
