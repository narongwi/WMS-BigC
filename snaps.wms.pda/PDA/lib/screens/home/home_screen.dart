import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:package_info/package_info.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/progress_component.dart';
import 'package:wms/constants.dart';
import 'package:wms/screens/Distribution/basecose_screen.dart';
import 'package:wms/screens/Distribution/distribute_screen.dart';
import 'package:wms/screens/home/models/permission.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:wms/screens/home/services/profile_service.dart';
import 'package:wms/screens/login/login_screen.dart';
import 'package:wms/screens/loosepick/loosepick_screen.dart';
import 'package:wms/screens/receive/receive_screen.dart';
import 'package:wms/screens/stockcount/count_screen.dart';
import 'package:wms/screens/tasks/fullpallet_screen.dart';
import 'package:wms/screens/tasks/putaway_screen.dart';
import 'package:wms/screens/tasks/replenish_screen.dart';
import 'package:wms/screens/tasks/transfer_screen.dart';
import 'comonents/buttonItem.dart';

class HomeScreen extends StatefulWidget {
  static const routeName = '/home';
  HomeScreen({Key key}) : super(key: key);
  @override
  _HomeScreenState createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final profileService = ProfileService();
  Profiles profile = Profiles();
  List<Permission> permissions = <Permission>[];
  Permission receivePermission = Permission();
  Permission putawayPermission = Permission();
  Permission replensPermission = Permission();
  Permission transferPermission = Permission();
  Permission palletpickPermission = Permission();
  Permission loosepickPermission = Permission();
  Permission distributePermission = Permission();
  Permission basecosePermission = Permission();
  Permission countPermission = Permission();

  bool isLoading = false;
  // flutter download
  PackageInfo _packageInfo = PackageInfo(
    appName: 'Unknown',
    packageName: 'Unknown',
    version: 'Unknown',
    buildNumber: 'Unknown',
  );
  Future<void> _initPackageInfo() async {
    final PackageInfo info = await PackageInfo.fromPlatform();
    setState(() {
      _packageInfo = info;
    });
  }

  // get account profile
  Future<void> getProfile() async {
    isLoading = true;
    profileService.getAccount().then((res) {
      setState(() {
        isLoading = false;
        profile = res;

        permissions = profile.roleaccs.modules.first.permission;
        receivePermission = permissions.singleWhere(
          (e) => e.objcode == "wms.mnureceive",
          orElse: () => Permission(objname: "Receive"),
        );
        putawayPermission = permissions.singleWhere(
          (e) => e.objcode == "wms.mnuputaway",
          orElse: () => Permission(objname: "Putaway"),
        );
        replensPermission = permissions.singleWhere(
          (e) => e.objcode == "wms.mnureplen",
          orElse: () => Permission(objname: "Replenishment"),
        );
        transferPermission = permissions.singleWhere(
          (e) => e.objcode == "wms.mnutransfer",
          orElse: () => Permission(objname: "Transfer"),
        );
        palletpickPermission = permissions.singleWhere(
          (e) => e.objcode == "wms.mnupalletpick",
          orElse: () => Permission(objname: "Pallet Pick"),
        );
        loosepickPermission = permissions.singleWhere(
          (e) => e.objcode == "wms.mnuloospick",
          orElse: () => Permission(objname: "Loose Pick"),
        );
        distributePermission = permissions.singleWhere(
          (e) => e.objcode == "wms.mnudistribute",
          orElse: () => Permission(objname: "Distribution"),
        );
        basecosePermission = permissions.singleWhere(
          (e) => e.objcode == "wms.mnubaseclose",
          orElse: () => Permission(objname: "Base Closing"),
        );
        countPermission = permissions.singleWhere(
          (e) => e.objcode == "wms.mnustockcnt",
          orElse: () => Permission(objname: "Stock Count"),
        );
        // for (var i = 0; i < permissions.length; i++) {
        //   print(permissions[i].objcode);
        //   print(permissions[i].objname);
        // }
      });
    }).catchError((e) {
      setState(() {
        isLoading = false;
        alertMessage(context, "Account", e.toString());
      });
    });
  }

  // custom alert popup
  Future<void> alertMessage(ctx, title, text) async {
    var alert = DialogAlert(title: title, content: text, onOk: () {});
    await showDialog(context: ctx, builder: (BuildContext context) => alert);
  }

  @override
  void initState() {
    super.initState();
    _initPackageInfo();
    getProfile();
  }

  @override
  Widget build(BuildContext context) {
    return ProgressContainer(
      child: buildScreen(context),
      inAsyncCall: isLoading,
      opacity: 0.3,
    );
  }

