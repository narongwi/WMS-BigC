// import 'dart:convert';
// import 'dart:io';
// import 'package:flutter/cupertino.dart';
// import 'package:flutter/material.dart';
// import 'package:flutter_downloader/flutter_downloader.dart';
// import 'package:open_file/open_file.dart';
// import 'package:package_info/package_info.dart';
// import 'package:path_provider/path_provider.dart';
// import 'package:url_launcher/url_launcher.dart';
// import 'package:permission_handler/permission_handler.dart';
// import 'package:http/http.dart' as http;
// import 'package:progress_dialog/progress_dialog.dart';
// import 'package:wms/screens/Upgrade/upgrade_model.dart';

// import '../../constants.dart';

// //Define the name of the apk and download progress dialog
// String apkName = 'snaps.wms.pda.apk';
// ProgressDialog pr;

// class Upgrade extends StatefulWidget {
//   Upgrade({Key key}) : super(key: key);

//   @override
//   _UpgradeState createState() => _UpgradeState();
// }

// class _UpgradeState extends State<Upgrade> {
//   @override
//   void initState() {
//     super.initState();
//     //Check for updates
//     checkUpdate(context);
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       appBar: AppBar(
//         title: Text('app upgrade'),
//       ),
//       body: Container(
//         alignment: Alignment.center,
//         child: Text(
//           'Checking...',
//           style: TextStyle(
//             fontSize: 16,
//           ),
//         ),
//       ),
//     );
//   }

//   ///Check for updates
//   Future<void> checkUpdate(BuildContext context) async {
//     //Android, need to download apk package
//     if (Platform.isAndroid) {
//       print('is android');
//       PackageInfo packageInfo = await PackageInfo.fromPlatform();
//       String localVersion = packageInfo.version;
//       //The dio is used here, and the httpGet method is encapsulated to obtain the latest app version information in the server
//       final response = await http.get("$pdaApiUrl/download/apk");
//       print("Apk Download Radio ==> ${response.statusCode}");
//       if (response.statusCode == 200) {
//         final upgradInfo = UpgradInfo.fromJson(jsonDecode(response.body));
//         print('Local version: ' +
//             localVersion +
//             ',The latest version: ' +
//             upgradInfo.downloaduri);

//         int c = upgradInfo.version.compareTo(localVersion);
//         //If the server version is greater than the local version, you will be prompted to update
//         if (c == 1) {
//           showUpdate(context, upgradInfo.version, upgradInfo.information,
//               upgradInfo.downloaduri);
//         }
//       } else {
//         print("upgrade failed => ${response.body}");
//       }
//     }

//     //Ios, can only jump to AppStore, just use url_launcher directly
//     //Android can also use this method, it will jump to the mobile browser to download
//     // if (Platform.isIOS) {
//     //   print('is ios');
//     //   final url =
//     //       "https://itunes.apple.com/cn/app/id1380512641"; // Just replace the number after id with your application id
//     //   if (await canLaunch(url)) {
//     //     await launch(url, forceSafariVC: false);
//     //   } else {
//     //     throw 'Could not launch $url';
//     //   }
//     // }
//   }

//   ///2. Show updated content
//   Future<void> showUpdate(
//       BuildContext context, String version, String data, String url) async {
//     return showDialog<void>(
//       context: context,
//       barrierDismissible: true,
//       builder: (BuildContext context) {
//         return CupertinoAlertDialog(
//           title: Text('New version detected v$version'),
//           content: Text('Do you want to update to the latest version?'),
//           actions: <Widget>[
//             new TextButton(
//               child: Text("Talking next time"),
//               onPressed: () {
//                 Navigator.of(context).pop(); // Close the dialog
//               },
//             ),
//             new TextButton(
//               child: Text("update immediately"),
//               onPressed: () => doUpdate(context, version, url),
//             ),
//           ],
//         );
//       },
//     );
//   }

//   ///3. Perform the update operation
//   doUpdate(BuildContext context, String version, String url) async {
//     //Close the update content prompt box
//     Navigator.pop(context);
//     //Get permission
//     var per = await checkPermission();
//     if (per != null && !per) {
//       return null;
//     }
//     //Start downloading apk
//     executeDownload(context, url);
//   }

//   ///4. Check if there is permission
//   Future<bool> checkPermission() async {
//     //Check if there is already read and write memory permissions
//     PermissionStatus status = await PermissionHandler()
//         .checkPermissionStatus(PermissionGroup.storage);
//     print(status);

//     //Determine if you don't have read and write permissions, apply for permission
//     if (status != PermissionStatus.granted) {
//       var map = await PermissionHandler()
//           .requestPermissions([PermissionGroup.storage]);
//       if (map[PermissionGroup.storage] != PermissionStatus.granted) {
//         return false;
//       }
//     }
//     return true;
//   }

//   ///5. Download apk
//   Future<void> executeDownload(BuildContext context, String url) async {
//     //Display download progress dialog during download
//     pr = new ProgressDialog(context,
//         type: ProgressDialogType.Download, isDismissible: true, showLogs: true);
//     if (!pr.isShowing()) {
//       pr.show();
//     }
//     //apk storage path
//     final path = await _apkLocalPath;
//     File file = File(path + '/' + apkName);
//     if (await file.exists()) await file.delete();

//     //download
//     final taskId = await FlutterDownloader.enqueue(
//         url: url, //Download the latest apk network address
//         savedDir: path,
//         fileName: apkName,
//         showNotification: true,
//         openFileFromNotification: true);

//     FlutterDownloader.registerCallback((id, status, progress) {
//       if (status == DownloadTaskStatus.running) {
//         pr.update(
//             progress: progress.toDouble(),
//             message: "Downloading, please wait...");
//       }
//       if (status == DownloadTaskStatus.failed) {
//         if (pr.isShowing()) {
//           pr.hide();
//         }
//       }
//       if (taskId == id && status == DownloadTaskStatus.complete) {
//         if (pr.isShowing()) {
//           pr.hide();
//         }
//         _installApk();
//       }
//     });
//   }

//   //6. Install the app
//   Future<Null> _installApk() async {
//     String path = await _apkLocalPath;
//     await OpenFile.open(path + '/' + apkName);
//   }

//   // Get apk storage address (external path)
//   Future<String> get _apkLocalPath async {
//     final directory = await getExternalStorageDirectory();
//     return directory.path;
//   }
// }
