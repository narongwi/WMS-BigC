class SavePlayload {
  int lnix;
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String inorder;
  String inln;
  String inrefno;
  int inrefln;
  String barcode;
  String article;
  int pv;
  int lv;
  String unitops;
  int qtyskurec;
  int qtypurec;
  double qtyweightrec;
  double qtynaturalloss;
  DateTime daterec;
  DateTime datemfg;
  DateTime dateexp;
  String batchno;
  String lotno;
  String serialno;
  DateTime datecreate;
  String accncreate;
  DateTime datemodify;
  String accnmodify;
  String procmodify;
  String inagrn;
  int inseq;
  int qtyhurec;
  SavePlayload(
      {this.lnix,
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
      this.qtyweightrec,
      this.qtynaturalloss,
      this.daterec,
      this.datemfg,
      this.dateexp,
      this.batchno,
      this.lotno,
      this.serialno,
      this.datecreate,
      this.accncreate,
      this.datemodify,
      this.accnmodify,
      this.procmodify,
      this.inagrn,
      this.inseq,
      this.qtyhurec});

  SavePlayload.fromJson(Map<String, dynamic> json) {
    lnix = json['lnix'];
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
    qtyweightrec = json['qtyweightrec'];
    qtynaturalloss = json['qtynaturalloss'];
    daterec = json['daterec'];
    datemfg = json['datemfg'];
    dateexp = json['dateexp'];
    batchno = json['batchno'];
    lotno = json['lotno'];
    serialno = json['serialno'];
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    datemodify = json['datemodify'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    inagrn = json['inagrn'];
    inseq = json['inseq'];
    qtyhurec = json['qtyhurec'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['lnix'] = this.lnix;
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
    data['qtyweightrec'] = this.qtyweightrec;
    data['qtynaturalloss'] = this.qtynaturalloss;
    data['daterec'] =
        this.daterec != null ? daterec.toIso8601String() : daterec;
    data['datemfg'] =
        this.datemfg != null ? datemfg.toIso8601String() : datemfg;
    data['dateexp'] =
        this.dateexp != null ? dateexp.toIso8601String() : dateexp;
    data['batchno'] = this.batchno;
    data['lotno'] = this.lotno;
    data['serialno'] = this.serialno;
    data['datecreate'] =
        this.datecreate != null ? datecreate.toIso8601String() : datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] =
        this.datemodify != null ? datemodify.toIso8601String() : datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    data['inagrn'] = this.inagrn;
    data['inseq'] = this.inseq;
    data['qtyhurec'] = this.qtyhurec;
    return data;
  }
}
