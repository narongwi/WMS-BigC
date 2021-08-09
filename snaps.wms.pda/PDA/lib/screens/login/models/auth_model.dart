import 'dart:convert';

ResponseAth resultRequestFromJson(String str) =>
    ResponseAth.fromJson(json.decode(str));

String resultRequestToJson(ResponseAth data) => json.encode(data.toJson());

class ResponseAth {
  ResponseAth({
    this.message,
    this.reqid,
    this.state,
  });

  String message;
  String reqid;
  int state;

  factory ResponseAth.fromJson(Map<String, dynamic> json) => ResponseAth(
        message: json["message"],
        reqid: json["reqid"],
        state: json["state"],
      );

  Map<String, dynamic> toJson() => {
        "message": message,
        "reqid": reqid,
        "state": state,
      };
}
