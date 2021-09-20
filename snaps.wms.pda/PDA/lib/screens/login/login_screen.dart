import 'dart:io';
import 'dart:ui';
import 'package:flutter/foundation.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/progress_component.dart';
import 'package:wms/screens/home/home_screen.dart';
import 'package:flutter/material.dart';
// import 'package:ota_update/ota_update.dart';
import '../../constants.dart';
import 'components/logo_banner.dart';
import 'services/login_service.dart';
// upgrade version lib
import 'package:package_info/package_info.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:progress_dialog/progress_dialog.dart';
import 'package:path_provider/path_provider.dart';
import 'package:open_file/open_file.dart';

class LoginScreen extends StatefulWidget {
  static const routeName = '/login';

  @override
  _LoginScreenState createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  // input text controller
  final usernameController = TextEditingController();
  final passwordController = TextEditingController();
  // input focus
  final usernameFocus = FocusNode();
  final passwordFocus = FocusNode();
  // Auth Service
  LoginService athService = new LoginService();

  //for show or hide progress bar
  bool isLoading = false;

  // flutter download
  PackageInfo _packageInfo = PackageInfo(
    appName: 'Unknown',
    packageName: 'Unknown',
    version: 'Unknown',
    buildNumber: 'Unknown',
  );
  ProgressDialog pr;

  @override
  void initState() {
    super.initState();
    _initPackageInfo();
  }

  Future<void> _initPackageInfo() async {
    final PackageInfo info = await PackageInfo.fromPlatform();
    setState(() {
      _packageInfo = info;
    });
  }

  Future<bool> updateDialog(String updateInfo, String version) async {
    return showDialog<bool>(
      barrierDismissible: false,
      context: context,
      builder: (context) {
        return AlertDialog(
          title: Text("New version detected"),
          content: Container(
            height: MediaQuery.of(context).size.height / 5,
            child: Column(
              children: [
                Text("Ready to update version $version "),
                SizedBox(height: 10),
                Text(
                  "- $updateInfo",
                  style: TextStyle(fontSize: 12, color: defaultColor),
                )
              ],
            ),
          ),
          actions: <Widget>[
            // new TextButton(
            //   child: Text("Cancel"),
            //   onPressed: () {
            //     Navigator.of(context).pop(false); // Close the dialog
            //   },
            // ),
            ConstrainedBox(
              constraints: BoxConstraints.tightFor(width: 120),
              child: ElevatedButton(
                style: ElevatedButton.styleFrom(
                    primary: successColor, // background
                    onPrimary: Colors.white),
                child: Text("Update"),
                onPressed: () async {
                  Navigator.of(context).pop(true);
                },
              ),
            ),
          ],
        );
      },
    );
  }

  @override
  void dispose() {
    usernameController.dispose();
    passwordController.dispose();
    usernameFocus.dispose();
    passwordFocus.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    // append progressbar widget
    return ProgressContainer(
      child: buildScreen(context),
      inAsyncCall: isLoading,
      opacity: 0.3,
    );
  }

  Future<void> alert(ctx, type, title, text) async {
    setState(() => isLoading = false);
    var alert = DialogAlert(
      title: title,
      content: text.replaceAll('Exception:', ''),
      type: type,
      onOk: () {},
    );
    await showDialog(
      context: ctx,
      builder: (BuildContext context) => alert,
    );
  }

  Future<String> downloadFile(String url, String fileName, String dir) async {
    File file;
    String filePath = '$dir/$fileName';

    pr = new ProgressDialog(
      context,
      type: ProgressDialogType.Normal,
      isDismissible: false,
      showLogs: false,
    );

    try {
      if (!pr.isShowing()) pr.show();
      HttpClient httpClient = new HttpClient();
      var request = await httpClient.getUrl(Uri.parse(url));
      var response = await request.close();
      if (response.statusCode == 200) {
        //clear old file
        if (await File(filePath).exists()) {
          await File(filePath).delete();
          print("delete old version");
        }
        var bytes = await consolidateHttpClientResponseBytes(response);
        filePath = '$dir/$fileName';
        file = File(filePath);
        await file.writeAsBytes(bytes);
        if (pr.isShowing()) pr.hide();
      } else {
        if (pr.isShowing()) pr.hide();
        filePath = 'Error code: ' + response.statusCode.toString();
      }
    } catch (ex) {
      if (pr.isShowing()) pr.hide();
      filePath = 'Can not fetch url';
    }
    return filePath;
    // https://gist.github.com/DeepMondal2001/c1fcf77dd9b8da053634b8ebc2f75ed9
    // var httpClient = http.Client();
    // var request = new http.Request('GET', Uri.parse(url));
    // var response = httpClient.send(request);
    // String dir = (await getApplicationDocumentsDirectory()).path;

    // List<List<int>> chunks = [];
    // int downloaded = 0;

    // response.asStream().listen((http.StreamedResponse r) {
    //   r.stream.listen((List<int> chunk) {
    //     // Display percentage of completion
    //     debugPrint('downloadPercentage: ${downloaded / r.contentLength * 100}');

    //     chunks.add(chunk);
    //     downloaded += chunk.length;
    //   }, onDone: () async {
    //     // Display percentage of completion
    //     debugPrint('downloadPercentage: ${downloaded / r.contentLength * 100}');

    //     // Save the file
    //     File file = new File('$dir/$fileName');
    //     final Uint8List bytes = Uint8List(r.contentLength);
    //     int offset = 0;
    //     for (List<int> chunk in chunks) {
    //       bytes.setRange(offset, offset + chunk.length, chunk);
    //       offset += chunk.length;
    //     }
    //     file.writeAsBytes(bytes);
    //     return;
    //   });
    // });
  }

