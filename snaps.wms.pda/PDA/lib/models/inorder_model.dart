class INB {
  bool success;
  String message;
  Data data;
  INB({
    this.success,
    this.message,
    this.data,
  });
  factory INB.fromJson(Map<String, dynamic> json) => INB(
        success: json["success"],
        message: json["message"],
        data: Data.fromJson(json["data"]),
      );
}

class IND {
  IND({
    this.success,
    this.message,
    this.data,
  });
  bool success;
  String message;
  List<OrderLine> data;

  factory IND.fromJson(Map<String, dynamic> json) => IND(
      success: json["success"],
      message: json["message"],
      data: (json["data"] as List).map((i) => OrderLine.fromJson(i)).toList());
}

class INL {
  INL({
    this.success,
    this.message,
    this.data,
  });
  bool success;
  String message;
  OrderLine data;

  factory INL.fromJson(Map<String, dynamic> json) => INL(
      success: json["success"],
      message: json["message"],
      data: OrderLine.fromJson(
          (json["data"] as List).length > 0 ? json["data"][0] : json["data"]));
}

class Data {
  Order pohead;
  List<OrderDetail> poline;
  Data({this.pohead, this.poline});

  factory Data.fromJson(Map<String, dynamic> json) => Data(
      pohead: json["header"] == null ? null : Order.fromJson(json["header"]),
      poline: json["line"] == null
          ? null
          : (json["line"] as List)
              .map((i) => OrderDetail.fromJson(i))
              .toList());
}

class Order {
  Order({
    this.orgcode,
    this.site,
    this.depot,
    this.thcode,
    this.thnameint,
    this.inorder,
    this.dockrec,
    this.recdate,
    this.tflow,
  });

  String orgcode;
  String site;
  String depot;
  String thcode;
  String thnameint;
  String inorder;
  String dockrec;
  String recdate;
  String tflow;

  factory Order.fromJson(Map<String, dynamic> json) => Order(
        orgcode: json["orgcode"],
        site: json["site"],
        depot: json["depot"],
        thcode: json["thcode"],
        thnameint: json["thnameint"],
        inorder: json["inorder"],
        dockrec: json["dockrec"],
        recdate: json["recdate"],
        tflow: json["tflow"],
      );

  Map<String, dynamic> toJson() => {
        "orgcode": orgcode,
        "site": site,
        "depot": depot,
        "thcode": thcode,
        "thnameint": thnameint,
        "inorder": inorder,
        "dockrec": dockrec,
        "tflow": tflow,
      };
}

class OrderDetail {
  OrderDetail(
      {this.inorder,
      this.article,
      this.lv,
      this.descalt,
      this.qtypu,
      this.qtysku,
      this.tflow,
      this.unitopsdesc,
      this.recpu});

  String inorder;
  String article;
  int lv;
  String descalt;
  int qtypu;
  int qtysku;
  String tflow;
  String unitopsdesc;
  int recpu;

  factory OrderDetail.fromJson(Map<String, dynamic> json) => OrderDetail(
        inorder: json["inorder"],
        article: json["article"],
        lv: json["lv"],
        descalt: json["descalt"],
        qtypu: json["qtypu"],
        qtysku: json["qtysku"],
        tflow: json["tflow"],
        unitopsdesc: json["unitopsdesc"],
        recpu: json["recpu"],
      );

  Map<String, dynamic> toJson() => {
        "inorder": inorder,
        "article": article,
        "lv": lv,
        "descalt": descalt,
        "qtypu": qtypu,
        "qtysku": qtysku,
        "tflow": tflow,
        "unitopsdesc": unitopsdesc,
        "recpu": recpu,
      };
}

class OrderLine {
  OrderLine(
      {this.inorder,
      this.spcarea,
      this.inagrn,
      this.inrefln,
      this.inrefno,
      this.inln,
      this.inseq,
      this.barcode,
      this.article,
      this.lv,
      this.pv,
      this.descalt,
      this.unitops,
      this.qtysku,
      this.qtypu,
      this.qtyweight,
      this.qtypnd,
      this.qtypurec,
      this.qtyskurec,
      this.qtyweightrec,
      this.qtyhurec,
      this.tflow,
      this.unitopsdesc,
      this.isdlc,
      this.isunique,
      this.ismixingprep,
      this.isbatchno,
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
      this.rtoskuofpu,
      this.rtopckoflayer,
      this.rtolayerofhu,
      this.rtopckofpallet});

