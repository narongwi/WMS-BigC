import 'package:fluttertoast/fluttertoast.dart';
import 'package:intl/intl.dart';
import 'package:wms/components/card_label.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/dialogconfirm_component.dart';
import 'package:wms/components/input_decoration.dart';
import 'package:wms/components/progress_component.dart';
import 'package:wms/models/lov_model.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:wms/screens/loosepick/models/prep_detail.dart';
import 'package:wms/screens/loosepick/preplist_screen.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:wms/screens/loosepick/services/prep_services.dart';
import 'package:wms/screens/receive/models/product_active.dart';
import 'package:wms/services/lov_services.dart';
import '../../constants.dart';
import 'models/prep_filter.dart';
import 'models/prep_lists.dart';

class LoosePickScreen extends StatefulWidget {
  static const routeName = '/loosepick';

  @override
  State<StatefulWidget> createState() {
    return _LoosePickScreen();
  }
}

class _LoosePickScreen extends State<LoosePickScreen> {
  LoosePickServices service = LoosePickServices();
  TextEditingController prepController = TextEditingController();
  TextEditingController locnoController = TextEditingController();
  TextEditingController hunoController = TextEditingController();
  TextEditingController supcodeController = TextEditingController();
  TextEditingController supnameController = TextEditingController();
  TextEditingController scanbarController = TextEditingController();
  TextEditingController confqtyController = TextEditingController();
  FocusNode prepFocus = FocusNode();
  FocusNode hunoFocus = FocusNode();
  FocusNode locFocus = FocusNode();
  FocusNode barFocus = FocusNode();
  FocusNode qtyFocus = FocusNode();

  bool isLoading = false;
  bool isScanbar = false;
  bool enableSave = false;
  String unitDesc;
  Profiles profile = Profiles();
  Product _productinfo = Product();
  PrepDetails _prepdetail = PrepDetails();
  List<PrepLines> _preplines = <PrepLines>[];
  PrepLines _prepops = PrepLines();
  PrepLists _currentprep = PrepLists();
  List<Lov> lov = <Lov>[];

  // total pick per prep
  int _currentPickqty = 0;
  // Todo initial unit lov
  @override
  void initState() {
    super.initState();
    unitMaster();

    _prepdetail.preplines = <PrepLines>[];
  }

  @override
  void dispose() {
    _preplines = [];
    _preplines = [];
    prepController?.dispose();
    locnoController?.dispose();
    hunoController?.dispose();
    supcodeController?.dispose();
    supnameController?.dispose();
    scanbarController?.dispose();
    confqtyController?.dispose();
    prepFocus?.dispose();
    hunoFocus?.dispose();
    locFocus?.dispose();
    barFocus?.dispose();
    qtyFocus?.dispose();
    super.dispose();
  }

