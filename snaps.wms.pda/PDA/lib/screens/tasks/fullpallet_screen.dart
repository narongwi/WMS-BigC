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
import 'package:fluttertoast/fluttertoast.dart';
import '../../constants.dart';

class ApproachScreen extends StatefulWidget {
  static const routeName = '/palletpick';

  @override
  State<StatefulWidget> createState() {
    return _ApproachScreen();
  }
}

class _ApproachScreen extends State<ApproachScreen> {
  TaskService service = TaskService();
  final scanLocController = TextEditingController();
  final scanhuController = TextEditingController();
  final conflocController = TextEditingController();
  final confirmFcusNode = FocusNode();
  final hufocusNode = FocusNode();
  final fromfocusNode = FocusNode();
  bool isLoading = false;
  bool isconfirm = false;
  Parameters taallowchangeworker = Parameters();
  Parameters taallowscanhuno = Parameters();
  Parameters taallowautoassign = Parameters();
  Parameters taallowscansourcelocation = Parameters();
  Parameters taallowscanbarcode = Parameters();
  Parameters taallowchangequantity = Parameters();
  Parameters taallowpickndrop = Parameters();
  Parameters taallowcheckdigit = Parameters();
  Parameters taallowfullycollect = Parameters();
  Parameters taallowchangetarget = Parameters();

