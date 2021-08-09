class Product {
  String barcode;
  String article;
  int pv;
  int lv;
  String descalt;
  int rtoskuofpu;
  int rtopckoflayer;
  int rtolayerofhu;
  int rtopckofpallet;
  int rtoskuofipck;
  int rtoskuofpck;
  int rtoskuoflayer;
  int rtoskuofhu;
  String unitprep;
  String unitreceipt;
  String unitmanage;
  String unitdesc;
  Product({
    this.barcode,
    this.article,
    this.pv,
    this.lv,
    this.descalt,
    this.rtoskuofpu,
    this.rtopckoflayer,
    this.rtolayerofhu,
    this.rtopckofpallet,
    this.rtoskuofipck,
    this.rtoskuofpck,
    this.rtoskuoflayer,
    this.rtoskuofhu,
    this.unitprep,
    this.unitreceipt,
    this.unitmanage,
    this.unitdesc,
  });

  Product.fromJson(Map<String, dynamic> json) {
    barcode = json['barcode'];
    article = json['article'];
    pv = json['pv'];
    lv = json['lv'];
    descalt = json['descalt'];
    rtoskuofpu = json['rtoskuofpu'];
    rtopckoflayer = json['rtopckoflayer'];
    rtolayerofhu = json['rtolayerofhu'];
    rtopckofpallet = json['rtopckofpallet'];
    rtoskuofipck = json['rtoskuofipck'];
    rtoskuofpck = json['rtoskuofpck'];
    rtoskuoflayer = json['rtoskuoflayer'];
    rtoskuofhu = json['rtoskuofhu'];
    unitprep = json['unitprep'];
    unitreceipt = json['unitreceipt'];
    unitmanage = json['unitmanage'];
    unitdesc = json['unitdesc'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['barcode'] = this.barcode;
    data['article'] = this.article;
    data['pv'] = this.pv;
    data['lv'] = this.lv;
    data['descalt'] = this.descalt;
    data['rtoskuofpu'] = this.rtoskuofpu;
    data['rtopckoflayer'] = this.rtopckoflayer;
    data['rtolayerofhu'] = this.rtolayerofhu;
    data['rtopckofpallet'] = this.rtopckofpallet;
    data['rtoskuofipck'] = this.rtoskuofipck;
    data['rtoskuofpck'] = this.rtoskuofpck;
    data['rtoskuoflayer'] = this.rtoskuoflayer;
    data['rtoskuofhu'] = this.rtoskuofhu;
    data['unitprep'] = this.unitprep;
    data['unitreceipt'] = this.unitreceipt;
    data['unitmanage'] = this.unitmanage;
    data['unitdesc'] = this.unitdesc;
    return data;
  }
}
