import 'roleaccs.dart';

class Profiles {
  String orgcode;
  String apcode;
  String accncode;
  String site;
  String depot;
  String cfgtype;
  String cfgcode;
  String cfgname;
  String cfgvalue;
  String cfghash;
  String tflow;
  String formatdate;
  String formatdateshort;
  String formatdatelong;
  String unitdimension;
  String unitweight;
  String unitvolume;
  String unitcubic;
  int pagelimit;
  String lang;
  String datecreate;
  String accncreate;
  String datemodify;
  String accnmodify;
  String procmodify;
  Roleaccs roleaccs;
  int isdefault;

  Profiles(
      {this.orgcode,
      this.apcode,
      this.accncode,
      this.site,
      this.depot,
      this.cfgtype,
      this.cfgcode,
      this.cfgname,
      this.cfgvalue,
      this.cfghash,
      this.tflow,
      this.formatdate,
      this.formatdateshort,
      this.formatdatelong,
      this.unitdimension,
      this.unitweight,
      this.unitvolume,
      this.unitcubic,
      this.pagelimit,
      this.lang,
      this.datecreate,
      this.accncreate,
      this.datemodify,
      this.accnmodify,
      this.procmodify,
      this.roleaccs,
      this.isdefault});

  Profiles.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    apcode = json['apcode'];
    accncode = json['accncode'];
    site = json['site'];
    depot = json['depot'];
    cfgtype = json['cfgtype'];
    cfgcode = json['cfgcode'];
    cfgname = json['cfgname'];
    cfgvalue = json['cfgvalue'];
    cfghash = json['cfghash'];
    tflow = json['tflow'];
    formatdate = json['formatdate'];
    formatdateshort = json['formatdateshort'];
    formatdatelong = json['formatdatelong'];
    unitdimension = json['unitdimension'];
    unitweight = json['unitweight'];
    unitvolume = json['unitvolume'];
    unitcubic = json['unitcubic'];
    pagelimit = json['pagelimit'];
    lang = json['lang'];
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    datemodify = json['datemodify'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    roleaccs = json['roleaccs'] != null
        ? new Roleaccs.fromJson(json['roleaccs'])
        : null;
    isdefault = json['isdefault'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['apcode'] = this.apcode;
    data['accncode'] = this.accncode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['cfgtype'] = this.cfgtype;
    data['cfgcode'] = this.cfgcode;
    data['cfgname'] = this.cfgname;
    data['cfgvalue'] = this.cfgvalue;
    data['cfghash'] = this.cfghash;
    data['tflow'] = this.tflow;
    data['formatdate'] = this.formatdate;
    data['formatdateshort'] = this.formatdateshort;
    data['formatdatelong'] = this.formatdatelong;
    data['unitdimension'] = this.unitdimension;
    data['unitweight'] = this.unitweight;
    data['unitvolume'] = this.unitvolume;
    data['unitcubic'] = this.unitcubic;
    data['pagelimit'] = this.pagelimit;
    data['lang'] = this.lang;
    data['datecreate'] = this.datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] = this.datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    if (this.roleaccs != null) {
      data['roleaccs'] = this.roleaccs.toJson();
    }
    data['isdefault'] = this.isdefault;
    return data;
  }
}
