import 'dart:convert';

class CreateHu {
    String orgcode;
    String site;
    String depot;
    String loccode;
    String article;
    String pv;
    String lv;
    int qtypu;
    String qtyunit;
    String countcode;
    String plancode;
    String accncode;
    String remarks;
    String mergeno;
    String huno;
  CreateHu({
    this.orgcode,
    this.site,
    this.depot,
    this.loccode,
    this.article,
    this.pv,
    this.lv,
    this.qtypu,
    this.qtyunit,
    this.countcode,
    this.plancode,
    this.accncode,
    this.remarks,
    this.mergeno,
    this.huno,
  });



  Map<String, dynamic> toMap() {
    return {
      'orgcode': orgcode,
      'site': site,
      'depot': depot,
      'loccode': loccode,
      'article': article,
      'pv': pv,
      'lv': lv,
      'qtypu': qtypu,
      'qtyunit': qtyunit,
      'countcode': countcode,
      'plancode': plancode,
      'accncode': accncode,
      'remarks': remarks,
      'mergeno': mergeno,
      'huno': huno,
    };
  }

  factory CreateHu.fromMap(Map<String, dynamic> map) {
    return CreateHu(
      orgcode: map['orgcode'],
      site: map['site'],
      depot: map['depot'],
      loccode: map['loccode'],
      article: map['article'],
      pv: map['pv'],
      lv: map['lv'],
      qtypu: map['qtypu'],
      qtyunit: map['qtyunit'],
      countcode: map['countcode'],
      plancode: map['plancode'],
      accncode: map['accncode'],
      remarks: map['remarks'],
      mergeno: map['mergeno'],
      huno: map['huno'],
    );
  }

  String toJson() => json.encode(toMap());

  factory CreateHu.fromJson(String source) => CreateHu.fromMap(json.decode(source));
}
