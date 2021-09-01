class Productvld {
  String orgcode;
  String site;
  String depot;
  String barcode;
  String article;
  int pv;
  int lv;
  String descalt;
  double skuweight;
  double skuvolume;
  String unitmanage;
  String loccode;
  String locarea;
  String loctype;
  String locunit;
  String huno;
  String unitcount;
  String unitdestr;
  int skuofunit;
  String countcode;
  String plancode;
  String linecode;
  int qtycount;
  String accncode;
  bool isnewhu;
  String lotmfg;
  DateTime datemfg;
  DateTime dateexp;
  String serialno;
  int rtoskuofpu;
  int rtopckoflayer;
  int rtolayerofhu;
  int rtopckofpallet;
  int rtoskuofipck;
  int rtoskuofpck;
  int rtoskuoflayer;
  int rtoskuofhu;

  Productvld(
      {this.orgcode,
      this.site,
      this.depot,
      this.barcode,
      this.article,
      this.pv,
      this.lv,
      this.descalt,
      this.skuweight,
      this.skuvolume,
      this.unitmanage,
      this.loccode,
      this.locarea,
      this.loctype,
      this.locunit,
      this.huno,
      this.unitcount,
      this.unitdestr,
      this.skuofunit,
      this.countcode,
      this.plancode,
      this.linecode,
      this.qtycount,
      this.accncode,
      this.isnewhu,
      this.lotmfg,
      this.datemfg,
      this.dateexp,
      this.serialno,
      this.rtoskuofpu,
      this.rtopckoflayer,
      this.rtolayerofhu,
      this.rtopckofpallet,
      this.rtoskuofipck,
      this.rtoskuofpck,
      this.rtoskuoflayer,
      this.rtoskuofhu});

  Productvld.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    barcode = json['barcode'];
    article = json['article'];
    pv = json['pv'];
    lv = json['lv'];
    descalt = json['descalt'];
    skuweight = json['skuweight'];
    skuvolume = json['skuvolume'];
    unitmanage = json['unitmanage'];
    loccode = json['loccode'];
    locarea = json['locarea'];
    loctype = json['loctype'];
    locunit = json['locunit'];
    huno = json['huno'];
    unitcount = json['unitcount'];
    unitdestr = json['unitdestr'];
    skuofunit = json['skuofunit'];
    countcode = json['countcode'];
    plancode = json['plancode'];
    linecode = json['linecode'];
    qtycount = json['qtycount'];
    accncode = json['accncode'];
    isnewhu = json['isnewhu'];
    lotmfg = json['lotmfg'];
    datemfg = json['datemfg'] == null ? null : DateTime.parse(json['datemfg']);
    dateexp = json['dateexp'] == null ? null : DateTime.parse(json['dateexp']);
    serialno = json['serialno'];
    rtoskuofpu = json['rtoskuofpu'];
    rtopckoflayer = json['rtopckoflayer'];
    rtolayerofhu = json['rtolayerofhu'];
    rtopckofpallet = json['rtopckofpallet'];
    rtoskuofipck = json['rtoskuofipck'];
    rtoskuofpck = json['rtoskuofpck'];
    rtoskuoflayer = json['rtoskuoflayer'];
    rtoskuofhu = json['rtoskuofhu'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['barcode'] = this.barcode;
    data['article'] = this.article;
    data['pv'] = this.pv;
    data['lv'] = this.lv;
    data['descalt'] = this.descalt;
    data['skuweight'] = this.skuweight;
    data['skuvolume'] = this.skuvolume;
    data['unitmanage'] = this.unitmanage;
    data['loccode'] = this.loccode;
    data['locarea'] = this.locarea;
    data['loctype'] = this.loctype;
    data['locunit'] = this.locunit;
    data['huno'] = this.huno;
    data['unitcount'] = this.unitcount;
    data['unitdestr'] = this.unitdestr;
    data['skuofunit'] = this.skuofunit;
    data['countcode'] = this.countcode;
    data['plancode'] = this.plancode;
    data['linecode'] = this.linecode;
    data['qtycount'] = this.qtycount;
    data['accncode'] = this.accncode;
    data['isnewhu'] = this.isnewhu;
    data['lotmfg'] = this.lotmfg;
    data['datemfg'] = this.datemfg != null ? datemfg.toIso8601String() : datemfg;
    data['dateexp'] = this.dateexp != null ? dateexp.toIso8601String() : dateexp;
    data['serialno'] = this.serialno;
    data['rtoskuofpu'] = this.rtoskuofpu;
    data['rtopckoflayer'] = this.rtopckoflayer;
    data['rtolayerofhu'] = this.rtolayerofhu;
    data['rtopckofpallet'] = this.rtopckofpallet;
    data['rtoskuofipck'] = this.rtoskuofipck;
    data['rtoskuofpck'] = this.rtoskuofpck;
    data['rtoskuoflayer'] = this.rtoskuoflayer;
    data['rtoskuofhu'] = this.rtoskuofhu;
    return data;
  }
}
