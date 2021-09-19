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
  PrepLines _preplineops = PrepLines();
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
        final _prepln = _prepdetail.preplines.where((e) => e.loccode == locationNo).toList();

        if (_prepln.length == 0) {
          throw Exception("invalid location $locationNo !");
        } else {
          print("tflow : ${_prepdetail.tflow}");
          // * Auto Assign Preparation
          if (_prepdetail.tflow == 'IO') {
            // * set start by login user
            await service.setstart(_prepln.first);

            // * refresh prep status
            await selectprep(_currentprep);

            Fluttertoast.showToast(msg: "Preparation is started", backgroundColor: colorSpringGreen);
          } else {
            print("Preparation is already started");
          }

          if (_prepln.length == 1) {
            // get product master infomation
            final _product = await service.productInfo(
              _prepln.first.article,
              _prepln.first.lv.toString(),
            );

            setState(() {
              isLoading = false;
              enableSave = true;
              isScanbar = false;
              _productinfo = _product;
              _preplineops = _prepln.first;
              _preplines = _prepln;
              unitDesc = decodeUnit(_prepln.first.unitprep);
              scanbarController.text = _productinfo.barcode;
              confqtyController.text = "";
              qtyFocus.requestFocus();
            });
          } else {
            Fluttertoast.showToast(msg: "Multiple pick line", backgroundColor: colorStem);
            setState(() {
              _productinfo = Product();
              _preplineops = PrepLines();
              _preplines = _prepln;
              isLoading = false;
              enableSave = false;
              isScanbar = true;
              unitDesc = "";
              scanbarController.text = "";
              confqtyController.text = "";
              barFocus.requestFocus();
            });
          }
        }
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // step 2 scan product if multple barcode in prep
  Future<void> scanBarcode(String barcode) async {
    try {
      if (_preplines.length == 0 || prepController.text.trim().isEmpty) {
        alert(context, "warning", "Invalid Preparation", "please select Preparation No.");
      } else {
        setState(() => isLoading = true);

        // get product master
        final product = await service.getProduct(barcode);

        if ((product.article ?? "").isEmpty) {
          alert(context, "warning", "Find Product", "Data Not found.");
          setState(() => barFocus.requestFocus());
        } else {
          // get prep line
          final lineops = _preplines.where((x) => (x.article == product.article)).toList();

          if (lineops.length == 0) {
            alert(context, "warning", "Check Line", "Data Not found.");
            setState(() => barFocus.requestFocus());
          } else {
            if (lineops.length > 1) {
              Fluttertoast.showToast(msg: "Multiple pick huno", backgroundColor: colorStem);
            }

            setState(() {
              isLoading = false;
              enableSave = true;
              isScanbar = false;
              _productinfo = product;
              _preplineops = lineops.first;
              unitDesc = decodeUnit(_preplineops.unitprep);
              scanbarController.text = product.barcode;
              confqtyController.text = "";
            });

            setState(() {
              qtyFocus.requestFocus();
            });
          }
        }
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
      setState(() => barFocus.requestFocus());
    }
  }

  Future<void> savePick() async {
    try {
      //int _sumorderqty = _preplines.fold(0, (tot, item) => tot + item.qtypuorder);

      _currentPickqty = int.parse(confqtyController.text);
      if (_prepdetail.prepno == null) {
        alert(context, "error", "Error", "please enter preperation no.");
      } else if (_preplines.first.prepno == null) {
        alert(context, "error", "Error", "please enter preperation no.");
      } else if (confqtyController.text.isEmpty) {
        alert(context, "error", "Error", "Quantity is required");
        // } else if (_currentPickqty > _sumorderqty) {
      } else {
        final linehuops = _preplines.where((x) => (x.article == _productinfo.article)).toList();
        final qtypuorder = linehuops.fold(0, (tot, item) => tot + item.qtypuorder);
        if (_currentPickqty > qtypuorder) {
          alert(context, "error", "Error", "Quantity is over order");
        } else {
          // Support Multiple Pallet on Picking
          for (var i = 0; i < linehuops.length; i++) {
            // set pick qty is not over order qty
            int _physicalPickqty = (_currentPickqty <= linehuops[i].qtypuorder ? _currentPickqty : linehuops[i].qtypuorder);

            // Update Pick qty
            linehuops[i].qtypuops = _physicalPickqty * linehuops[i].rtoskuofpu;
            linehuops[i].qtyskuops = _physicalPickqty;
            linehuops[i].accnmodify = profile.accncode;
            linehuops[i].locdigit = "";
            linehuops[i].skipdigit = "skip";

            // cal confirm qty
            _currentPickqty = (_currentPickqty - _physicalPickqty);

            print("_physicalhuno: ${linehuops[i].huno} line ${linehuops[i].prepln}");
            print("_physicalPickqty: $_physicalPickqty");
            print("_currentPickqty: $_physicalPickqty");

            // exit loop;
            if (_currentPickqty <= 0) break;
          }

          // save pick
          setState(() => isLoading = true);
          await service.setpick(linehuops);

          setState(() {
            isLoading = false;
            scanbarController.text = "";
            confqtyController.text = "";
            unitDesc = "";
            _productinfo = new Product();
            _preplineops = new PrepLines();

            if (_preplines.length == 1) {
              locnoController.text = "";
            }
          });

          // refresh screen
          await selectprep(_currentprep);
          Fluttertoast.showToast(msg: "Picked Successfully !", backgroundColor: colorSpringGreen);

          if (_preplines.length == 1) {
            setState(() => locFocus.requestFocus());
          } else {
            setState(() => barFocus.requestFocus());
          }
        }
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
    _preplineops = PrepLines();
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
    _preplineops = PrepLines();
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
      keyboardType: TextInputType.number,
      controller: scanbarController,
      focusNode: barFocus,
      decoration: Txtheme.deco(
        icon: Icons.qr_code,
        label: "Barcode : ",
      ),
      onSubmitted: (value) async {
        if (value.isNotEmpty) {
          await scanBarcode(value);
        }
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
                      style: TextStyle(fontSize: 12, color: Colors.grey, fontStyle: FontStyle.italic),
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
        CardLabel(title: _preplineops.batchno, subTitle: "Batch"),
        CardLabel(title: _formatDate(_preplineops.datemfg), subTitle: "MFG"),
        CardLabel(title: _formatDate(_preplineops.dateexp), subTitle: "EXP"),
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
                    "${_preplineops.ouorder ?? ""}",
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
                    "${_preplineops.picker ?? ""}",
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
                    " ${_preplineops.loczone ?? ""}",
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
                    (_productinfo.article ?? "").isEmpty ? "" : "${_currentprep.thcode ?? ""}",
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
                    (_productinfo.article ?? "").isEmpty ? "" : "${_currentprep.thname ?? ""}",
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
                CardLabel(title: _productinfo.rtoskuofpu ?? "", subTitle: "SKU/PU"),
                CardLabel(title: _productinfo.rtoskuofhu ?? "", subTitle: "SKU/Pal"),
                CardLabel(title: _productinfo.rtopckoflayer ?? "", subTitle: "PCK/Lay"),
                CardLabel(title: _productinfo.rtolayerofhu ?? "", subTitle: "Lay/Pal"),
                CardLabel(title: _productinfo.rtopckofpallet ?? "", subTitle: "PCK/Pal"),
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
            textAlign: TextAlign.center,
            keyboardType: TextInputType.number,
            decoration: Txtheme.deco(
              icon: Icons.shopping_bag_outlined,
              label: "Pick : ",
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
            style: ElevatedButton.styleFrom(primary: primaryColor),
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
                      DataCell(Text(item.qtyskuorder.toString(), style: redStyle)),
                      DataCell(Icon(
                        Icons.check,
                        color: item.qtypuops == 0 ? Colors.transparent : Colors.green,
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
        style: TextStyle(
          fontWeight: FontWeight.bold,
        ),
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
            children: [SizedBox(height: 10), prepTextField, SizedBox(height: 10), locationTextField, SizedBox(height: 10), barcodeTextField, SizedBox(height: 10), productMaster, SizedBox(height: 20), confirmQuantity, SizedBox(height: 20), loosepickTable, SizedBox(height: 20), footerButton],
          ),
        ),
      ),
      // drawer: SideMenu(),
      // persistentFooterButtons: <Widget>[footerButton],
    );
  }
}
