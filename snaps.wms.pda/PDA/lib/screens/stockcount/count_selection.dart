import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:intl/intl.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/input_decoration.dart';
import 'package:wms/components/progress_component.dart';
import 'package:wms/constants.dart';
import 'package:wms/models/lov_model.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:wms/screens/receive/models/product_active.dart';
import 'package:wms/screens/stockcount/services/count_services.dart';
import 'package:wms/services/lov_services.dart';

import 'models/countplan_model.dart';
import 'models/counttask_model.dart';

class CountSelection extends StatefulWidget {
  static const routeName = '/stockcount';

  @override
  State<StatefulWidget> createState() {
    return _CountSelection();
  }
}

class _CountSelection extends State<CountSelection> with SingleTickerProviderStateMixin {
  TabController _tabController;
  String _selectTaskCode;
  String _selectPlanCode;
  String _selectCountType;
  int selectedIndex;
  final CountServices sv = CountServices();
  bool isLoading = false;
  bool isScanHU = false;

  Profiles profile = Profiles();
  Product product = Product();
  List<CountTask> tasks = <CountTask>[];
  List<Countplan> plans = <Countplan>[];
  List<Lov> lov = <Lov>[];
  TextEditingController _planCodeController = TextEditingController();

  final locfocusNode = FocusNode();
  final hufocusNode = FocusNode();
  final barfocusNode = FocusNode();
  final batchfocusNode = FocusNode();
  final expfocusNode = FocusNode();
  final mfgfocusNode = FocusNode();
  final qtyfocusNode = FocusNode();
  String filterPlancode = "";
  String filterValueText = "";

