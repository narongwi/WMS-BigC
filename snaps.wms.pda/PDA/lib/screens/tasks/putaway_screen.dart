import 'package:flutter/services.dart';
import 'package:intl/intl.dart';
import 'package:wms/components/card_label.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/dialogconfirm_component.dart';
import 'package:wms/components/input_decoration.dart';
import 'package:wms/components/progress_component.dart';
import 'package:wms/models/lov_model.dart';
import 'package:wms/models/parameter_model.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:wms/screens/receive/models/product_active.dart';
import 'package:wms/screens/tasks/models/taskfilter_models.dart';
import 'package:wms/screens/tasks/models/tasklist_models.dart';
import 'package:wms/screens/tasks/models/taskmovement_model.dart';
import 'package:wms/screens/tasks/services/tasks_services.dart';
import 'package:wms/services/lov_services.dart';
import 'package:wms/services/parammeter_services.dart';

import '../../constants.dart';

class PutawayScreen extends StatefulWidget {
  static const routeName = '/putaway';

  @override
  State<StatefulWidget> createState() {
    return _PutawayScreen();
  }
}

class _PutawayScreen extends State<PutawayScreen> {
  TaskService service = TaskService();
  final huController = TextEditingController();
  final locController = TextEditingController();
  final hufocusNode = FocusNode();
  final locfocusNode = FocusNode();

  bool isLoading = false;
  bool isconfirm = false;
  Profiles profile = Profiles();
  Parameters tpallowscanhuongrap = Parameters();
  Parameters tpallowautoassign = Parameters();
  Parameters tpallowscansourcelocation = Parameters();
  Parameters tpallowscanbarcode = Parameters();
  Parameters tpallowinputqtyongrap = Parameters();
  Parameters tpallowpickndrop = Parameters();
  Parameters tpallowcheckdigit = Parameters();
  Parameters tpallowfullygrap = Parameters();
  Parameters tpallowfullycollect = Parameters();
  Parameters tpallowchangetarget = Parameters();

  TaskMovement taskmvt = TaskMovement();
  MovementLines taskline = MovementLines();
  Product product = Product();
  List<TaskList> taskList = <TaskList>[];
  List<Lov> lov = <Lov>[];
  String unitDesc;

  // Todo initial unit lov
  Future<void> _getunit() async {
    LovService lovSerivce = new LovService();
    lov = await lovSerivce.getUnit();
  }

