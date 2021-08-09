import 'package:wms/screens/receive/models/searchpo_line.dart';

class SearchPO {
  String intype;
  String inflag;
  String dockrec;
  String invno;
  String datecreate;
  String accncreate;
  String datemodify;
  String accnmodify;
  String procmodify;
  String orbitsource;
  dynamic dateassign;
  dynamic dateunloadstart;
  dynamic dateunloadend;
  dynamic datefinish;
  List<SearchPOLines> lines;
  String orgcode;
  String site;
  String depot;
  String spcarea;
  String thcode;
  String subtype;
  String inorder;
  String dateorder;
  String dateplan;
  String dateexpire;
  dynamic slotdate;
  String slotno;
  int inpriority;
  String inpromo;
  String tflow;
  dynamic daterec;
  String thname;
  String remarkrec;
  int isreqmeasurement;
  dynamic dateremarks;
  int opsprogress;

  SearchPO(
      {this.intype,
      this.inflag,
      this.dockrec,
      this.invno,
      this.datecreate,
      this.accncreate,
      this.datemodify,
      this.accnmodify,
      this.procmodify,
      this.orbitsource,
      this.dateassign,
      this.dateunloadstart,
      this.dateunloadend,
      this.datefinish,
      this.lines,
      this.orgcode,
      this.site,
      this.depot,
      this.spcarea,
      this.thcode,
      this.subtype,
      this.inorder,
      this.dateorder,
      this.dateplan,
      this.dateexpire,
      this.slotdate,
      this.slotno,
      this.inpriority,
      this.inpromo,
      this.tflow,
      this.daterec,
      this.thname,
      this.remarkrec,
      this.isreqmeasurement,
      this.dateremarks,
      this.opsprogress});

  SearchPO.fromJson(Map<String, dynamic> json) {
    intype = json['intype'];
    inflag = json['inflag'];
    dockrec = json['dockrec'];
    invno = json['invno'];
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    datemodify = json['datemodify'];
    accnmodify = json['accnmodify'];
    procmodify = json['procmodify'];
    orbitsource = json['orbitsource'];
    dateassign = json['dateassign'];
    dateunloadstart = json['dateunloadstart'];
    dateunloadend = json['dateunloadend'];
    datefinish = json['datefinish'];
    if (json['lines'] != null) {
      lines = <SearchPOLines>[];
      json['lines'].forEach((v) {
        lines.add(new SearchPOLines.fromJson(v));
      });
    }
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    spcarea = json['spcarea'];
    thcode = json['thcode'];
    subtype = json['subtype'];
    inorder = json['inorder'];
    dateorder = json['dateorder'];
    dateplan = json['dateplan'];
    dateexpire = json['dateexpire'];
    slotdate = json['slotdate'];
    slotno = json['slotno'];
    inpriority = json['inpriority'];
    inpromo = json['inpromo'];
    tflow = json['tflow'];
    daterec = json['daterec'];
    thname = json['thname'];
    remarkrec = json['remarkrec'];
    isreqmeasurement = json['isreqmeasurement'];
    dateremarks = json['dateremarks'];
    opsprogress = json['opsprogress'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['intype'] = this.intype;
    data['inflag'] = this.inflag;
    data['dockrec'] = this.dockrec;
    data['invno'] = this.invno;
    data['datecreate'] = this.datecreate;
    data['accncreate'] = this.accncreate;
    data['datemodify'] = this.datemodify;
    data['accnmodify'] = this.accnmodify;
    data['procmodify'] = this.procmodify;
    data['orbitsource'] = this.orbitsource;
    data['dateassign'] = this.dateassign;
    data['dateunloadstart'] = this.dateunloadstart;
    data['dateunloadend'] = this.dateunloadend;
    data['datefinish'] = this.datefinish;
    if (this.lines != null) {
      data['lines'] = this.lines.map((v) => v.toJson()).toList();
    }
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['spcarea'] = this.spcarea;
    data['thcode'] = this.thcode;
    data['subtype'] = this.subtype;
    data['inorder'] = this.inorder;
    data['dateorder'] = this.dateorder;
    data['dateplan'] = this.dateplan;
    data['dateexpire'] = this.dateexpire;
    data['slotdate'] = this.slotdate;
    data['slotno'] = this.slotno;
    data['inpriority'] = this.inpriority;
    data['inpromo'] = this.inpromo;
    data['tflow'] = this.tflow;
    data['daterec'] = this.daterec;
    data['thname'] = this.thname;
    data['remarkrec'] = this.remarkrec;
    data['isreqmeasurement'] = this.isreqmeasurement;
    data['dateremarks'] = this.dateremarks;
    data['opsprogress'] = this.opsprogress;
    return data;
  }
}
