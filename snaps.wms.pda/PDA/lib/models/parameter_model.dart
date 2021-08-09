class Parameters {
  String orgcode;
  String site;
  String depot;
  String apps;
  dynamic datecreate;
  dynamic accncreate;
  String pmdesc;
  String pmdescalt;
  String datemodify;
  String accnmodify;
  String pmmodule;
  String pmtype;
  String pmcode;
  bool pmvalue;
  String pmstate;
  String pmoption;

  Parameters(
      {this.orgcode,
      this.site,
      this.depot,
      this.apps,
      this.datecreate,
      this.accncreate,
      this.pmdesc,
      this.pmdescalt,
      this.datemodify,
      this.accnmodify,
      this.pmmodule,
      this.pmtype,
      this.pmcode,
      this.pmvalue = false,
      this.pmstate,
      this.pmoption});

  Parameters.fromJson(Map<String, dynamic> json) {
    orgcode = json['orgcode'];
    site = json['site'];
    depot = json['depot'];
    apps = json['apps'];
    datecreate = json['datecreate'];
    accncreate = json['accncreate'];
    pmdesc = json['pmdesc'];
    pmdescalt = json['pmdescalt'];
    datemodify = json['datemodify'];
    accnmodify = json['accnmodify'];
    pmmodule = json['pmmodule'];
    pmtype = json['pmtype'];
    pmcode = json['pmcode'];
    pmvalue = json['pmvalue'];
    pmstate = json['pmstate'];
    pmoption = json['pmoption'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['orgcode'] = this.orgcode;
    data['site'] = this.site;
    data['depot'] = this.depot;
    data['apps'] = this.apps;
    data['datecreate'] = this.datecreate;
    data['accncreate'] = this.accncreate;
    data['pmdesc'] = this.pmdesc;
    data['pmdescalt'] = this.pmdescalt;
    data['datemodify'] = this.datemodify;
    data['accnmodify'] = this.accnmodify;
    data['pmmodule'] = this.pmmodule;
    data['pmtype'] = this.pmtype;
    data['pmcode'] = this.pmcode;
    data['pmvalue'] = this.pmvalue;
    data['pmstate'] = this.pmstate;
    data['pmoption'] = this.pmoption;
    return data;
  }
}