  Future<void> unitMaster() async {
    try {
      LovService lovSerivce = new LovService();
      lov = await lovSerivce.getUnit();
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  String decodeUnit(String unitcode) {
    try {
      if (unitcode == null) return "";
      return lov.firstWhere((e) => e.value == unitcode).desc;
    } catch (e) {
      alert(context, "error", "Error", e.toString());
      return "";
    }
  }

  Future<void> factprep(String prepno) async {
    try {
      if (prepno.isNotEmpty) {
        final filterParam = PrepFilter(
          spcarea: 'ST',
          preptype: 'P',
          prepno: prepno,
        );

        clearFormData();

        setState(() => isLoading = true);
        final filterResult = await service.listprep(filterParam);
        if (filterResult.length == 0) {
          await alert(context, "warning", "Information", "data not found.");
          prepFocus.requestFocus();
        } else {
          setState(() => isLoading = false);
          await selectprep(filterResult.first);
        }
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // step 1 select preparation no
  Future<void> selectprep(PrepLists _selectprep) async {
    try {
      if (_selectprep.tflow == 'PE') {
        alert(context, "warning", "Information", "Preparation is Waiting load");
      } else {
        setState(() => isLoading = true);
        final _prepdt = await service.getprep(_selectprep);
        setState(() {
          enableSave = false;
          isLoading = false;
          _prepdetail = _prepdt;
          _currentprep = _selectprep;
          locFocus.requestFocus();
        });
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // step 2 scan location
  Future<void> scanLocaion(String locationNo) async {
    try {
      if (prepController.text.isNotEmpty && locationNo.isNotEmpty) {
        // get prepline on api return list
        // final _prepln = _prepdetail.preplines
        //     .firstWhere((x) => x.loccode == locationNo, orElse: () => null);

        final _prepln = _prepdetail.preplines
            .where((e) => e.loccode == locationNo)
            .toList();

        _prepln.forEach((element) {
          print("Article : ${element.article} LV ${element.lv}");
        });

        if (_prepln.length == 0) {
          throw Exception("invalid location $locationNo !");
        } else {
          setState(() => isLoading = true);

          // step 3 get product master infomation
          final _product = await service.productInfo(
            _prepln.first.article,
            _prepln.first.lv.toString(),
          );
          // step 4 start if status is active
          if (_prepdetail.tflow == 'IO') {
            await service.setstart(_prepln.first);
            Fluttertoast.showToast(
              msg: "start preparation successful",
              backgroundColor: colorSpringGreen,
            );
          }

          setState(() {
            _productinfo = _product;
            _preplines = _prepln;
            _prepops = _prepln.first;
            isLoading = false;
            enableSave = true;
            unitDesc = decodeUnit(_preplines.first.unitprep);
            scanbarController.text = _productinfo.barcode;
            qtyFocus.requestFocus();
          });
        }
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> savePick() async {
    try {
      int _sumorderqty =
          _preplines.fold(0, (tot, item) => tot + item.qtypuorder);
      _currentPickqty = int.parse(confqtyController.text);

      if (_prepdetail.prepno == null) {
        alert(context, "error", "Error", "please enter preperation no.");
      } else if (_preplines.first.prepno == null) {
        alert(context, "error", "Error", "please enter preperation no.");
      } else if (confqtyController.text.isEmpty) {
        alert(context, "error", "Error", "Quantity is required");
      } else if (_currentPickqty > _sumorderqty) {
        alert(context, "error", "Error", "Quantity is over order");
      } else if (confqtyController.text.isNotEmpty) {
        // Support Multiple Pallet on Picking
        for (var i = 0; i < _preplines.length; i++) {
          int _physicalPickqty = (_currentPickqty <= _preplines[i].qtypuorder
              ? _currentPickqty
              : _preplines[i].qtypuorder);

          // Update Pick qty
          _preplines[i].qtypuops = _physicalPickqty * _preplines[i].rtoskuofpu;
          _preplines[i].qtyskuops = _physicalPickqty;
          _preplines[i].accnmodify = profile.accncode;
          _preplines[i].locdigit = "";
          _preplines[i].skipdigit = "skip";

          // cal confirm qty
          _currentPickqty = (_currentPickqty - _physicalPickqty);

          print(
              "_physicalhuno: ${_preplines[i].huno} line ${_preplines[i].prepln}");
          print("_physicalPickqty: $_physicalPickqty");
          print("_currentPickqty: $_physicalPickqty");

          // exit loop;
          // if (_currentPickqty == 0) break;
        }

        // save pick
        setState(() => isLoading = true);
        await service.setpick(_preplines);
        setState(() => isLoading = false);

        setState(() {
          locnoController.text = "";
          scanbarController.text = "";
          confqtyController.text = "";
          unitDesc = "";

          _productinfo = new Product();
          _prepops = new PrepLines();

          locFocus.requestFocus();
        });

        // refresh screen
        await selectprep(_currentprep);

        // alert(context, "success", "Infomation", "Picked Successfully !");
        Fluttertoast.showToast(
          msg: "Picked Successfully !",
          backgroundColor: colorSpringGreen,
        );
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> finishPick() async {
    print("Finsihed");
    try {
      if (_prepdetail.prepno == null) {
        alert(context, "error", "Error", "preparation no. is required");
      } else if (_prepdetail.prepno == null) {
        alert(context, "error", "Error", "preparation no. is required");
      } else {
        print("Picked");
        _prepdetail.accnmodify = profile.accncode;

        setState(() => isLoading = true);

        print(_prepdetail.toJson());
        await service.setend(_prepdetail);
        setState(() {
          enableSave = false;
          _prepdetail.preplines.clear();
          _prepdetail.tflow = "ED";
        });
        // await selectprep(_currentprep);
        alert(context, "success", "Infomation", "End preparation success");
        // clearFormData();

      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
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

    await showDialog(context: ctx, builder: (BuildContext context) => alert);
  }

  void clearFormData() {
    enableSave = false;
    locnoController.text = "";
    scanbarController.text = "";
    scanbarController.text = "";
    confqtyController.text = "";
    _prepdetail = PrepDetails();
    _prepdetail.preplines = <PrepLines>[];
    _preplines = <PrepLines>[];
    _prepops = PrepLines();
    _currentprep = PrepLists();
    _productinfo = Product();
    unitDesc = "";
    prepFocus.requestFocus();
  }

  void clearAllFormData() {
    enableSave = false;
    prepController.text = "";
    locnoController.text = "";
    scanbarController.text = "";
    scanbarController.text = "";
    confqtyController.text = "";
    _prepdetail = PrepDetails();
    _prepdetail.preplines = <PrepLines>[];
    _preplines = <PrepLines>[];
    _prepops = PrepLines();
    _currentprep = PrepLists();
    _productinfo = Product();
    unitDesc = "";
    prepFocus.requestFocus();
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
    var prepTextField = TextField(
      keyboardType: TextInputType.number,
      controller: prepController,
      focusNode: prepFocus,
      decoration: Txtheme.decoration(
        Icons.accessibility,
        "Prep No : ",
        "Prep No : ",
        null,
      ),
      onSubmitted: (value) async {
        if (value.isNotEmpty) {
          await factprep(value);
        }
      },
    );
    var locationTextField = TextField(
      // keyboardType: TextInputType.number,
      controller: locnoController,
      focusNode: locFocus,
      decoration: Txtheme.decoration(
        Icons.domain,
        "Location : ",
        "Location : ",
        null,
      ),
      onSubmitted: (value) async {
        if (value.isNotEmpty) {
          await scanLocaion(value);
        }
      },
    );
    var barcodeTextField = TextField(
      enabled: false,
      keyboardType: TextInputType.number,
      controller: scanbarController,
      focusNode: barFocus,
      decoration: Txtheme.deco(
        icon: Icons.qr_code,
        label: "Barcode : ",
        enabled: false,
      ),
      onSubmitted: (value) async {
        // if (value.isNotEmpty) {
        //   await _scanBarcode(value);
        // }
      },
    );
    var productName = Row(
      children: [
        Padding(
          padding: EdgeInsets.only(top: 10),
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
                    "${_productinfo.barcode ?? ""} ${_productinfo.article ?? ""} ${_productinfo.lv ?? ""}",
                    style: TextStyle(fontSize: 12, color: Colors.blue),
                  ),
                ],
              ),
              SizedBox(height: 5),
              (_productinfo.article ?? "").isEmpty
                  ? Text(
                      "Product Description",
                      textAlign: TextAlign.center,
                      style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey,
                          fontStyle: FontStyle.italic),
                    )
                  : Text(
                      "${_productinfo.descalt ?? ""}",
                      textAlign: TextAlign.center,
                      style: TextStyle(fontSize: 12, color: Colors.red),
                    ),
            ],
          ),
        ),
      ],
    );
    String _formatDate(dynamic _date) {
      if (_date == null) {
        return "";
      } else {
        return DateFormat('dd/MM/yyyy').format(_date);
      }
    }

    var productControl = Padding(
      padding: const EdgeInsets.only(top: 3, bottom: 10),
      child: Row(mainAxisAlignment: MainAxisAlignment.spaceBetween, children: [
        CardLabel(title: _prepops.batchno, subTitle: "Batch"),
        CardLabel(title: _formatDate(_prepops.datemfg), subTitle: "MFG"),
        CardLabel(title: _formatDate(_prepops.dateexp), subTitle: "EXP"),
      ]),
    );

    var productMaster = Card(
      elevation: 0,
      margin: EdgeInsets.zero,
      color: iconBgColor,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.only(top: 5, bottom: 5),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Text(
                    "Order No",
                    style: TextStyle(fontSize: 12),
                  ),
                ),
                Expanded(
                  child: Text(
                    "${_prepops.ouorder ?? ""}",
                    style: TextStyle(fontSize: 12, color: Colors.red),
                  ),
                ),
                Expanded(
                  child: Text(
                    " Worker ",
                    style: TextStyle(fontSize: 12),
                  ),
                ),
                Expanded(
                  child: Text(
                    "${_prepops.picker ?? ""}",
                    style: TextStyle(fontSize: 12, color: Colors.red),
                  ),
                ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.only(top: 10, bottom: 5),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Text(
                    "Prep.zone",
                    style: TextStyle(fontSize: 12),
                  ),
                ),
                Expanded(
                  child: Text(
                    " ${_prepops.loczone ?? ""}",
                    style: TextStyle(fontSize: 12, color: Colors.red),
                  ),
                ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.only(top: 10, bottom: 5),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Text(
                    "Store ",
                    style: TextStyle(fontSize: 12),
                  ),
                ),
                Expanded(
                  child: Text(
                    (_productinfo.article ?? "").isEmpty
                        ? ""
                        : "${_currentprep.thcode ?? ""}",
                    style: TextStyle(fontSize: 12, color: primaryColor),
                  ),
                ),
                Expanded(
                  child: Text(
                    "Name ",
                    style: TextStyle(fontSize: 12),
                  ),
                ),
                Expanded(
                  flex: 3,
                  child: Text(
                    (_productinfo.article ?? "").isEmpty
                        ? ""
                        : "${_currentprep.thname ?? ""}",
                    style: TextStyle(fontSize: 12, color: dangerColor),
                  ),
                ),
              ],
            ),
          ),
          productName,
          Divider(color: Colors.grey[80], height: 10),
          Padding(
            padding: const EdgeInsets.only(bottom: 5),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                CardLabel(title: unitDesc, subTitle: "PU Code"),
                CardLabel(
                    title: _productinfo.rtoskuofpu ?? "", subTitle: "SKU/PU"),
                CardLabel(
                    title: _productinfo.rtoskuofhu ?? "", subTitle: "SKU/Pal"),
                CardLabel(
                    title: _productinfo.rtopckoflayer ?? "",
                    subTitle: "PCK/Lay"),
                CardLabel(
                    title: _productinfo.rtolayerofhu ?? "",
                    subTitle: "Lay/Pal"),
                CardLabel(
                    title: _productinfo.rtopckofpallet ?? "",
                    subTitle: "PCK/Pal"),
              ],
            ),
          ),
          Divider(color: Colors.grey[80], height: 10),
          productControl
        ],
      ),
    );
    var confirmQuantity = Row(
      children: [
        Expanded(
          flex: 2,
          child: TextField(
            enabled: enableSave,
            controller: confqtyController,
            focusNode: qtyFocus,
            keyboardType: TextInputType.number,
            decoration: Txtheme.deco(
              icon: Icons.shopping_bag_outlined,
              label: "Pick Qty : ",
              suffix: "$unitDesc",
              enabled: enableSave,
            ),
            onSubmitted: (value) async {
              if (value.isNotEmpty) {
                var conf = DialogConfirm(
                  title: "Confirm Pick",
                  content: "Do you confirm pick ?",
                  onYes: () async => await savePick(),
                  onNo: () {},
                );

                showDialog(
                  context: context,
                  builder: (BuildContext context) => conf,
                );
              }
            },
          ),
        ),
        SizedBox(width: 5),
        ButtonTheme(
          minWidth: 120.0,
          height: 28.0,
          child: ElevatedButton.icon(
            style: ElevatedButton.styleFrom(primary: infoColor),
            icon: Icon(
              CupertinoIcons.checkmark_alt,
              size: 14,
            ),
            label: Text("Save"),
            onPressed: enableSave
                ? () {
                    if (confqtyController.text.isNotEmpty) {
                      var conf = DialogConfirm(
                        title: "Confirm Pick",
                        content: "Do you confirm pick ?",
                        onYes: () async => await savePick(),
                        onNo: () {},
                      );

                      showDialog(
                        context: context,
                        builder: (BuildContext context) => conf,
                      );
                    }
                  }
                : null,
          ),
        )
      ],
    );

    // Tables
    const colTextStyle = TextStyle(fontSize: 12);
    const blueStyle = TextStyle(color: primaryColor);
    const redStyle = TextStyle(color: dangerColor);
    const greenStyle = TextStyle(color: successColor);
    const dataStyle = TextStyle(fontSize: 12, color: primaryColor);
    var rowColor = MaterialStateColor.resolveWith((states) => Colors.white);
    var loosepickTable = Container(
      height: MediaQuery.of(context).size.height / 2,
      width: MediaQuery.of(context).size.width,
      padding: EdgeInsets.all(2),
      decoration: BoxDecoration(
        border: Border.all(color: Color(0xFFCCCCCC)),
        borderRadius: BorderRadius.all(Radius.circular(5.0)),
        color: Colors.white,
        // borderRadius: BorderRadius.circular(15),
      ),
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        child: DataTable(
            dataRowHeight: 30,
            dataTextStyle: dataStyle,
            columnSpacing: 15,
            headingRowHeight: 40,
            headingRowColor: rowColor,
            columns: const <DataColumn>[
              DataColumn(label: Text("Zone", style: colTextStyle)),
              DataColumn(label: Text("Location", style: colTextStyle)),
              DataColumn(label: Text("HU No", style: colTextStyle)),
              DataColumn(label: Text("Product", style: colTextStyle)),
              DataColumn(label: Text("Barcode", style: colTextStyle)),
              DataColumn(label: Text("Description", style: colTextStyle)),
              DataColumn(label: Text("PU Qty", style: colTextStyle)),
              DataColumn(label: Text("Picked", style: colTextStyle)),
            ],
            rows: _prepdetail.preplines
                .map((item) => DataRow(cells: [
                      DataCell(Text(item.loczone, style: redStyle)),
                      DataCell(Text(item.loccode, style: blueStyle)),
                      DataCell(Text(item.hunosource, style: redStyle)),
                      DataCell(Text(item.article, style: greenStyle)),
                      DataCell(Text(item.barcode, style: blueStyle)),
                      DataCell(Text(item.description, style: blueStyle)),
                      DataCell(
                          Text(item.qtyskuorder.toString(), style: redStyle)),
                      DataCell(Icon(
                        Icons.check,
                        color: item.qtypuops == 0
                            ? Colors.transparent
                            : Colors.green,
                        size: 13,
                      )),
                    ]))
                .toList()),
      ),
    );

    // Body
    var appBar = AppBar(
      leadingWidth: 50,
      leading: IconButton(
        onPressed: () => Navigator.pop(context),
        icon: Icon(CupertinoIcons.home, size: 20),
      ),
      title: Text(
        'Loose Pick',
      ),
      actions: <Widget>[
        IconButton(
          icon: const Icon(CupertinoIcons.plus_circle, color: colorBlue),
          onPressed: () => clearAllFormData(),
        ),
        IconButton(
          icon: const Icon(CupertinoIcons.search_circle, color: colorBlue),
          onPressed: () async {
            setState(() {
              clearAllFormData();
            });
            final selPrep = await Navigator.push(
              context,
              MaterialPageRoute(
                builder: (context) => PrepListScreen(),
              ),
            );

            if (selPrep.prepno == null) return;

            final slp = selPrep as PrepLists;
            print(selPrep.toString());

            if (slp.prepno != null) {
              setState(() => prepController.text = selPrep.prepno);
              await selectprep(selPrep as PrepLists);
            }
          },
        ),
      ],
    );
    var footerButton = Container(
      margin: EdgeInsets.only(left: 0, right: 0),
      width: MediaQuery.of(context).copyWith().size.width,
      child: Row(
        children: [
          Expanded(
            child: ElevatedButton.icon(
              style: ElevatedButton.styleFrom(primary: successColor),
              icon: Icon(CupertinoIcons.checkmark_alt),
              label: Text("Finish"),
              onPressed: _prepdetail.tflow == 'PA'
                  ? () async {
                      if (_prepdetail.preplines.length > 0) {
                        var conf = DialogConfirm(
                          title: "Confirm Preperation",
                          content: "Do you confirm to end preparation  ?",
                          onYes: () async => await finishPick(),
                          onNo: () {},
                        );

                        showDialog(
                          context: context,
                          builder: (BuildContext context) => conf,
                        );
                      }
                    }
                  : null,
            ),
          ),
        ],
      ),
    );

    return Scaffold(
      appBar: appBar,
      body: SingleChildScrollView(
        child: Container(
          padding: const EdgeInsets.only(left: 20, right: 20, bottom: 30),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              SizedBox(height: 10),
              prepTextField,
              SizedBox(height: 10),
              locationTextField,
              SizedBox(height: 10),
              barcodeTextField,
              SizedBox(height: 10),
              productMaster,
              SizedBox(height: 20),
              confirmQuantity,
              SizedBox(height: 20),
              loosepickTable,
              SizedBox(height: 20),
              footerButton
            ],
          ),
        ),
      ),
      // drawer: SideMenu(),
      // persistentFooterButtons: <Widget>[footerButton],
    );
  }
}
