class ReplenList {
  ReplenList({
    this.prepno,
    this.setno,
    this.prepzone,
    this.puqty,
  });

  String prepno;
  String setno;
  String prepzone;
  int puqty;

  factory ReplenList.fromJson(Map<String, dynamic> json) => ReplenList(
        prepno: json["prepno"],
        setno: json["setno"],
        prepzone: json["prepzone"],
        puqty: json["puqty"],
      );
}
