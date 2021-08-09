class Tasks {
  String sourceloc;
  String targetadv;
  String sourcehuno;
  String article;
  int lv;
  String barcode;
  String description;
  int qty;
  Tasks(
      {this.sourceloc,
      this.targetadv,
      this.sourcehuno,
      this.article,
      this.lv,
      this.barcode,
      this.description,
      this.qty});

  factory Tasks.fromJson(Map<String, dynamic> json) => Tasks(
        sourceloc: json["sourceloc"],
        targetadv: json["targetadv"],
        sourcehuno: json["sourcehuno"],
        article: json["article"],
        lv: json["lv"],
        barcode: json["barcode"],
        description: json["description"],
        qty: json["qty"],
      );
}

class TaskDetail {
  TaskDetail({
    this.taskno,
    this.sourceloc,
    this.targetadv,
    this.suppCode,
    this.spcarea,
    this.suppName,
    this.recDock,
    this.recDate,
    this.huno,
    this.article,
    this.pv,
    this.lv,
    this.barcode,
    this.description,
    this.puCode,
    this.skuPu,
    this.skuPallet,
    this.pckLayer,
    this.layerPallet,
    this.pckPallet,
    this.serialno,
    this.batchno,
    this.datemfg,
    this.dateexp,
    this.toLocation,
    this.confirmLoc,
    this.qty,
    this.digit,
    this.accnassign,
  });

  String taskno;
  String sourceloc;
  String targetadv;
  String suppCode;
  String spcarea;
  String suppName;
  String recDock;
  String recDate;
  String huno;
  String article;
  int pv;
  int lv;
  String barcode;
  String description;
  String puCode;
  int skuPu;
  int skuPallet;
  int pckLayer;
  int layerPallet;
  int pckPallet;
  String serialno;
  String batchno;
  DateTime datemfg;
  DateTime dateexp;
  String toLocation;
  String confirmLoc;
  int qty;
  String digit;
  String accnassign;

  factory TaskDetail.fromJson(Map<String, dynamic> json) => TaskDetail(
        taskno: json["taskno"],
        sourceloc: json["sourceloc"],
        targetadv: json["targetadv"],
        suppCode: json["suppcode"],
        spcarea: json["spcarea"],
        suppName: json["suppname"],
        recDock: json["recdock"],
        recDate: json["recdate"],
        huno: json["huno"],
        article: json["article"],
        pv: json["pv"],
        lv: json["lv"],
        barcode: json["barcode"],
        description: json["description"],
        puCode: json["pucode"],
        skuPu: json["skupu"],
        skuPallet: json["skupallet"],
        pckLayer: json["pcklayer"],
        layerPallet: json["layerpallet"],
        pckPallet: json["pckpallet"],
        serialno: json["serialno"],
        batchno: json["batchno"],
        datemfg:
            json["datemfg"] == null ? null : DateTime.parse(json["datemfg"]),
        dateexp:
            json["dateexp"] == null ? null : DateTime.parse(json["dateexp"]),
        toLocation: json["tolocation"],
        confirmLoc: json["confirmLoc"],
        qty: json["qty"],
        digit: json["digit"],
        accnassign: json["accnassign"],
      );

  Map<String, dynamic> toJson() => {
        "taskno": taskno,
        "sourceloc": sourceloc,
        "targetadv": targetadv,
        "suppcode": suppCode,
        "spcarea": spcarea,
        "suppname": suppName,
        "recdock": recDock,
        "recdate": recDate,
        "huno": huno,
        "article": article,
        "pv": pv,
        "lv": lv,
        "barcode": barcode,
        "description": description,
        "pu_code": puCode,
        "sku_pu": skuPu,
        "skupallet": skuPallet,
        "pcklayer": pckLayer,
        "layerpallet": layerPallet,
        "pckpallet": pckPallet,
        "serialno": serialno,
        "batchno": batchno,
        "datemfg": datemfg != null ? datemfg.toIso8601String() : datemfg,
        "dateexp": dateexp != null ? dateexp.toIso8601String() : dateexp,
        "tolocation": toLocation,
        "confirmLoc": confirmLoc,
        "qty": qty,
        "digit": digit,
        "accnassign": accnassign
      };
}

// class TaskMd {
//   String orgcode;
//   String site;
//   String depot;
//   String spcarea;
//   String tasktype;
//   String taskno;
//   String iopromo;
//   String iorefno;
//   String priority;
//   DateTime taskdate;
//   String tflow;
//   DateTime datemodify;
//   DateTime datestart;
//   DateTime dateend;
//   DateTime datecreate;
//   String accncreate;
//   String accnmodify;
//   String procmodify;
//   String taskname;
//   dynamic lines;
//   String confirmdigit;
//   String devid;
//   String routeno;
//   String routethcode;
//   String opscode;
// }
