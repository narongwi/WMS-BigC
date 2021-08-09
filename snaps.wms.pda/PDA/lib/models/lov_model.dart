class Lov {
  String desc;
  String value;
  String valopnfirst;
  String valopnsecond;
  String valopnthird;
  String valopnfour;
  String icon;

  Lov({
    this.desc,
    this.value,
    this.valopnfirst,
    this.valopnsecond,
    this.valopnthird,
    this.valopnfour,
    this.icon,
  });

  factory Lov.fromJson(Map<String, dynamic> json) => Lov(
        desc: json["desc"],
        value: json["value"],
        valopnfirst: json["valopnfirst"],
        valopnsecond: json["valopnsecond"],
        valopnthird: json["valopnthird"],
        valopnfour: json["valopnfour"],
        icon: json["icon"],
      );

  Map<String, dynamic> toJson() => {
        "desc": desc,
        "value": value,
        "valopnfirst": valopnfirst,
        "valopnsecond": valopnsecond,
        "valopnthird": valopnthird,
        "valopnfour": valopnfour,
        "icon": icon,
      };
}
