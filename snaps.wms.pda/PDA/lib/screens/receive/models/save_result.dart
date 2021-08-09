class Inboulx {
  double inlx;
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String inorder;
  String inln;
  String inrefno;
  String inrefln;
  String barcode;
  String article;
  int pv;
  int lv;
  String unitops;
  int qtyskurec;
  int qtypurec;
  int qtyhurec;
  double qtyweightrec;
  double qtynaturalloss;
  DateTime daterec;
  DateTime datemfg;
  DateTime dateexp;
  String lotno;
  String batchno;
  String serialno;
  DateTime datecreate;
  String accncreate;
  DateTime datemodify;
  String accnmodify;
  String procmodify;
  String tflow;
  String ingrno;
  int rtohu;
  int rtopu;
  String dockno;
  String thcode;
  String intype;
  String insubtype;
  String inpromo;
  String orbitsource;
  double qtyvolumerec;
  String inagrn;
  int inseq;
  double skuweight;
  double skuvolume;
  double skucubic;
  String unitmanage;

  Inboulx(
      {this.inlx,
      this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.inorder,
      this.inln,
      this.inrefno,
      this.inrefln,
      this.barcode,
      this.article,
      this.pv,
      this.lv,
      this.unitops,
      this.qtyskurec,
      this.qtypurec,
      this.qtyhurec,
      this.qtyweightrec,
      this.qtynaturalloss,
      this.daterec,
      this.datemfg,
      this.dateexp,
      this.lotno,
      this.batchno,
      this.serialno,
      this.datecreate,
      this.accncreate,
      this.datemodify,
      this.accnmodify,
      this.procmodify,
      this.tflow,
      this.ingrno,
      this.rtohu,
      this.rtopu,
      this.dockno,
      this.thcode,
      this.intype,
      this.insubtype,
      this.inpromo,
      this.orbitsource,
      this.qtyvolumerec,
      this.inagrn,
      this.inseq,
      this.skuweight,
      this.skuvolume,
      this.skucubic,
      this.unitmanage});

  Inboulx.fromJson(Map<String, dynamic> json) {
    inlx = json['inlx'];
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    inorder = json['inorder'];
    inln = json['inln'];
    inrefno = json['inrefno'];
    inrefln = json['inrefln'];
    barcode = json['barcode'];
    article = json['article'];
    pv = json['pv'];
    lv = json['lv'];
    unitops = json['unitops'];
    qtyskurec = json['qtyskurec'];
    qtypurec = json['qtypurec'];
    qtyhurec = json['qtyhurec'];
    qtyweightrec = json['qtyweightrec'];
    qtynaturalloss = json['qtynaturalloss'];
    daterec = json['daterec'];
    datemfg = json['datemfg'];
    dateexp = json['dateexp'];
    lotno = json['lotno'];
    batchno = json['batchno'];
    serialno = json['serialno'];
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    datemodify = json['datemodify'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    tflow = json['tflow'];
    ingrno = json['ingrno'];
    rtohu = json['rtohu'];
    rtopu = json['rtopu'];
    dockno = json['dockno'];
    thcode = json['thcode'];
    intype = json['intype'];
    insubtype = json['insubtype'];
    inpromo = json['inpromo'];
    orbitsource = json['orbitsource'];
    qtyvolumerec = json['qtyvolumerec'];
    inagrn = json['inagrn'];
    inseq = json['inseq'];
    skuweight = json['skuweight'];
    skuvolume = json['skuvolume'];
    skucubic = json['skucubic'];
    unitmanage = json['unitmanage'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['inlx'] = this.inlx;
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['inorder'] = this.inorder;
    data['inln'] = this.inln;
    data['inrefno'] = this.inrefno;
    data['inrefln'] = this.inrefln;
    data['barcode'] = this.barcode;
    data['article'] = this.article;
    data['pv'] = this.pv;
    data['lv'] = this.lv;
    data['unitops'] = this.unitops;
    data['qtyskurec'] = this.qtyskurec;
    data['qtypurec'] = this.qtypurec;
    data['qtyhurec'] = this.qtyhurec;
    data['qtyweightrec'] = this.qtyweightrec;
    data['qtynaturalloss'] = this.qtynaturalloss;
    data['daterec'] =
        this.daterec != null ? daterec.toIso8601String() : daterec;
    data['datemfg'] =
        this.datemfg != null ? datemfg.toIso8601String() : datemfg;
    data['dateexp'] =
        this.dateexp != null ? dateexp.toIso8601String() : dateexp;
    data['lotno'] = this.lotno;
    data['batchno'] = this.batchno;
    data['serialno'] = this.serialno;
    data['datecreate'] = this.datecreate != null
        ? datecreate.toIso8601String()
        : this.datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] = this.datemodify != null
        ? datemodify.toIso8601String()
        : this.datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    data['tflow'] = this.tflow;
    data['ingrno'] = this.ingrno;
    data['rtohu'] = this.rtohu;
    data['rtopu'] = this.rtopu;
    data['dockno'] = this.dockno;
    data['thcode'] = this.thcode;
    data['intype'] = this.intype;
    data['insubtype'] = this.insubtype;
    data['inpromo'] = this.inpromo;
    data['orbitsource'] = this.orbitsource;
    data['qtyvolumerec'] = this.qtyvolumerec;
    data['inagrn'] = this.inagrn;
    data['inseq'] = this.inseq;
    data['skuweight'] = this.skuweight;
    data['skuvolume'] = this.skuvolume;
    data['skucubic'] = this.skucubic;
    data['unitmanage'] = this.unitmanage;
    return data;
  }
}