  Future<void> _parameter() async {
    try {
      ParameterService paramSerivce = new ParameterService();
      String putaway = "putaway";
      final _pm = await paramSerivce.getParameter("task", putaway);
      setState(() {
        tpallowscanhuongrap = _pm.firstWhere(
          (x) => x.pmcode == "allowscanhuongrap",
          orElse: () => Parameters(),
        );
        tpallowautoassign = _pm.firstWhere(
          (x) => x.pmcode == "allowautoassign",
          orElse: () => Parameters(),
        );
        tpallowscansourcelocation = _pm.firstWhere(
          (x) => x.pmcode == "allowscansourcelocation",
          orElse: () => Parameters(),
        );
        tpallowscanbarcode = _pm.firstWhere(
          (x) => x.pmcode == "allowscanbarcode",
          orElse: () => Parameters(),
        );
        tpallowinputqtyongrap = _pm.firstWhere(
          (x) => x.pmcode == "allowinputqtyongrap",
          orElse: () => Parameters(),
        );
        tpallowpickndrop = _pm.firstWhere(
          (x) => x.pmcode == "allowpickndrop",
          orElse: () => Parameters(),
        );
        tpallowcheckdigit = _pm.firstWhere(
          (x) => x.pmcode == "allowcheckdigit",
          orElse: () => Parameters(),
        );
        tpallowfullygrap = _pm.firstWhere(
          (x) => x.pmcode == "allowfullygrap",
          orElse: () => Parameters(),
        );
        tpallowfullycollect = _pm.firstWhere(
          (x) => x.pmcode == "allowfullycollect",
          orElse: () => Parameters(),
        );
        tpallowchangetarget = _pm.firstWhere(
          (x) => x.pmcode == "allowchangetarget",
          orElse: () => Parameters(),
        );
      });
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  String decodeUnit(String unitcode) {
    print("dec $unitcode");
    if (unitcode == null) return "";
    try {
      return lov.firstWhere((e) => e.value == unitcode).desc;
    } catch (e) {
      alert(context, "error", "Error", e.toString());
      return "";
    }
  }

  //
  String decodeType(String orderType) {
    switch (orderType) {
      case 'ST':
        return 'Stocking';
      case 'XD':
        return 'Crossdocking';
      case 'FW':
        return 'Forwarding';
      default:
        return orderType;
    }
  }

  Future<void> _taskList() async {
    setState(() => isLoading = true);
    try {
      final _filtertask = TaskFilter(
        tasktype: 'P',
        tflow: 'IO',
      );
      final _taskList = await service.lists(_filtertask);
      setState(() {
        taskList = _taskList.take(30).toList();
        isLoading = false;
      });
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> _scanhu(String huno) async {
    try {
      setState(() => isLoading = true);
      // FocusScope.of(context).requestFocus(new FocusNode());
      final _filterhu = TaskFilter(
        tasktype: 'P',
        sourcehuno: huno,
        tflow: 'IO',
      );
      final _selecthu = await service.lists(_filterhu);

      if (_selecthu.length == 0) {
        alert(context, "warning", "Warning", "invalid HU No $huno");
      } else {
        setState(() => isLoading = true);
        final _task = await service.assignTask(_selecthu[0], profile.accncode);
        final _line = _task.lines.single;
        final _arts =
            await service.productInfo(_line.article, _line.lv.toString());

        if (_line.accnassign != profile.accncode && _task.tflow == 'IO') {
          alert(context, "info", "Infomation",
              "HU $huno is already working on user ${_line.accnassign}");
          setState(() => isconfirm = false);
        } else if (_task.tflow != 'IO') {
          setState(() => isconfirm = false);
          alert(context, "success", "Information", "HU $huno is Finished!");
        } else {
          setState(() => isconfirm = true);
        }

        setState(() {
          isLoading = false;
          taskmvt = _task;
          taskline = _line;
          product = _arts;
          unitDesc = decodeUnit(product.unitreceipt.toString());
          locfocusNode.requestFocus();
        });
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> _confirm() async {
    if (taskmvt == null) {
      alert(context, "warning", "Warning", "please scan HU.No.");
      hufocusNode.requestFocus();
    } else if (locController.text.isEmpty) {
      alert(context, "warning", "Warning", "confirm location is required !");
    } else if (tpallowchangetarget.pmvalue &&
        taskmvt.lines[0].targetadv != locController.text) {
      alert(
        context,
        "error",
        "Error",
        "Task is not change target location !",
      );
    } else {
      taskmvt.lines[0].skipdigit = "skip";
      taskmvt.lines[0].targetloc = locController.text;
      taskmvt.confirmdigit = "";

      try {
        setState(() => isLoading = true);
        await service.confirm(taskmvt);
        alert(
            context, "success", "Information", " confirm task putaway success");

        _resetScreen();
        // refresh
        await _taskList();
      } catch (e) {
        alert(context, "error", "Error", e.toString());
      }
    }
  }

  void _resetScreen() {
    // FocusScope.of(context).requestFocus(
    //   new FocusNode(),
    // );

    taskmvt = TaskMovement();
    taskline = MovementLines();
    product = Product();
    taskList = <TaskList>[];
    huController.text = "";
    locController.text = "";
    isconfirm = false;
    unitDesc = "";
    hufocusNode.requestFocus();
  }

  String _formatDate(dynamic _date) {
    if (_date == null || _date == '') {
      return "";
    } else {
      return DateFormat('dd/MM/yyyy').format(_date);
    }
  }

  String _formatDateTime(dynamic _date) {
    if (_date == null || _date == '') {
      return "";
    } else {
      return DateFormat('dd/MM/yyyy HH:mm:ss').format(_date);
    }
  }

  // custom alert popup
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
    _resetScreen();
    _getunit();
    _parameter();
    _taskList();
  }

  @override
  void dispose() {
    huController.dispose();
    locController.dispose();
    hufocusNode.dispose();
    locfocusNode.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    profile = ModalRoute.of(context).settings.arguments as Profiles;

    return ProgressContainer(
      child: buildScreen(context),
      inAsyncCall: isLoading,
      opacity: 0.3,
    );
  }

  Widget buildScreen(BuildContext context) {
    // isEnterKeyd(RawKeyEvent event) {
    //   print(event.)

    //   if (event.logicalKey == LogicalKeyboardKey.enter) {
    //     return true;
    //   } else if (event.data is RawKeyEventDataAndroid) {
    //     final data = event.data as RawKeyEventDataAndroid;
    //     return data.keyCode == 13 ? true : false;
    //   } else if (event.data is RawKeyEventDataIos) {
    //     final data = event.data as RawKeyEventDataIos;
    //     return data.keyCode == 13 ? true : false;
    //   } else if (event.data is RawKeyEventDataWeb) {
    //     final data = event.data as RawKeyEventDataWeb;
    //     return data.keyLabel == 'Enter' ? true : false;
    //   } else {
    //     return false;
    //   }
    // }

    // var hunoTextField2 = RawKeyboardListener(
    //   focusNode: hufocusNode,
    //   onKey: (event) {
    //     print(event.data.logicalKey.keyId);
    //     if (isEnterKeyd(event)) {
    //       alert(context, "warning", "Warning",
    //           "you key ${event.data.logicalKey.keyLabel}");
    //     }
    //   },
    //   child: TextField(
    //     controller: huController,
    //     decoration: Txtheme.deco(
    //       icon: Icons.domain,
    //       label: "HU No 2: ",
    //     ),
    //     onSubmitted: (_textController) {
    //       alert(context, "warning", "Warning", "submit text $_textController");
    //       // Navigator.pop(context, this.huController.text);
    //     },
    //   ),
    // );

    TextField hunoTextField = TextField(
      textInputAction: TextInputAction.search,
      controller: huController,
      focusNode: hufocusNode,
      decoration: Txtheme.deco(
        icon: Icons.domain,
        label: "HU No: ",
      ),
      onSubmitted: (huno) {
        if (huno.isEmpty) {
          alert(context, "warning", "Warning", "please enter huno");
        } else {
          _scanhu(huno).then((value) => null);
        }
      },
    );
    AppBar appBar(BuildContext context) {
      return AppBar(
        leadingWidth: 50,
        leading: IconButton(
          onPressed: () => Navigator.pop(context),
          icon: Icon(CupertinoIcons.home, size: 20),
        ),
        title: Text('Putaway'),
        actions: <Widget>[
          IconButton(
            icon: const Icon(CupertinoIcons.plus_circle, color: colorBlue),
            onPressed: () async {
              _resetScreen();
              await _taskList();
            },
          ),
        ],
      );
    }

    var productName = Padding(
      padding: EdgeInsets.only(top: 3),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.start,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                "Product ",
                style: TextStyle(fontSize: 12),
              ),
              Text(
                "${product.barcode ?? ""} ${product.article ?? ""} ${product.lv ?? ""}",
                style: TextStyle(fontSize: 12, color: Colors.blue),
              ),
            ],
          ),
          SizedBox(height: 10),
          product.descalt == null
              ? Text(
                  "Product Description",
                  style: TextStyle(
                      fontSize: 12,
                      color: Colors.grey,
                      fontStyle: FontStyle.italic),
                )
              : Text(
                  "${product.descalt ?? ""}",
                  style: TextStyle(fontSize: 12, color: Colors.red),
                ),
          SizedBox(height: 5),
        ],
      ),
    );
    var taskInfo = Padding(
      padding: EdgeInsets.only(top: 15),
      child: Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.start,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Text(
                  "Task No ",
                  style: TextStyle(fontSize: 12),
                ),
              ),
              Expanded(
                child: Text(
                  "${taskline.taskno ?? " "}",
                  style: TextStyle(fontSize: 12, color: Colors.red),
                ),
              ),
              Expanded(
                flex: 1,
                child: Text(
                  " Worker ",
                  style: TextStyle(fontSize: 12),
                ),
              ),
              Expanded(
                child: Text(
                  " ${taskline.accnassign ?? " "}",
                  style: TextStyle(fontSize: 12, color: Colors.red),
                ),
              ),
            ],
          ),
          SizedBox(
            height: 15,
          ),
          Row(
            mainAxisAlignment: MainAxisAlignment.start,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Text(
                  "Task Date ",
                  style: TextStyle(fontSize: 12),
                ),
              ),
              Expanded(
                child: Text(
                  " ${_formatDateTime(taskline.datecreate ?? "")} ",
                  style: TextStyle(fontSize: 12, color: Colors.red),
                ),
              ),
              Expanded(
                flex: 1,
                child: Text(
                  " Dock No ",
                  style: TextStyle(fontSize: 12),
                ),
              ),
              Expanded(
                child: Text(
                  "${taskline.sourceloc ?? ""}",
                  style: TextStyle(fontSize: 12, color: Colors.blue),
                ),
              ),
            ],
          ),
        ],
      ),
    );

    var productMaster = Padding(
      padding: EdgeInsets.only(top: 8, bottom: 10),
      child: Row(mainAxisAlignment: MainAxisAlignment.spaceBetween, children: [
        CardLabel(title: unitDesc, subTitle: "PU Code"),
        CardLabel(title: product.rtoskuofpu, subTitle: "SKU/PU"),
        CardLabel(title: product.rtoskuofhu, subTitle: "SKU/Pal"),
        CardLabel(title: product.rtopckoflayer, subTitle: "PCK/Lay"),
        CardLabel(title: product.rtolayerofhu, subTitle: "Lay/Pal"),
        CardLabel(title: product.rtopckofpallet, subTitle: "PCK/Pal"),
      ]),
    );

    var productControl = Padding(
      padding: const EdgeInsets.only(top: 3, bottom: 3),
      child: Row(mainAxisAlignment: MainAxisAlignment.spaceBetween, children: [
        CardLabel(title: taskline.lotno, subTitle: "Batch"),
        CardLabel(title: _formatDate(taskline.datemfg), subTitle: "MFG"),
        CardLabel(title: _formatDate(taskline.dateexp), subTitle: "EXP"),
      ]),
    );

    var productConfirmQty = Padding(
      padding: const EdgeInsets.only(top: 10, bottom: 20),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          Text(
            "Qty :",
            style: TextStyle(fontSize: 12),
          ),
          SizedBox(width: 10),
          Text("${taskline.sourceqty ?? "0"}"),
          SizedBox(width: 10),
          Text("${unitDesc ?? ""}", style: TextStyle(color: Colors.red)),
          Text(
            "Loc.Suggest ",
            style: TextStyle(fontSize: 12),
          ),
          Text("${taskline.targetadv ?? ""}",
              style: TextStyle(color: successColor))
        ],
      ),
    );

    var productCard = Card(
      elevation: 0,
      margin: EdgeInsets.zero,
      color: iconBgColor,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          taskInfo,
          Divider(color: Colors.grey[80], height: 10),
          productName,
          Divider(color: Colors.grey[80], height: 10),
          productMaster,
          Divider(color: Colors.grey[80], height: 10),
          productControl,
          Divider(color: Colors.grey[80], height: 10),
          productConfirmQty,
        ],
      ),
    );
    var confirmLocTextField = TextField(
      enabled: isconfirm,
      controller: locController,
      focusNode: locfocusNode,
      decoration: Txtheme.deco(
        icon: Icons.domain,
        label: "LOC:",
        enabled: isconfirm,
      ),
      onSubmitted: (pono) async {
        if (pono.isEmpty) return;
        var conf = DialogConfirm(
          title: "Confirm Putaway",
          content: "Location ${locController.text} Are you sure ?",
          onYes: () async => await _confirm(),
          onNo: () {},
        );

        showDialog(
          context: context,
          builder: (BuildContext context) => conf,
        );
      },
    );

    var confirmLocButton = ButtonTheme(
      height: 30.0,
      child: ElevatedButton.icon(
        style: ElevatedButton.styleFrom(primary: successColor),
        icon: Icon(
          CupertinoIcons.checkmark_alt,
          size: 13,
        ),
        label: Text("Confirm"),
        onPressed: isconfirm
            ? () {
                if (locController.text.isEmpty) return;
                var conf = DialogConfirm(
                  title: "Confirm Putaway",
                  content: "Location ${locController.text} Are you sure ?",
                  onYes: () async => await _confirm(),
                  onNo: () {},
                );

                showDialog(
                  context: context,
                  builder: (BuildContext context) => conf,
                );
              }
            : null,
      ),
    );
    // Tables
    const colTextStyle = TextStyle(fontSize: 12);
    const blueStyle = TextStyle(color: primaryColor);
    const redStyle = TextStyle(color: dangerColor);
    const greenStyle = TextStyle(color: successColor);
    const dataStyle = TextStyle(fontSize: 12, color: primaryColor);
    var rowColor = MaterialStateColor.resolveWith((states) => Colors.white);
    var taskTable = Container(
      height: MediaQuery.of(context).size.height / 2,
      width: MediaQuery.of(context).size.width,
      padding: EdgeInsets.all(2),
      decoration: BoxDecoration(
        border: Border.all(color: Color(0xFFEEEEEE)),
        borderRadius: BorderRadius.all(Radius.circular(5.0)),
        color: Colors.white,
      ),
      child: SingleChildScrollView(
        scrollDirection: Axis.vertical,
        child: SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: DataTable(
            dataRowHeight: 30,
            dataTextStyle: dataStyle,
            columnSpacing: 15,
            headingRowHeight: 40,
            headingRowColor: rowColor,
            columns: const <DataColumn>[
              // DataColumn(label: Text("Task.Date", style: colTextStyle)),
              DataColumn(label: Text("Task.No", style: colTextStyle)),
              DataColumn(label: Text("Product", style: colTextStyle)),
              DataColumn(label: Text("Description", style: colTextStyle)),
              DataColumn(label: Text("Ref.No", style: colTextStyle)),
              DataColumn(label: Text("HU.No.", style: colTextStyle)),
              DataColumn(label: Text("Source", style: colTextStyle)),
              DataColumn(label: Text("Target", style: colTextStyle)),
            ],
            rows: taskList
                .map((rw) => DataRow(cells: [
                      // DataCell(Text(rw.taskdate, style: redStyle)),
                      DataCell(Text(rw.taskno, style: redStyle)),
                      DataCell(Text("${rw.article} ${rw.lv}", style: redStyle)),
                      DataCell(Text(rw.descalt, style: dataStyle)),
                      DataCell(Text(rw.iorefno, style: blueStyle)),
                      DataCell(Text(rw.sourcehuno, style: redStyle)),
                      DataCell(Text(rw.sourceloc, style: greenStyle)),
                      DataCell(Text(rw.targetadv, style: redStyle)),
                    ]))
                .toList(),
          ),
        ),
      ),
    );

    // layout
    return Scaffold(
      appBar: appBar(context),
      body: SafeArea(
        child: SingleChildScrollView(
          child: Container(
            padding: const EdgeInsets.only(left: 20, right: 20, bottom: 30),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                SizedBox(height: 15),
                hunoTextField,
                SizedBox(height: 10),
                productCard,
                Row(
                  children: [
                    Expanded(child: confirmLocTextField),
                    SizedBox(width: 5),
                    confirmLocButton,
                  ],
                ),
                SizedBox(height: 15),
                taskTable
              ],
            ),
          ),
        ),
      ),
    );
  }
}