  String inorder;
  String spcarea;
  String inagrn;
  int inrefln;
  String inrefno;
  String inln;
  int inseq;
  String barcode;
  String article;
  int lv;
  int pv;
  String descalt;
  String unitops;
  int qtysku;
  int qtypu;
  double qtyweight;
  double qtypnd;
  int qtypurec;
  int qtyskurec;
  double qtyweightrec;
  int qtyhurec;
  String tflow;
  String unitopsdesc;
  int isdlc;
  int isunique;
  int ismixingprep;
  int isbatchno;
  int dlcfactory;
  int dlcwarehouse;
  String unitreceipt;
  double innaturalloss;
  double skulength;
  double skuwidth;
  double skuheight;
  double skuweight;
  String tihi;
  dynamic laslotno;
  dynamic lasdatemfg;
  dynamic lasdateexp;
  String lasserialno;
  int rtoskuofipck;
  int rtoskuofpck;
  int rtoskuoflayer;
  int rtoskuofhu;
  double huestimate;
  int rtoskuofpu;
  int rtopckoflayer;
  int rtolayerofhu;
  int rtopckofpallet;
  factory OrderLine.fromJson(Map<String, dynamic> json) {
    if (json == null) return OrderLine();
    return OrderLine(
        inorder: json["inorder"],
        spcarea: json["spcarea"],
        inagrn: json["inagrn"],
        inrefln: json["inrefln"],
        inrefno: json["inrefno"],
        inln: json["inln"],
        inseq: json["inseq"],
        barcode: json["barcode"],
        article: json["article"],
        lv: json["lv"],
        pv: json["pv"],
        descalt: json["descalt"],
        unitops: json["unitops"],
        qtysku: json["qtysku"],
        qtypu: json["qtypu"],
        qtyweight: json["qtyweight"],
        qtypnd: json["qtypnd"],
        qtypurec: json["qtypurec"],
        qtyskurec: json["qtyskurec"],
        qtyweightrec: json["qtyweightrec"],
        qtyhurec: json["qtyhurec"],
        tflow: json["tflow"],
        unitopsdesc: json["unitopsdesc"],
        isdlc: json["isdlc"],
        isunique: json["isunique"],
        ismixingprep: json["ismixingprep"],
        isbatchno: json["isbatchno"],
        dlcfactory: json["dlcfactory"],
        dlcwarehouse: json["dlcwarehouse"],
        unitreceipt: json["unitreceipt"],
        innaturalloss: json["innaturalloss"],
        skulength: json["skulength"],
        skuwidth: json["skuwidth"],
        skuheight: json["skuheight"],
        skuweight: json["skuweight"],
        tihi: json["tihi"],
        laslotno: json["laslotno"],
        lasdatemfg: json["lasdatemfg"],
        lasdateexp: json["lasdateexp"],
        lasserialno: json["lasserialno"],
        rtoskuofipck: json["rtoskuofipck"],
        rtoskuofpck: json["rtoskuofpck"],
        rtoskuoflayer: json["rtoskuoflayer"],
        rtoskuofhu: json["rtoskuofhu"],
        huestimate: json["huestimate"],
        rtoskuofpu: json["rtoskuofpu"],
        rtopckoflayer: json["rtopckoflayer"],
        rtolayerofhu: json["rtolayerofhu"],
        rtopckofpallet: json["rtopckofpallet"]);
  }

  Map<String, dynamic> toJson() => {
        "inorder": inorder,
        "spcarea": spcarea,
        "inagrn": inagrn,
        "inrefln": inrefln,
        "inrefno": inrefno,
        "inln": inln,
        "inseq": inseq,
        "barcode": barcode,
        "article": article,
        "lv": lv,
        "pv": pv,
        "descalt": descalt,
        "unitops": unitops,
        "qtysku": qtysku,
        "qtypu": qtypu,
        "qtyweight": qtyweight,
        "qtypnd": qtypnd,
        "qtypurec": qtypurec,
        "qtyskurec": qtyskurec,
        "qtyweightrec": qtyweightrec,
        "qtyhurec": qtyhurec,
        "tflow": tflow,
        "unitopsdesc": unitopsdesc,
        "isdlc": isdlc,
        "isunique": isunique,
        "ismixingprep": ismixingprep,
        "isbatchno": isbatchno,
        "dlcfactory": dlcfactory,
        "dlcwarehouse": dlcwarehouse,
        "unitreceipt": unitreceipt,
        "innaturalloss": innaturalloss,
        "skulength": skulength,
        "skuwidth": skuwidth,
        "skuheight": skuheight,
        "skuweight": skuweight,
        "tihi": tihi,
        "laslotno": laslotno,
        "lasdatemfg": lasdatemfg,
        "lasdateexp": lasdateexp,
        "lasserialno": lasserialno,
        "rtoskuofipck": rtoskuofipck,
        "rtoskuofpck": rtoskuofpck,
        "rtoskuoflayer": rtoskuoflayer,
        "rtoskuofhu": rtoskuofhu,
        "huestimate": huestimate,
        "rtoskuofpu": rtoskuofpu,
        "rtopckoflayer": rtopckoflayer,
        "rtolayerofhu": rtolayerofhu,
        "rtopckofpallet": rtopckofpallet
      };
}

class RCV {
  String orgcode;
  String site;
  String depot;
  String inorder;
  String dockno;
  String barcode;
  String article;
  String lv;
  int unit;
  int qty;

  RCV(this.orgcode, this.site, this.depot, this.inorder, this.dockno,
      this.barcode, this.article, this.lv, this.unit, this.qty);

  Map<String, dynamic> toJson() => {
        "orgcode": orgcode,
        "site": site,
        "depot": depot,
        "inorder": inorder,
        "dockno": dockno,
        "barcode": barcode,
        "article": article,
        "lv": lv,
        "unit": unit,
        "qty": qty
      };
}

class CNF {
  String inorder;
  String staging;
  String inln;
  String barcode;
  String article;
  int pv;
  int lv;
  String lot;
  String serial;
  String mfg;
  String exp;
  int qty;
  CNF({
    this.inorder,
    this.staging,
    this.inln,
    this.barcode,
    this.article,
    this.pv,
    this.lv,
    this.lot,
    this.serial,
    this.mfg,
    this.exp,
    this.qty,
  });
  Map<String, dynamic> toJson() => {
        "inorder": inorder,
        "staging": staging,
        "inln": inln,
        "barcode": barcode,
        "article": article,
        "pv": pv,
        "lv": lv,
        "lot": lot,
        "serial": serial,
        "mfg": mfg,
        "exp": exp,
        "qty": qty
      };
}
