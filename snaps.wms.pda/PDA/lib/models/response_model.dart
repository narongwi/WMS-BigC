class ResponseData {
  ResponseData({this.success, this.message, this.data, this.params});
  bool success;
  String message;
  dynamic data;
  dynamic params;

  factory ResponseData.fromJson(Map<String, dynamic> json) {
    return ResponseData(
        success: json["success"],
        message: json["message"],
        data: json["data"],
        params: json["params"]);
  }
}