  Widget buildScreen(BuildContext context) {
    return WillPopScope(
      onWillPop: () async => false,
      child: Scaffold(
        appBar: buildAppBar(),
        body: Padding(
          padding: const EdgeInsets.symmetric(horizontal: defaultPadding),
          child: Column(
            children: [
              profileWidget(),
              // buildWH(),
              Expanded(
                child: GridView.count(
                  shrinkWrap: true,
                  primary: false,
                  padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 10),
                  crossAxisSpacing: 10,
                  mainAxisSpacing: 10,
                  crossAxisCount: 3,
                  children: <Widget>[
                    ButtonItem(
                      itemText: "${receivePermission.objname ?? 'waiting'}",
                      itemIcon: CupertinoIcons.bag_badge_plus,
                      isEnable: receivePermission.isenable == 1 ? true : false,
                      onPress: () {
                        Navigator.pushNamed(
                          context,
                          ReceiveScreen.routeName,
                          arguments: profile,
                        );
                      },
                    ),
                    ButtonItem(
                      itemText: "${putawayPermission.objname ?? 'waiting'}",
                      itemIcon: CupertinoIcons.square_arrow_down_on_square,
                      isEnable: putawayPermission.isenable == 1 ? true : false,
                      onPress: () {
                        Navigator.pushNamed(
                          context,
                          PutawayScreen.routeName,
                          arguments: profile,
                        );
                      },
                    ),
                    ButtonItem(
                      itemText: "${replensPermission.objname ?? 'waiting'}",
                      itemIcon: CupertinoIcons.square_grid_2x2,
                      isEnable: replensPermission.isenable == 1 ? true : false,
                      onPress: () {
                        Navigator.pushNamed(
                          context,
                          ReplenScreen.routeName,
                          arguments: profile,
                        );

                        // Navigator.pushNamed(
                        //     context, ReplenishmentScreen.routeName);
                      },
                    ),
                    ButtonItem(
                      itemText: "${transferPermission.objname ?? 'waiting'}",
                      itemIcon: CupertinoIcons.arrow_up_arrow_down_square,
                      isEnable: transferPermission.isenable == 1 ? true : false,
                      onPress: () {
                        Navigator.pushNamed(
                          context,
                          TranferStockScreen.routeName,
                          arguments: profile,
                        );
                      },
                    ),
                    ButtonItem(
                      itemText: "${palletpickPermission.objname ?? 'waiting'}",
                      itemIcon: CupertinoIcons.cube_box,
                      isEnable: palletpickPermission.isenable == 1 ? true : false,
                      onPress: () {
                        Navigator.pushNamed(
                          context,
                          ApproachScreen.routeName,
                          arguments: profile,
                        );
                      },
                    ),
                    ButtonItem(
                      itemText: "${loosepickPermission.objname ?? 'waiting'}",
                      itemIcon: CupertinoIcons.circle_grid_hex,
                      isEnable: loosepickPermission.isenable == 1 ? true : false,
                      onPress: () {
                        Navigator.pushNamed(
                          context,
                          LoosePickScreen.routeName,
                          arguments: profile,
                        );
                      },
                    ),
                    ButtonItem(
                      itemText: "${distributePermission.objname ?? 'waiting'}",
                      itemIcon: CupertinoIcons.globe,
                      isEnable: distributePermission.isenable == 1 ? true : false,
                      onPress: () {
                        Navigator.pushNamed(
                          context,
                          DistributeScreen.routeName,
                          arguments: profile,
                        );
                      },
                    ),
                    ButtonItem(
                      itemText: "${basecosePermission.objname ?? 'waiting'}",
                      itemIcon: CupertinoIcons.dot_radiowaves_left_right,
                      isEnable: basecosePermission.isenable == 1 ? true : false,
                      onPress: () {
                        Navigator.pushNamed(context, BaseClosedPage.routeName, arguments: profile);
                      },
                    ),
                    ButtonItem(
                      itemText: "${countPermission.objname ?? 'waiting'}",
                      itemIcon: CupertinoIcons.gauge,
                      isEnable: countPermission.isenable == 1 ? true : false,
                      onPress: () {
                        Navigator.pushNamed(context, CountScreen.routeName);
                      },
                    ),
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.all(10),
                child: Text(
                  "Powered By Snaps Solutions",
                  style: TextStyle(fontSize: 12, fontStyle: FontStyle.italic, color: defaultColor),
                ),
              ),
              Padding(
                padding: const EdgeInsets.only(bottom: 10),
                child: Text(
                  "$appConfig version ${_packageInfo.version}+${_packageInfo.buildNumber}",
                  style: TextStyle(fontSize: 12, fontStyle: FontStyle.italic, color: appConfig == "SIM" ? colorPoppy : defaultColor),
                ),
              )
            ],
          ),
        ),
      ),
    );
  }

  // Row buildWH() {
  //   return Row(
  //     mainAxisAlignment: MainAxisAlignment.center,
  //     crossAxisAlignment: CrossAxisAlignment.center,
  //     children: [
  //       WHCard(
  //           label: "site:", value: "${profile.site ?? ""}", color: Colors.red),
  //       SizedBox(width: 10),
  //       WHCard(
  //           label: "depot:",
  //           value: "${profile.depot ?? ""}",
  //           color: Colors.orange),
  //     ],
  //   );
  // }

  Card profileWidget() {
    return Card(
      elevation: 0,
      margin: EdgeInsets.zero,
      color: iconBgColor,
      child: Column(
        children: [
          // ListTile(
          // leading: Container(
          //   padding: EdgeInsets.all(5),
          //   decoration: BoxDecoration(
          //     color: Colors.white,
          //     borderRadius: BorderRadius.all(Radius.circular(50.0)),
          //   ),
          //   child: ClipRRect(
          //     borderRadius: BorderRadius.all(Radius.circular(50.0)),
          //     child: Image.asset("assets/images/bigc-logo-small.jpg"),
          //   ),
          // ),
          //   title: Text(
          //     "Hi ${profile.accncode ?? ""}",
          //     style: TextStyle(color: Colors.blue, fontSize: 18),
          //   ),
          //   subtitle: Text(
          //     'Site: ${profile.site} Depot: ${profile.depot}',
          //     style: TextStyle(fontSize: 12),
          //   ),
          //   trailing: Icon(
          //     CupertinoIcons.circle_fill,
          //     size: 12,
          //     color: Color(0xffBBCE00),
          //   ),
          // ),
          Row(
            children: [
              Expanded(
                child: ListTile(
                  title: Text(
                    '${profile.accncode ?? 'waiting'}',
                    textAlign: TextAlign.center,
                    overflow: TextOverflow.ellipsis,
                    style: TextStyle(color: dangerColor, fontSize: 13, fontWeight: FontWeight.bold),
                  ),
                  subtitle: Text('${profile.cfgcode ?? 'waiting'}', overflow: TextOverflow.ellipsis, textAlign: TextAlign.center, style: TextStyle(fontSize: 12)),
                ),
              ),
              Expanded(
                child: ListTile(
                  title: Text(
                    '${profile.site ?? 'waiting'}',
                    textAlign: TextAlign.center,
                    style: TextStyle(color: dangerColor, fontSize: 13, fontWeight: FontWeight.bold),
                  ),
                  subtitle: Text('Site', textAlign: TextAlign.center, style: TextStyle(fontSize: 12)),
                ),
              ),
              Expanded(
                child: ListTile(
                  title: Text(
                    '${profile.depot ?? 'waiting'}',
                    textAlign: TextAlign.center,
                    style: TextStyle(color: dangerColor, fontSize: 13, fontWeight: FontWeight.bold),
                  ),
                  subtitle: Text('Depot', textAlign: TextAlign.center, style: TextStyle(fontSize: 12)),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  AppBar buildAppBar() {
    return AppBar(
      leadingWidth: 50,
      leading: Padding(
        padding: const EdgeInsets.only(left: 20.0),
        child: CircleAvatar(
          radius: 18,
          child: ClipOval(
            child: Image.asset("assets/images/bigc-logo-small.jpg"),
          ),
        ),
      ),
      // leading: Container(
      //   padding: EdgeInsets.only(left: 10),
      //   child: ClipRRect(
      //     borderRadius: BorderRadius.all(Radius.circular(50.0)),
      //     child: Image.asset("assets/images/bigc-logo-small.jpg"),
      //   ),
      // ),
      // leading: Padding(
      //   padding: const EdgeInsets.only(left: 10),
      //   child: IconButton(
      //     icon: Icon(CupertinoIcons.chat_bubble),
      //     onPressed: null,
      //   ),
      // ),
      // title: Text("Warehouse Management System"),
      title: Text(
        "Welcome",
        style: TextStyle(fontWeight: FontWeight.bold),
      ),
      // centerTitle: true,
      actions: [
        // IconButton(
        //   visualDensity: VisualDensity(horizontal: -2.0, vertical: -2.0),
        //   padding: EdgeInsets.zero,
        //   icon: Icon(CupertinoIcons.bell),
        //   onPressed: () async {},
        // ),
        IconButton(
          // visualDensity: VisualDensity(horizontal: -2.0, vertical: -2.0),
          // padding: EdgeInsets.zero,
          icon: Icon(CupertinoIcons.info_circle),
          onPressed: () async {
            Fluttertoast.showToast(
              msg: "Warehouse Management System \nversion ${_packageInfo.version} build number ${_packageInfo.buildNumber}",
              backgroundColor: colorBlue,
            );
          },
        ),
        IconButton(
          // visualDensity: VisualDensity(horizontal: -2.0, vertical: -2.0),
          // padding: EdgeInsets.zero,
          icon: Icon(CupertinoIcons.lock_circle),
          onPressed: () async {
            profileService.logout().then((value) {
              Navigator.of(context).pushReplacement(MaterialPageRoute(builder: (context) => LoginScreen()));
            });
          },
        ),
        SizedBox(
          width: 20,
        )
      ],
    );
  }
}
