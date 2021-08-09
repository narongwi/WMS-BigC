class PrepList {
  PrepList({
    this.prepno,
    this.setno,
    this.prepzone,
    this.puqty,
  });

  String prepno;
  String setno;
  String prepzone;
  int puqty;

  factory PrepList.fromJson(Map<String, dynamic> json) => PrepList(
        prepno: json["prepno"],
        setno: json["setno"],
        prepzone: json["prepzone"],
        puqty: json["puqty"],
      );
}
