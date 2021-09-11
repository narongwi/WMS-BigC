import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:wms/components/progress_component.dart';
import 'package:wms/models/lov_model.dart';
import 'package:wms/screens/stockcount/services/count_services.dart';
import '../../constants.dart';
import 'models/countline_model.dart';
import 'models/countplan_model.dart';

class SheetTab extends StatefulWidget {
  static Countplan countPlan = Countplan();
  static List<Countline> countSheet = <Countline>[];
  static List<Lov> lov = <Lov>[];
  @override
  State<StatefulWidget> createState() {
    return _SheetTab();
  }
}

class _SheetTab extends State<SheetTab> with SingleTickerProviderStateMixin {
  final CountServices sv = CountServices();
  bool isLoading = false;

  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
  }

  Future<void> refreshSheet() async {
    try {
      setState(() => isLoading = true);
      if ((SheetTab.countPlan.plancode ?? "") == "") return;
      var _lines = await sv.countLine(SheetTab.countPlan);
      if (_lines.length > 0) {
        // _lines.forEach((element) {
        //   element.unitcount = decodeUnit(element.unitcount);
        // });

        // initail count sheet
        setState(() {
          isLoading = false;
          SheetTab.countSheet = _lines;
        });
      }
    } catch (e) {
      setState(() => isLoading = false);
      Fluttertoast.showToast(
        msg: e.toString(),
        backgroundColor: colorStem,
      );
    }
  }

  String decodeUnit(String unitcode) {
    print("decodeUnit==>$unitcode");
    if (unitcode == null) return "";
    try {
      return SheetTab.lov
          .firstWhere(
            (e) => e.value == unitcode,
            orElse: () => Lov(),
          )
          .desc;
    } catch (e) {
      Fluttertoast.showToast(
        msg: e.toString(),
        backgroundColor: colorStem,
      );
      return "";
    }
  }

  String decTflow(String tflow) {
    try {
      switch (tflow) {
        case "CC":
          return "Cycle count";
        case "CT":
          return "Stock take";
        default:
          return tflow;
      }
    } catch (e) {
      Fluttertoast.showToast(
        msg: e.toString(),
        backgroundColor: colorStem,
      );
      return tflow;
    }
  }

  Future<bool> clearList() async {
    SheetTab.countPlan = null;
    SheetTab.lov.clear();
    SheetTab.countSheet.clear();
    Navigator.pop(context);
    return true;
  }

  IconData planIcon(String tflow) {
    try {
      switch (tflow) {
        case "XX":
          return CupertinoIcons.minus_circle;
        case "ED":
          return CupertinoIcons.checkmark_circle;
        default:
          return CupertinoIcons.hourglass_bottomhalf_fill;
      }
    } catch (e) {
      return CupertinoIcons.clock;
    }
  }

  @override
  Widget build(BuildContext context) {
    // get profile arguments
    // profile = ModalRoute.of(context).settings.arguments as Profiles;
    return WillPopScope(
      onWillPop: () async => await clearList(),
      child: ProgressContainer(
        child: buildScreen(context),
        inAsyncCall: isLoading,
        opacity: 0.3,
      ),
    );
  }

  Widget buildScreen(BuildContext context) {
    return WillPopScope(
      onWillPop: () async => await clearList(),
      child: Scaffold(
        appBar: AppBar(
          automaticallyImplyLeading: false,
          // leadingWidth: 30,
          // leading: IconButton(
          //   onPressed: () async {
          //     Navigator.pop(context);
          //   },
          //   icon: Icon(CupertinoIcons.info, size: 20),
          // ),
          title: Padding(
            padding: const EdgeInsets.only(left: 5),
            child: Text(
              (SheetTab.countPlan?.plancode ?? "").isEmpty ? "No Plan" : "${SheetTab.countPlan?.plancode ?? ""} ${SheetTab.countPlan?.planname ?? ""}",
              overflow: TextOverflow.ellipsis,
              style: TextStyle(
                color: primaryColor,
              ),
            ),
          ),
          actions: <Widget>[
            IconButton(
              icon: const Icon(CupertinoIcons.refresh_circled),
              onPressed: () async {
                await refreshSheet();
              },
            ),
          ],
        ),
        body: ListView.separated(
          separatorBuilder: (BuildContext context, int index) => const Divider(),
          itemCount: SheetTab.countSheet.length,
          itemBuilder: (context, index) {
            return Row(
              children: [
                Container(
                  margin: EdgeInsets.zero,
                  width: 105,
                  child: ListTile(
                    contentPadding: EdgeInsets.only(left: 15),
                    // leading: CircleAvatar(
                    //   radius: 10,
                    //   backgroundColor: colorLeafyGreen,
                    //   child: Text(
                    //     '${SheetTab.countSheet[index].locctype}',
                    //     style: TextStyle(
                    //       fontSize: 12,
                    //     ),
                    //   ),
                    // ),
                    title: Text(
                      "${SheetTab.countSheet[index].loccode}",
                      textAlign: TextAlign.left,
                      style: TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.bold,
                        color: primaryColor,
                      ),
                    ),
                    subtitle: Text(
                      "${SheetTab.countSheet[index].locctype} : ${(SheetTab.countSheet[index].cnflow.isNotEmpty ? SheetTab.countSheet[index].cnarticle : SheetTab.countSheet[index].starticle)}" + (SheetTab.countSheet[index].starticle.isEmpty ? "" : "-${SheetTab.countSheet[index].stlv}"),
                      textAlign: TextAlign.left,
                      style: TextStyle(fontSize: 11, color: colorBlue),
                    ),
                  ),
                ),
                Expanded(
                  flex: 2,
                  child: ListTile(
                    contentPadding: EdgeInsets.only(left: 5),
                    title: Text(
                      "${(SheetTab.countSheet[index].cnflow.isNotEmpty ? SheetTab.countSheet[index].cnbarcode : SheetTab.countSheet[index].stbarcode)} ",
                      overflow: TextOverflow.ellipsis,
                      style: TextStyle(fontSize: 13, color: colorSeeds),
                    ),
                    subtitle: Text(
                      "${SheetTab.countSheet[index].productdesc}",
                      overflow: TextOverflow.ellipsis,
                      style: TextStyle(fontSize: 11, color: colorBlue),
                    ),
                  ),
                ),
                Container(
                  width: 80,
                  child: SheetTab.countSheet[index].cnflow.isNotEmpty
                      ? ListTile(
                          contentPadding: EdgeInsets.only(left: 5),
                          title: Text(
                            "${SheetTab.countSheet[index].cnqtypu}",
                            textAlign: TextAlign.center,
                            style: TextStyle(
                              fontSize: 13,
                              color: colorPoppy,
                            ),
                          ),
                          subtitle: Text(
                            "Qty",
                            textAlign: TextAlign.center,
                            style: TextStyle(fontSize: 11, color: colorBlue),
                          ),
                        )
                      : Text(''),
                ),
              ],
            );
          },
        ),
        // body: ListView(
        //   padding: const EdgeInsets.all(8),
        //   children: <Widget>[
        //     ListTile(
        //       title: Text("Ballot"),
        //       leading: Icon(Icons.ballot),
        //       trailing: Icon(
        //         Icons.star,
        //       ),
        //     )
        //   ],
        // ),
      ),
    );
  }
}