  Future<void> countUnit() async {
    try {
      LovService lovSerivce = new LovService();
      lov = await lovSerivce.getUnit();
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // list all count task
  Future<void> getTask() async {
    try {
      setState(() => isLoading = true);
      var _task = await sv.countTask();
      setState(() {
        isLoading = false;
        tasks = _task;
      });
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // list plan of task selection
  Future<void> getplan(CountTask sltask) async {
    try {
      if (sltask == null) {
        alert(context, "warning", "Warning", "please select tasks");
        // _controller.animateTo(0);
      } else {
        setState(() {
          _selectCountType = decTflow(sltask.counttype);
          _selectTaskCode = sltask.countcode;
          _selectPlanCode = "";
          filterPlancode = "";
          filterValueText = "";
          _planCodeController.text = "";
          isLoading = true;
        });
        var _plans = await sv.countList(_selectTaskCode);

        setState(() {
          plans = _plans;
        });
        await new Future.delayed(new Duration(seconds: 1));
        setState(() => isLoading = false);
        _tabController.animateTo(1);
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> getplanByCode() async {
    print("_selectTaskCode : $_selectTaskCode");
    if (_selectTaskCode.isNotEmpty) {
      setState(() => isLoading = true);
      var _plans = await sv.countList(_selectTaskCode);

      print("_plans.length : ${_plans.length}");

      if (filterPlancode.isEmpty) {
        print("No Filter");
        setState(() {
          isLoading = false;
          plans = _plans;
        });
      } else {
        print("Filter");
        print("filterPlancode : $filterPlancode");
        final filterPlans = _plans.where((element) => element.plancode == filterPlancode).toList();
        print("filterPlans.length : ${filterPlans.length}");
        setState(() {
          isLoading = false;
          plans = filterPlans;
        });
      }
    }
  }

// list plan of task selection
  Future<void> getlines(Countplan slplan) async {
    try {
      if (slplan.plancode != null) {
        //setState(() => isLoading = true);

        // var _lines = await sv.countLine(slplan);
        // if (_lines.length > 0) {
        //   _lines.forEach((element) {
        //     element.unitcount = decodeUnit(element.unitcount);
        //   });
        // }

        if (slplan.tflow == "ED") {
          Fluttertoast.showToast(
            msg: "this plan is alredy completed",
            backgroundColor: colorLeafyGreen,
          );
        } else if (slplan.tflow == "XX") {
          Fluttertoast.showToast(
            msg: "this plan is cancelled",
            backgroundColor: colorLeafyGreen,
          );
        } else {
          slplan.countType = _selectCountType;

          print(_selectCountType);
          Navigator.pop(context, slplan);
        }
        // setState(() => isLoading = false);
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  String _formatDate(dynamic _date) {
    if (_date == null || _date == '') {
      return "";
    } else {
      return DateFormat('dd/MM/yyyy').format(_date);
    }
  }

  Future<void> refresh() async {
    setState(() {
      isLoading = false;
      //tasks = <CountTask>[];
      plans = <Countplan>[];
      _selectTaskCode = "";
      _selectPlanCode = "";
      _tabController.animateTo(0);
      filterPlancode = "";
      filterValueText = "";
      _planCodeController.text = "";
    });

    await getTask();
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
      alert(context, "error", "Error", e.toString());
      return tflow;
    }
  }

  String decodeUnit(String unitcode) {
    print("decodeUnit==>$unitcode");
    if (unitcode == null) return "";
    try {
      return lov
          .firstWhere(
            (e) => e.value == unitcode,
            orElse: () => Lov(),
          )
          .desc;
    } catch (e) {
      alert(context, "error", "Error", e.toString());
      return "";
    }
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

  Color planState(String tflow) {
    try {
      switch (tflow) {
        case "XX":
          return colorSeeds;
        case "ED":
          return colorLeafyGreen;
        default:
          return Colors.grey;
      }
    } catch (e) {
      return Colors.grey;
    }
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

  @override
  void initState() {
    super.initState();

    _tabController = TabController(length: 2, vsync: this);
    selectedIndex = 0;
    _tabController.addListener(() {
      setState(() {
        selectedIndex = _tabController.index;
      });
    });
    countUnit();
    getTask();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    // get profile arguments
    profile = ModalRoute.of(context).settings.arguments as Profiles;

    return ProgressContainer(
      child: buildScreen(context),
      inAsyncCall: isLoading,
      opacity: 0.3,
    );
  }

  Future<void> _displayTextInputDialog(BuildContext context) async {
    return showDialog(
        context: context,
        builder: (context) {
          return AlertDialog(
            title: Text('Plan Filter'),
            content: TextField(
              onSubmitted: (value) async {
                setState(() {
                  filterPlancode = filterValueText;
                  Navigator.pop(context);
                });

                // filter plan
                await getplanByCode();
              },
              onChanged: (value) {
                setState(() {
                  filterValueText = value;
                });
              },
              controller: _planCodeController,
              decoration: Txtheme.deco(
                label: "Plan Code :  ",
              ),
            ),
            actions: <Widget>[
              ElevatedButton.icon(
                onPressed: () {
                  setState(() {
                    _planCodeController.text = "";
                    filterPlancode = "";
                    filterValueText = "";
                    // Navigator.pop(context);
                  });
                },
                label: Text("Clear", style: TextStyle(fontWeight: FontWeight.bold)),
                icon: Icon(
                  CupertinoIcons.multiply,
                  color: Colors.grey,
                  size: 20.0,
                ),
                style: ElevatedButton.styleFrom(
                  elevation: 0,
                  primary: Colors.white, // background
                  onPrimary: Colors.grey, // foreground
                  shape: RoundedRectangleBorder(
                    borderRadius: new BorderRadius.circular(8.0),
                  ),
                ),
              ),
              ElevatedButton.icon(
                onPressed: () async {
                  setState(() {
                    filterPlancode = filterValueText;
                    Navigator.pop(context);
                  });

                  // filter plan
                  await getplanByCode();
                },
                label: Text("Ok", style: TextStyle(fontWeight: FontWeight.bold)),
                icon: Icon(CupertinoIcons.checkmark_alt, color: colorBlue, size: 20.0),
                style: ElevatedButton.styleFrom(
                  elevation: 0,
                  primary: Colors.white, // background
                  onPrimary: colorBlue, // foreground
                  shape: RoundedRectangleBorder(
                    borderRadius: new BorderRadius.circular(8.0),
                  ),
                ),
              ),
            ],
          );
        });
  }

  Widget buildScreen(BuildContext context) {
    var tabStyle = TextStyle(color: primaryColor);
    var tabStyle2 = TextStyle(color: dangerColor);
    return DefaultTabController(
      length: 2,
      child: Scaffold(
        appBar: AppBar(
          leading: IconButton(
            icon: const Icon(CupertinoIcons.multiply, color: colorSeeds),
            onPressed: () {
              Navigator.pop(context);
            },
          ),
          title: Text("Stok Count"),
          actions: <Widget>[
            IconButton(
              icon: Icon(
                CupertinoIcons.search_circle,
                color: (_planCodeController.text.isEmpty ? colorBlue : dangerColor),
              ),
              onPressed: () {
                _displayTextInputDialog(context);
              },
            ),
            IconButton(
              icon: const Icon(CupertinoIcons.refresh_circled),
              onPressed: () async {
                refresh();
              },
            ),
          ],
          bottom: TabBar(
            controller: _tabController,
            isScrollable: true,
            indicatorColor: colorLeafyGreen,
            indicatorSize: TabBarIndicatorSize.tab,
            indicatorWeight: 3,
            // indicator: BoxDecoration(
            //   borderRadius: BorderRadius.circular(50), // Creates border
            //   color: Colors.grey.shade300,
            // ),
            tabs: [
              Tab(
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 0),
                  child: Row(
                    children: [Text("Tasks ", style: tabStyle), Text("${_selectTaskCode ?? ""}", style: tabStyle2)],
                  ),
                ),
              ),
              Tab(
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 20),
                  child: Row(
                    children: [Text("Plan Count ", style: tabStyle), Text("${_selectPlanCode ?? ""}", style: tabStyle2)],
                  ),
                ),
              ),
            ],
          ),
        ),
        body: TabBarView(
          controller: _tabController,
          children: <Widget>[
            tasks.isEmpty
                ? Center(child: Text('Empty'))
                : ListView.separated(
                    separatorBuilder: (BuildContext context, int index) => const Divider(),
                    itemCount: tasks.length,
                    itemBuilder: (context, index) {
                      return Row(
                        children: [
                          Container(
                            width: 100,
                            child: ListTile(
                              title: Text(
                                "${tasks[index].countcode}",
                                textAlign: TextAlign.center,
                                style: TextStyle(
                                  fontSize: 16,
                                  color: colorSeeds,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              subtitle: Text(
                                "${decTflow(tasks[index].counttype)}",
                                textAlign: TextAlign.center,
                                style: TextStyle(fontSize: 11, color: colorBlue),
                              ),
                            ),
                          ),
                          Expanded(
                            flex: 2,
                            child: ListTile(
                              onTap: () async => await getplan(tasks[index]),
                              title: Text(
                                "${tasks[index].countname}",
                                style: TextStyle(fontSize: 13, color: colorBlue),
                              ),
                              subtitle: Text(
                                "${_formatDate(tasks[index].datestart)} to ${_formatDate(tasks[index].dateend)}",
                                style: TextStyle(fontSize: 11, color: Colors.grey),
                              ),
                            ),
                          ),
                        ],
                      );
                    },
                  ),
            plans.isEmpty
                ? Center(child: Text('Empty'))
                : ListView.separated(
                    separatorBuilder: (BuildContext context, int index) => const Divider(),
                    itemCount: plans.length,
                    itemBuilder: (context, index) {
                      return Row(
                        children: [
                          Container(
                            width: 100,
                            child: ListTile(
                              title: Text(
                                "${plans[index].plancode}",
                                textAlign: TextAlign.center,
                                style: TextStyle(
                                  fontSize: 16,
                                  color: colorSeeds,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              subtitle: Text(
                                "Plan Code",
                                textAlign: TextAlign.center,
                                style: TextStyle(fontSize: 11, color: colorBlue),
                              ),
                            ),
                          ),
                          Expanded(
                            flex: 2,
                            child: ListTile(
                              onTap: () async => await getlines(plans[index]),
                              title: Text(
                                "${plans[index].planname}",
                                style: TextStyle(fontSize: 13, color: colorBlue),
                              ),
                              subtitle: Text(
                                "Task ${plans[index].countcode}",
                                style: TextStyle(fontSize: 11, color: Colors.grey),
                              ),
                              trailing: Icon(
                                planIcon(plans[index].tflow),
                                color: planState(plans[index].tflow),
                                size: 16,
                              ),
                            ),
                          ),
                        ],
                      );
                    },
                  ),
          ],
        ),
      ),
    );
  }
}