  Future<void> login() async {
    try {
      if (usernameController.text.isEmpty) {
        Fluttertoast.showToast(msg: "please enter username", backgroundColor: colorWatermelon);
        // alert(context, "warning", "Warning", "please enter username");
      } else if (passwordController.text.isEmpty) {
        Fluttertoast.showToast(msg: "please enter password", backgroundColor: colorWatermelon);
        // alert(context, "warning", "Warning", "please enter password");
      } else {
        // hide keyboard focus
        FocusScope.of(context).requestFocus(new FocusNode());

        setState(() => isLoading = true);
        final uname = usernameController.text;
        final passw = passwordController.text;
        final res = await athService.verify(uname, passw);
        if (res.state == null) {
          alert(context, "warning", "Sign In", res.message);
        } else {
          final upgradeInfo = await athService.checkVersion(uname);
          setState(() => isLoading = false);

          print("local version : ${_packageInfo.version}");
          print("lasted version : ${upgradeInfo.version}");
          if (upgradeInfo.version != _packageInfo.version) {
            final confirm = await updateDialog(upgradeInfo.information, upgradeInfo.version);
            if ((confirm ?? true)) {
              if (Platform.isIOS) {
              } else if (Platform.isAndroid) {
                if (upgradeInfo.downloaduri.isNotEmpty) {
                  var per = await _checkPermission();
                  if (per != null && !per) {
                    return null;
                  }
                  String dir = await _apkLocalPath();
                  String uri = upgradeInfo.downloaduri;
                  String apkname = upgradeInfo.apkname;
                  final filePath = await downloadFile(uri, apkname, dir);

                  print(filePath);
                  _installApk(filePath);
                }
              }
            } else {
              Navigator.of(context).pushReplacement(
                MaterialPageRoute(builder: (context) => HomeScreen()),
              );
            }
          } else {
            Navigator.of(context).pushReplacement(
              MaterialPageRoute(builder: (context) => HomeScreen()),
            );
          }
        } //check login
      } // end validate
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<bool> _checkPermission() async {
    if (Platform.isAndroid) {
      PermissionStatus permission = await PermissionHandler().checkPermissionStatus(PermissionGroup.storage);
      if (permission != PermissionStatus.granted) {
        Map<PermissionGroup, PermissionStatus> permissions = await PermissionHandler().requestPermissions([PermissionGroup.storage]);
        if (permissions[PermissionGroup.storage] == PermissionStatus.granted) {
          return true;
        }
      } else {
        return true;
      }
    } else {
      return true;
    }
    return false;
  }

  Future<String> _apkLocalPath() async {
    String _downloadPath = (await _findLocalPath()) + Platform.pathSeparator + 'Download';
    final savedDir = Directory(_downloadPath);
    bool hasExisted = await savedDir.exists();
    if (!hasExisted) {
      savedDir.create();
    }

    return _downloadPath;
  }

  Future<String> _findLocalPath() async {
    final directory = Platform.isAndroid ? await getExternalStorageDirectory() : await getApplicationDocumentsDirectory();
    return directory?.path;
  }

  Future<Null> _installApk(String downloadPath) async {
    await OpenFile.open(downloadPath);
  }

  Widget buildScreen(BuildContext context) {
    // Style
    var textStyle = TextStyle(fontSize: 13);
    var size = MediaQuery.of(context).size;

    // Username input
    final usernameField = TextField(
      controller: usernameController,
      textAlign: TextAlign.center,
      focusNode: usernameFocus,
      onSubmitted: (value) => passwordFocus.requestFocus(),
      decoration: InputDecoration(
        hintText: "Username",
        hintStyle: textStyle,
      ),
    );
    // Password Input Filed
    final passwordField = TextField(
      onSubmitted: (value) async => login(),
      controller: passwordController,
      obscureText: true,
      focusNode: passwordFocus,
      textAlign: TextAlign.center,
      decoration: InputDecoration(
        hintText: "Password",
        hintStyle: textStyle,
      ),
    );

    var buttonLogin = ConstrainedBox(
      constraints: BoxConstraints.tightFor(width: size.width),
      child: ElevatedButton(
        child: Text("Sign In"),
        onPressed: () async {
          await login();
        },
      ),
    );

    var powerbyText = Text(
      "Powered By Snaps Solution",
      style: TextStyle(fontSize: 12, fontStyle: FontStyle.italic, color: secondaryColor),
    );
    // Body
    return WillPopScope(
      onWillPop: () async => false,
      child: Scaffold(
        body: SingleChildScrollView(
          child: Container(
            height: MediaQuery.of(context).size.height - 24,
            padding: const EdgeInsets.only(left: 50, right: 50),
            color: Colors.white,
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: <Widget>[
                WMSBanner(),
                SizedBox(height: 30.0),
                usernameField,
                SizedBox(height: 10.0),
                passwordField,
                SizedBox(height: 30.0),
                buttonLogin,
                SizedBox(height: 40.0),
                powerbyText,
                SizedBox(height: 5.0),
                Text(
                  "$appConfig version ${_packageInfo.version}+${_packageInfo.buildNumber}",
                  style: TextStyle(fontSize: 11, color: appConfig == "SIM" ? colorPoppy : defaultColor),
                )
              ],
            ),
          ),
        ),
      ),
    );
  }
}
