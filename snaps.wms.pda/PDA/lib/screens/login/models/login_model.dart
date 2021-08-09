class LoginModel {
  String _accncode;
  String _password;
  String _email;
  String _lang;
  LoginModel(this._accncode, this._password);

  LoginModel.map(dynamic obj) {
    this._accncode = obj["accncode"];
    this._password = obj["password"];
    this._email = obj["email"];
    this._lang = obj["lang"];
  }

  String get username => _accncode;
  String get password => _password;
  String get email => _email;
  String get lang => _lang;

  Map<String, dynamic> toMap() {
    var map = new Map<String, dynamic>();
    map["accncode"] = _accncode;
    map["password"] = _password;
    map["email"] = _email;
    map["lang"] = _lang;
    return map;
  }
}