  Profiles profile = Profiles();
  TaskMovement taskmvt = TaskMovement();
  MovementLines taskline = MovementLines();
  Product product = Product();
  List<TaskList> taskList = <TaskList>[];
  List<Lov> lov = <Lov>[];
  String unitDesc;
  // Todo initial unit lov
  Future<void> _getunit() async {
    try {
      LovService lovSerivce = new LovService();
      lov = await lovSerivce.getUnit();
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  @override
  void dispose() {
    scanLocController.dispose();
    confirmFcusNode.dispose();
    scanhuController.dispose();
    conflocController.dispose();
    hufocusNode.dispose();
    fromfocusNode.dispose();
    super.dispose();
  }

  Future<void> _parameter() async {
    try {
      ParameterService paramSerivce = new ParameterService();
      String replen = "replenishment";
      List<Parameters> _pm = await paramSerivce.getParameter("task", replen);

      setState(() {
        taallowchangeworker = _pm.firstWhere(
          (x) => x.pmcode == "allowchangeworker",
          orElse: () => Parameters(),
        );
        taallowscanhuno = _pm.firstWhere(
          (x) => x.pmcode == "allowscanhuno",
          orElse: () => Parameters(),
        );
        taallowautoassign = _pm.firstWhere(
          (x) => x.pmcode == "allowautoassign",
          orElse: () => Parameters(),
        );
        taallowscansourcelocation = _pm.firstWhere(
          (x) => x.pmcode == "allowscansourcelocation",
          orElse: () => Parameters(),
        );
        taallowscanbarcode = _pm.firstWhere(
          (x) => x.pmcode == "allowscanbarcode",
          orElse: () => Parameters(),
        );
        taallowchangequantity = _pm.firstWhere(
          (x) => x.pmcode == "allowchangequantity",
          orElse: () => Parameters(),
        );
        taallowpickndrop = _pm.firstWhere(
          (x) => x.pmcode == "allowpickndrop",
          orElse: () => Parameters(),
        );
        taallowcheckdigit = _pm.firstWhere(
          (x) => x.pmcode == "allowcheckdigit",
          orElse: () => Parameters(),
        );
        taallowfullycollect = _pm.firstWhere(
          (x) => x.pmcode == "allowfullycollect",
          orElse: () => Parameters(),
        );
        taallowchangetarget = _pm.firstWhere(
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

  // step 1 list all of task pending
  // show in list view below form
  Future<void> facttask() async {
    try {
      setState(() => isLoading = true);
      // filter task with call web api
      final fillterParams = TaskFilter(
        tasktype: 'A',
        tflow: 'IO',
      );
      final fillterResult = await service.lists(fillterParams);
      setState(() => isLoading = false);

      // is task pending list
      if (fillterResult.length > 0) {
        setState(() {
          taskList = fillterResult.toList();
          fromfocusNode.requestFocus();
        });
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // step 2 scan source location
  Future<void> sourceLocation(String sourceloc) async {
    try {
      clearScreen();
      setState(() => isLoading = true);
      final fillterParams = TaskFilter(
        tasktype: 'A',
        sourceloc: sourceloc,
        tflow: 'IO',
      );
      final fillterResult = await service.lists(fillterParams);

      // no data found
      if (fillterResult.length == 0) {
        alert(context, "warning", "Warning", "invalid source location $sourceloc");

        // back to scan location
        fromfocusNode.requestFocus();
      } else {
        // check scanhu parameter
        if (taallowscanhuno.pmvalue) {
          setState(() {
            isLoading = false;
            hufocusNode.requestFocus();
          });
        } else {
          var currenthu = fillterResult.single;
          await autoAssignTask(currenthu);
          Fluttertoast.showToast(msg: "Assign task success");
        }
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> _huinfo(String sourcehu) async {
    try {
      // get atarget location
      setState(() => isLoading = true);

      final filter = TaskFilter(
        tasktype: 'A',
        tflow: 'IO',
        sourcehuno: sourcehu,
      );

      final _taskList = await service.lists(filter);
      setState(() => isLoading = false);

      final _target = _taskList.firstWhere(
        (e) => e.sourcehuno == sourcehu,
        orElse: () => null,
      );

      if (_taskList.length == 0 || _target == null) {
        alert(context, "warning", "Warning", "invalid HU No $sourcehu");
      } else {
        alert(context, "info", "Target : ${_target.sourceloc}", "HU : $sourcehu");
      }

      setState(() {
        scanLocController.text = "";
        fromfocusNode.requestFocus();
      });

      // _resetScreen();
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> _scanhu(String sourcehu) async {
    try {
      if (scanLocController.text.isEmpty) {
        await _huinfo(sourcehu);
      } else {
        setState(() => isLoading = true);
        FocusScope.of(context).requestFocus(new FocusNode());
        final _filterloc = TaskFilter(tasktype: 'A', tflow: 'IO', sourceloc: scanLocController.text, sourcehuno: sourcehu);
        final _selecthu = await service.lists(_filterloc);
        if (_selecthu.length == 0) {
          alert(context, "warning", "Warning", "invalid HU No $sourcehu");
          hufocusNode.requestFocus();
        } else {
          await autoAssignTask(_selecthu.single);
        }
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> autoAssignTask(TaskList _selecthu) async {
    final _task = await service.assignTask(_selecthu, profile.accncode);
    print("_task.setno : ${_task.setno}");
    final _line = _task.lines.single;
    final _arts = await service.productInfo(_line.article, _line.lv.toString());
    if (_line.accnassign != profile.accncode && _task.tflow == 'IO') {
      alert(context, "info", "Infomation", "location ${_selecthu.sourceloc} is already working on user ${_line.accnassign}");
      setState(() => isconfirm = false);
    } else if (_task.tflow != 'IO') {
      setState(() => isconfirm = false);
      alert(context, "success", "Information", "HU ${_selecthu.sourceloc} is Finished!");
    } else {
      setState(() => isconfirm = true);
    }

    setState(() {
      isLoading = false;
      taskmvt = _task;
      taskline = _line;
      product = _arts;
      unitDesc = decodeUnit(product.unitreceipt.toString());
      confirmFcusNode.requestFocus();
    });
  }

  Future<void> _confirm() async {
    if (taskmvt == null) {
      alert(context, "warning", "Warning", "please scan HU.No.");
      hufocusNode.requestFocus();
    } else if (conflocController.text.isEmpty) {
      alert(context, "warning", "Warning", "confirm location is required !");
    } else if (conflocController.text != taskline.targetadv) {
      alert(context, "warning", "Warning", "Location ${conflocController.text} is invalid !");
      confirmFcusNode.requestFocus();
    } else {
      taskmvt.lines[0].skipdigit = "skip";
      taskmvt.lines[0].targetloc = taskmvt.lines[0].targetadv;
      taskmvt.lines[0].targethuno = taskmvt.lines[0].sourcehuno;
      taskmvt.confirmdigit = "";

      try {
        setState(() => isLoading = true);
        await service.confirm(taskmvt);
        alert(context, "success", "Information", " confirm Pallet Pick success");

        clearScreen();
        // refresh
        scanLocController.text = "";

        await facttask();
      } catch (e) {
        alert(context, "error", "Error", e.toString());
      }
    }
  }

  void clearScreen() {
    FocusScope.of(context).requestFocus(
      new FocusNode(),
    );
    taskmvt = TaskMovement();
    taskline = MovementLines();
    product = Product();
    taskList = <TaskList>[];
    isconfirm = false;
    setState(() {
      fromfocusNode.requestFocus();
      scanhuController.text = "";
      conflocController.text = "";
      unitDesc = "";
    });
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
    _getunit();
    _parameter();
    facttask();
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
    var appBar = AppBar(
      leadingWidth: 50,
      leading: IconButton(
        onPressed: () => Navigator.pop(context),
        icon: Icon(CupertinoIcons.home, size: 20),
      ),
      title: Text(
        'Pallet Pick',
        style: TextStyle(fontWeight: FontWeight.bold),
      ),
      actions: <Widget>[
        IconButton(
          icon: const Icon(CupertinoIcons.plus_circle, color: colorBlue),
          onPressed: () async {
            clearScreen();
            setState(() {
              scanLocController.text = "";
            });
            await facttask();
          },
        ),
      ],
    );

    var fromTextField = TextField(
      enabled: !isconfirm,
      controller: scanLocController,
      focusNode: fromfocusNode,
      decoration: Txtheme.deco(
        icon: Icons.domain,
        label: "From : ",
        enabled: !isconfirm,
      ),
      onSubmitted: (value) async {
        if (value.isNotEmpty) {
          await sourceLocation(value);
        }
      },
    );
    var hunoTextField = TextField(
      // enabled: taallowautoassign.pmvalue,
      onSubmitted: (value) async {
        if (value.isNotEmpty) {
          await _scanhu(value);
        }
      },
      controller: scanhuController,
      focusNode: hufocusNode,
      decoration: Txtheme.deco(
        icon: Icons.domain,
        label: "HU No : ",
        enabled: taallowscanhuno.pmvalue,
      ),
    );

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
                  style: TextStyle(fontSize: 12, color: Colors.grey, fontStyle: FontStyle.italic),
                )
              : Text(
                  "${product.descalt ?? ""}",
                  style: TextStyle(fontSize: 12, color: Colors.red),
                ),
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
      padding: const EdgeInsets.only(top: 3, bottom: 10),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          Text(
            "Qty :",
            style: TextStyle(fontSize: 12),
          ),
          SizedBox(width: 10),
          Text("${taskline.sourceqty ?? ""}"),
          SizedBox(width: 10),
          Text("${unitDesc ?? ""}", style: TextStyle(color: Colors.red)),
          Spacer(),
          Text(
            "Loc.Suggest ",
            style: TextStyle(fontSize: 12),
          ),
          Text("${taskline.targetadv ?? ""}", style: TextStyle(color: successColor))
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
      controller: conflocController,
      focusNode: confirmFcusNode,
      decoration: Txtheme.deco(
        icon: Icons.domain,
        label: "LOC: ",
        enabled: isconfirm,
      ),
      onSubmitted: (pono) async {
        if (pono.isEmpty) return;
        var conf = DialogConfirm(
          title: "Confirm Pallet Pick",
          content: "Location ${conflocController.text} Are you sure ?",
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
                if (conflocController.text.isEmpty) return;
                var conf = DialogConfirm(
                  title: "Confirm Pallet Pick",
                  content: "Location ${conflocController.text} Are you sure ?",
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
      appBar: appBar,
      body: SingleChildScrollView(
        child: Container(
          padding: const EdgeInsets.only(left: 20, right: 20, bottom: 30),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              SizedBox(height: 15),
              // Text("Search", style: tsStyle),
              fromTextField,
              SizedBox(height: 10),
              hunoTextField,
              SizedBox(height: 10),
              // dockTitle,
              // SizedBox(height: 10),
              productCard,
              SizedBox(height: 15),
              // Text("Confirm Location ", style: tsStyle),
              Row(
                children: [
                  Expanded(child: confirmLocTextField),
                  SizedBox(width: 5),
                  confirmLocButton,
                ],
              ),
              // Text("Movement Task", style: tsStyle),
              SizedBox(height: 20),
              taskTable
            ],
          ),
        ),
      ),
    );
  }
}
