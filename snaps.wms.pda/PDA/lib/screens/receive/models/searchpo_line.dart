class SearchPOLines {
  String inrefno;
  int inrefln;
  String inagrn;
  String unitops;
  String lotno;
  dynamic expdate;
  String serialno;
  double qtypnd;
  int qtyskurec;
  int qtypurec;
  int qtyhurec;
  double qtyweightrec;
  double qtynaturalloss;
  String datecreate;
  String accncreate;
  String datemodify;
  String accnmodify;
  String procmodify;
  int isdlc;
  int isunique;
  int ismixingprep;
  int isbatchno;
  int dlcall;
  int dlcfactory;
  int dlcwarehouse;
  String unitreceipt;
  double innaturalloss;
  double skulength;
  double skuwidth;
  double skuheight;
  double skuweight;
  String tihi;
  String laslotno;
  dynamic lasdatemfg;
  dynamic lasdateexp;
  String lasserialno;
  int rtoskuofipck;
  int rtoskuofpck;
  int rtoskuoflayer;
  int rtoskuofhu;
  double huestimate;
  dynamic details;
  int inseq;
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String inorder;
  String inln;
  String barcode;
  String article;
  int pv;
  int lv;
  int qtysku;
  double qtypu;
  double qtyweight;
  String tflow;
  String description;
  String unitopsdesc;

  SearchPOLines(
      {this.inrefno,
      this.inrefln,
      this.inagrn,
      this.unitops,
      this.lotno,
      this.expdate,
      this.serialno,
      this.qtypnd,
      this.qtyskurec,
      this.qtypurec,
      this.qtyhurec,
      this.qtyweightrec,
      this.qtynaturalloss,
      this.datecreate,
      this.accncreate,
      this.datemodify,
      this.accnmodify,
      this.procmodify,
      this.isdlc,
      this.isunique,
      this.ismixingprep,
      this.isbatchno,
      this.dlcall,
      this.dlcfactory,
      this.dlcwarehouse,
      this.unitreceipt,
      this.innaturalloss,
      this.skulength,
      this.skuwidth,
      this.skuheight,
      this.skuweight,
      this.tihi,
      this.laslotno,
      this.lasdatemfg,
      this.lasdateexp,
      this.lasserialno,
      this.rtoskuofipck,
      this.rtoskuofpck,
      this.rtoskuoflayer,
      this.rtoskuofhu,
      this.huestimate,
      this.details,
      this.inseq,
      this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.inorder,
      this.inln,
      this.barcode,
      this.article,
      this.pv,
      this.lv,
      this.qtysku,
      this.qtypu,
      this.qtyweight,
      this.tflow,
      this.description,
      this.unitopsdesc});

  SearchPOLines.fromJson(Map<String, dynamic> json) {
    inrefno = json['inrefno'];
    inrefln = json['inrefln'];
    inagrn = json['inagrn'];
    unitops = json['unitops'];
    lotno = json['lotno'];
    expdate = json['expdate'];
    serialno = json['serialno'];
    qtypnd = json['qtypnd'] ?? 0.0;
    qtyskurec = json['qtyskurec'];
    qtypurec = json['qtypurec'];
    qtyhurec = json['qtyhurec'];
    qtyweightrec = json['qtyweightrec'] ?? 0.0;
    qtynaturalloss = json['qtynaturalloss'] ?? 0.0;
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    datemodify = json['datemodify'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    isdlc = json['isdlc'];
    isunique = json['isunique'];
    ismixingprep = json['ismixingprep'];
    isbatchno = json['isbatchno'];
    dlcall = json['dlcall'];
    dlcfactory = json['dlcfactory'];
    dlcwarehouse = json['dlcwarehouse'];
    unitreceipt = json['unitreceipt'];
    innaturalloss = json['innaturalloss'] ?? 0.0;
    skulength = json['skulength'] ?? 0.0;
    skuwidth = json['skuwidth'] ?? 0.0;
    skuheight = json['skuheight'] ?? 0.0;
    skuweight = json['skuweight'] ?? 0.0;
    tihi = json['tihi'];
    laslotno = json['laslotno'];
    lasdatemfg = json['lasdatemfg'];
    lasdateexp = json['lasdateexp'];
    lasserialno = json['lasserialno'];
    rtoskuofipck = json['rtoskuofipck'];
    rtoskuofpck = json['rtoskuofpck'];
    rtoskuoflayer = json['rtoskuoflayer'];
    rtoskuofhu = json['rtoskuofhu'];
    huestimate = json['huestimate'] ?? 0.0;
    details = json['details'];
    inseq = json['inseq'];
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    inorder = json['inorder'];
    inln = json['inln'];
    barcode = json['barcode'];
    article = json['article'];
    pv = json['pv'];
    lv = json['lv'];
    qtysku = json['qtysku'];
    qtypu = json['qtypu'];
    qtyweight = json['qtyweight'];
    tflow = json['tflow'];
    description = json['description'];
    unitopsdesc = json['unitopsdesc'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['inrefno'] = this.inrefno;
    data['inrefln'] = this.inrefln;
    data['inagrn'] = this.inagrn;
    data['unitops'] = this.unitops;
    data['lotno'] = this.lotno;
    data['expdate'] = this.expdate;
    data['serialno'] = this.serialno;
    data['qtypnd'] = this.qtypnd;
    data['qtyskurec'] = this.qtyskurec;
    data['qtypurec'] = this.qtypurec;
    data['qtyhurec'] = this.qtyhurec;
    data['qtyweightrec'] = this.qtyweightrec;
    data['qtynaturalloss'] = this.qtynaturalloss;
    data['datecreate'] = this.datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] = this.datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    data['isdlc'] = this.isdlc;
    data['isunique'] = this.isunique;
    data['ismixingprep'] = this.ismixingprep;
    data['isbatchno'] = this.isbatchno;
    data['dlcall'] = this.dlcall;
    data['dlcfactory'] = this.dlcfactory;
    data['dlcwarehouse'] = this.dlcwarehouse;
    data['unitreceipt'] = this.unitreceipt;
    data['innaturalloss'] = this.innaturalloss;
    data['skulength'] = this.skulength;
    data['skuwidth'] = this.skuwidth;
    data['skuheight'] = this.skuheight;
    data['skuweight'] = this.skuweight;
    data['tihi'] = this.tihi;
    data['laslotno'] = this.laslotno;
    data['lasdatemfg'] = this.lasdatemfg;
    data['lasdateexp'] = this.lasdateexp;
    data['lasserialno'] = this.lasserialno;
    data['rtoskuofipck'] = this.rtoskuofipck;
    data['rtoskuofpck'] = this.rtoskuofpck;
    data['rtoskuoflayer'] = this.rtoskuoflayer;
    data['rtoskuofhu'] = this.rtoskuofhu;
    data['huestimate'] = this.huestimate;
    data['details'] = this.details;
    data['inseq'] = this.inseq;
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['inorder'] = this.inorder;
    data['inln'] = this.inln;
    data['barcode'] = this.barcode;
    data['article'] = this.article;
    data['pv'] = this.pv;
    data['lv'] = this.lv;
    data['qtysku'] = this.qtysku;
    data['qtypu'] = this.qtypu;
    data['qtyweight'] = this.qtyweight;
    data['tflow'] = this.tflow;
    data['description'] = this.description;
    data['unitopsdesc'] = this.unitopsdesc;
    return data;
  }
}
