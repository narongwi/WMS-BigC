class Account {
  Account({
    this.orgcode,
    this.accntype,
    this.accncode,
    this.accnname,
    this.accnsurname,
    this.accnavartar,
    this.accsrole,
    this.email,
    this.site,
    this.depot,
  });

  String orgcode;
  String accntype;
  String accncode;
  String accnname;
  String accnsurname;
  String accnavartar;
  String accsrole;
  String email;
  String site;
  String depot;

  factory Account.fromJson(Map<String, dynamic> json) => Account(
        orgcode: json["orgcode"],
        accntype: json["accntype"],
        accncode: json["accncode"],
        accnname: json["accnname"],
        accnsurname: json["accnsurname"],
        accnavartar: json["accnavartar"],
        accsrole: json["accsrole"],
        email: json["email"],
        site: json["site"],
        depot: json["depot"],
      );

  Map<String, dynamic> toJson() => {
        "orgcode": orgcode,
        "accntype": accntype,
        "accncode": accncode,
        "accnname": accnname,
        "accnsurname": accnsurname,
        "accnavartar": accnavartar,
        "accsrole": accsrole,
        "email": email,
        "site": site,
        "depot": depot,
      };
}
