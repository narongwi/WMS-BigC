class Permission {
  String orgcode;
  String apcode;
  String rolecode;
  String objmodule;
  String objtype;
  String objcode;
  String objname;
  int objseq;
  String objroute;
  String objicon;
  int isenable;
  int isexecute;

  Permission(
      {this.orgcode,
      this.apcode,
      this.rolecode,
      this.objmodule,
      this.objtype,
      this.objcode,
      this.objname,
      this.objseq,
      this.objroute,
      this.objicon,
      this.isenable,
      this.isexecute});

  Permission.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    apcode = json['apcode'];
    rolecode = json['rolecode'];
    objmodule = json['objmodule'];
    objtype = json['objtype'];
    objcode = json['objcode'];
    objname = json['objname'];
    objseq = json['objseq'];
    objroute = json['objroute'];
    objicon = json['objicon'];
    isenable = json['isenable'];
    isexecute = json['isexecute'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['apcode'] = this.apcode;
    data['rolecode'] = this.rolecode;
    data['objmodule'] = this.objmodule;
    data['objtype'] = this.objtype;
    data['objcode'] = this.objcode;
    data['objname'] = this.objname;
    data['objseq'] = this.objseq;
    data['objroute'] = this.objroute;
    data['objicon'] = this.objicon;
    data['isenable'] = this.isenable;
    data['isexecute'] = this.isexecute;
    return data;
  }
}
