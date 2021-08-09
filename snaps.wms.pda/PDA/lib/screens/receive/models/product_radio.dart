class ProductRadio {
  String desc;
  String value;
  String valopnfirst;
  String valopnsecond;
  String valopnthird;
  String valopnfour;
  String icon;

  ProductRadio(
      {this.desc,
      this.value,
      this.valopnfirst,
      this.valopnsecond,
      this.valopnthird,
      this.valopnfour,
      this.icon});

  ProductRadio.fromJson(Map<String, dynamic> json) {
    desc = json['desc'];
    value = json['value'];
    valopnfirst = json['valopnfirst'];
    valopnsecond = json['valopnsecond'];
    valopnthird = json['valopnthird'];
    valopnfour = json['valopnfour'];
    icon = json['icon'];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['desc'] = this.desc;
    data['value'] = this.value;
    data['valopnfirst'] = this.valopnfirst;
    data['valopnsecond'] = this.valopnsecond;
    data['valopnthird'] = this.valopnthird;
    data['valopnfour'] = this.valopnfour;
    data['icon'] = this.icon;
    return data;
  }
}
