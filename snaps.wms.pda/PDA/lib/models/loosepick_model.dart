class LoospickItem {
  LoospickItem(
      {this.spcarea,
      this.huno,
      this.prepno,
      this.setno,
      this.toLocation,
      this.lszone,
      this.pickpath,
      this.prepln,
      this.barcode,
      this.article,
      this.pv,
      this.lv,
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
      this.nextlocation,
      this.qty,
      this.puqty,
      this.prepskuqty,
      this.preppuqty,
      this.routeno,
      this.thcode,
      this.prepdate,
      this.picker});
  String spcarea;
  String huno;
  String prepno;
  String setno;
  String toLocation;
  String lszone;
  int pickpath;
  int prepln;
  String barcode;
  String article;
  int pv;
  int lv;
  String description;
  String puCode;
  int skuPu;
  int skuPallet;
  int pckLayer;
  int layerPallet;
  int pckPallet;
  String serialno;
  String batchno;
  dynamic datemfg;
  dynamic dateexp;
  dynamic nextlocation;
  int qty;
  int puqty;
  int preppuqty;
  int prepskuqty;
  String routeno;
  String thcode;
  String prepdate;
  String picker;
  factory LoospickItem.fromJson(Map<String, dynamic> json) => LoospickItem(
      spcarea: json["spcarea"],
      huno: json["huno"],
      prepno: json["prepno"],
      setno: json["setno"],
      toLocation: json["to_location"],
      lszone: json["lszone"],
      pickpath: json["pickpath"],
      prepln: json["prepln"],
      barcode: json["barcode"],
      article: json["article"],
      pv: json["pv"],
      lv: json["lv"],
      description: json["description"],
      puCode: json["pu_code"],
      skuPu: json["sku_pu"],
      skuPallet: json["sku_pallet"],
      pckLayer: json["pck_layer"],
      layerPallet: json["layer_pallet"],
      pckPallet: json["pck_pallet"],
      serialno: json["serialno"],
      batchno: json["batchno"],
      datemfg: json["datemfg"],
      dateexp: json["dateexp"],
      nextlocation: json["nextlocation"],
      qty: json["qty"],
      puqty: json["puqty"],
      prepskuqty: json["prepskuqty"],
      preppuqty: json["preppuqty"],
      routeno: json["routeno"],
      thcode: json["thcode"],
      prepdate: json["prepdate"],
      picker: json["picker"]);

  Map<String, dynamic> toJson() => {
        "spcarea": spcarea,
        "huno": huno,
        "prepno": prepno,
        "setno": setno,
        "toLocation": toLocation,
        "lszone": lszone,
        "pickpath": pickpath,
        "prepln": prepln,
        "barcode": barcode,
        "article": article,
        "pv": pv,
        "lv": lv,
        "description": description,
        "puCode": puCode,
        "skuPu": skuPu,
        "skuPallet": skuPallet,
        "pckLayer": pckLayer,
        "layerPallet": layerPallet,
        "pckPallet": pckPallet,
        "serialno": serialno,
        "batchno": batchno,
        "datemfg": datemfg,
        "dateexp": dateexp,
        "nextlocation": nextlocation,
        "qty": qty,
        "puqty": puqty,
        "prepskuqty": prepskuqty,
        "preppuqty": preppuqty,
        "routeno": routeno,
        "thcode": thcode,
        "prepdate": prepdate,
        "picker": picker
      };
}
