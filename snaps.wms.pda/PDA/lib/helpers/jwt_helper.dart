import 'dart:convert';

class JwtReader {
  String userName;
  String fullName;
  JwtReader({token}) {
    parseToken(token);
  }
  Map<String, dynamic> parseToken(String token) {
    if (token == null) return null;
    final parts = token.split('.');
    if (parts.length != 3) return null;

    final payload = parts[1];
    var normalized = base64Url.normalize(payload);

    var resp = utf8.decode(base64Url.decode(normalized));
    final payloadMap = json.decode(resp);
    if (payloadMap is! Map<String, dynamic>) {
      return null;
    }
    userName =
        payloadMap["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid"];

    fullName = payloadMap["unique_name"];

    return payloadMap;
  }

  // how to decode
  // _decodeJwt(String token) {
  //   Map<String, dynamic> tokenDecoded = parseJwt(token);
  //   String siteId = tokenDecoded['site_id'];
  // }

  // Map<String, dynamic> parseJwt(String token) {
  //   final parts = token.split('.');
  //   if (parts.length != 3) {
  //     throw Exception('invalid token');
  //   }

  //   final payload = _decodeBase64(parts[1]);
  //   final payloadMap = json.decode(payload);
  //   if (payloadMap is! Map<String, dynamic>) {
  //     throw Exception('invalid payload');
  //   }
  //   return payloadMap;
  // }

  // String _decodeBase64(String str) {
  //   String output = str.replaceAll('-', '+').replaceAll('_', '/');
  //   switch (output.length % 4) {
  //     case 0:
  //       break;
  //     case 2:
  //       output += '==';
  //       break;
  //     case 3:
  //       output += '=';
  //       break;
  //     default:
  //       throw Exception('Illegal base64url string!"');
  //   }
  //   return utf8.decode(base64Url.decode(output));
  // }
}
