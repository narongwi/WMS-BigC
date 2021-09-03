// import 'dart:convert';

import 'package:fluttertoast/fluttertoast.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/dialogconfirm_component.dart';
import 'package:wms/components/input_decoration.dart';
import 'package:wms/components/progress_component.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:flutter/services.dart';
import 'package:wms/models/lov_model.dart';
import 'package:wms/screens/distribution/genempty_screen.dart';
import 'package:wms/screens/distribution/models/empty_model.dart';
import 'package:wms/screens/distribution/models/prepdistribute_model.dart';
import 'package:wms/screens/distribution/serivces/distribute_services.dart';
import 'package:wms/screens/home/models/profiles.dart';
// import 'package:wms/screens/receive/models/product_active.dart';
import 'package:wms/services/lov_services.dart';
import '../../constants.dart';

class DistributeScreen extends StatefulWidget {
  static const routeName = '/distribute';

  @override
  State<StatefulWidget> createState() {
    return _DistributeScreen();
  }
}

class _DistributeScreen extends State<DistributeScreen> {
  // final _service = OutboundService();
  final hunoController = TextEditingController();
  final barcodeController = TextEditingController();
  final emptyHuController = TextEditingController();
  final qtyPuController = TextEditingController();
  final qtySkuController = TextEditingController();

  final hunoFocusNode = FocusNode();
  final docknoFocusNode = FocusNode();
  final barcodeFocusNode = FocusNode();
  final emptyFocusNode = FocusNode();
  final qtyFocusNode = FocusNode();
  bool isscanbar = false;
  bool isscanempty = false;
  bool isLoading = false;
  bool enableFinish = false;
  bool enablePutline = false;
  List<Lov> lov = <Lov>[];
  DistributeServices service = DistributeServices();
  List<Distribution> prepdtrs = <Distribution>[];
  Profiles profile;
  // Product product = Product();
  Distribution prepdtr = Distribution();
  DistrbInfo distinfo = DistrbInfo();
  List<DistrbLine> lines = <DistrbLine>[];
  DistrbLine distline = DistrbLine();
  Empty emptyInfo = Empty();
  String unitDesc;

  Future<void> _getunit() async {
    try {
      LovService lovSerivce = new LovService();
      lov = await lovSerivce.getUnit();
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  void resetScreen() {
    setState(() {
      isLoading = false;
      //prepdtr = Distribution();
      distline = DistrbLine();
      lines = <DistrbLine>[];
      emptyInfo = Empty();
      unitDesc = "";
      enableFinish = false;
      enablePutline = false;
      barcodeController.text = "";
      emptyHuController.text = "";
      qtyPuController.text = "";
      qtySkuController.text = "";
      if (distinfo.tflow == 'PA') {
        //distinfo = DistrbInfo();
        enableFinish = true;
      } else {
        distinfo = DistrbInfo();
        prepdtr = Distribution();
        hunoController.text = "";
      }

      hunoFocusNode.requestFocus();
    });
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

  // Scan Receive Pallet for Get HU
  // Get Pallet Preperation List
  // Get Preperation Infomation
  Future<void> scanhu(String huno) async {
    try {
      if (huno.isNotEmpty) {
        setState(() => isLoading = true);
        final _filter = DistributeFilter(
          spcarea: 'XD',
          preptype: 'P',
          huno: huno,
        );
        // get xd preperation list
        final _prepdtrs = await service.listprep(_filter);

        // preperation information and line detail
        final _distinfo = await service.getprep(_prepdtrs.single);

        final _lines = _distinfo.lines.where((x) => x.qtypuops < x.qtypuorder || x.preplineops == 0).toList();
        if (_lines.length == 0) {
          final message = _distinfo.lines.length > 0 ? "This HU has already completed" : "No data found!";
          alert(context, "info", "Information", message);
          resetScreen();
          setState(() {
            isLoading = false;
            distinfo = _distinfo;
            prepdtr = _prepdtrs.single;
          });
        } else {
          setState(() {
            isLoading = false;
            prepdtr = _prepdtrs.single;
            distinfo = _distinfo;
            lines = _lines;
            // hide and show finished button
            enableFinish = distinfo.tflow == 'PA' ? true : false;
            // default putline input
            enablePutline = false;
            // clear text input
            barcodeController.text = "";
            emptyHuController.text = "";
            qtyPuController.text = "";
            unitDesc = "";
            emptyInfo = Empty();
            // confirm scan product
            barcodeFocusNode.requestFocus();
          });
        }
      } else {
        hunoFocusNode.requestFocus();
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // refresh preperation detail
  Future<void> refresh() async {
    try {
      // show loading bar
      setState(() => isLoading = true);

      // get xd preperation info by prep
      final _distinfo = await service.getprep(prepdtr);
      final _lines = _distinfo.lines.where((x) => x.qtypuops < x.qtypuorder || x.preplineops == 0).toList();
      if (_lines.length == 0) {
        // alert(context, "info", "Information", "This HU has already completed or No data found!");
        resetScreen();
        setState(() {
          isLoading = false;
          distinfo = _distinfo;
        });
      } else {
        // update screen data
        setState(() {
          isLoading = false;
          distinfo = _distinfo;
          enableFinish = distinfo.tflow == 'PA' ? true : false;
          enablePutline = false;
          emptyHuController.text = "";
          qtyPuController.text = "";
          unitDesc = "";
          emptyInfo = Empty();
        });
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // scan check product
  Future<void> scanBarcode(String barcode) async {
    try {
      // show loading
      setState(() => isLoading = true);
      // get product info by barcode or article
      final _product = await service.getProduct(barcode);

      // product is not equal preperation product
      if (_product.article != prepdtr.thcode) {
        alert(context, "error", "Error", "Not found barcode in this Receive Pallet");

        // scan product agian
        setState(() => barcodeFocusNode.requestFocus());
      } else {
        final _lnops = lines.firstWhere(
          (x) => x.hunosource == distinfo.huno && x.article == _product.article && x.lv == _product.lv,
          orElse: () => null,
        );

        setState(() {
          isLoading = false;
          emptyHuController.text = (_lnops.huno ?? "");

          // Scan HU Empty Pallet
          emptyFocusNode.requestFocus();
        });
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // Scan Store HU Empty Event
  Future<void> scanEmpty(String emptyno) async {
    try {
      setState(() => isLoading = true);

      // get hu empty infomation
      final filter = EmptyFilter(hutype: 'XE', huno: emptyno, tflow: 'IO');
      final emptys = await service.getEmpty(filter);

      // stamp auto start preperation
      if (distinfo.tflow == "IO") {
        await service.setstart(distinfo);
        Fluttertoast.showToast(msg: "Started", backgroundColor: colorStem);
      }

      //hu empty not found
      if (emptys == null) {
        alert(context, "warning", "Warning", "Not found this Empty HU / Empty HU not available");
      } else {
        // get preperateion store by empty pallet
        final _distline = distinfo.lines.firstWhere((x) => x.thcode == emptys.thcode, orElse: () => null);

        if (_distline == null) {
          alert(context, "warning", "Warning", "Not found this Empty HU / Empty HU not available");
          setState(() => emptyFocusNode.requestFocus());
        } else {
          setState(() {
            isLoading = false;
            // Put line process
            enablePutline = true;
            // current empty infomation
            emptyInfo = emptys;
            // current preperation line
            distline = _distline;
            // convert preperation unit description
            unitDesc = decodeUnit(distline.unitprep);
            // initail input qty
            qtyPuController.text = (_distline.qtypuops ?? "").toString();
            qtyFocusNode.requestFocus();
          });
        }
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // update preperation line qty
  Future<void> putline() async {
    try {
      if (distline == null || emptyInfo == null) {
        alert(context, "error", "Warning", "invalid HU empty please try again");
        emptyFocusNode.requestFocus();
      } else if (distline.qtypuorder < int.parse(qtyPuController.text)) {
        alert(context, "error", "Warning", "Put Over Qty!");
        qtyFocusNode.requestFocus();
      } else {
        setState(() => isLoading = true);
        // setpreperation d
        distline.huno = emptyInfo.huno;
        distline.qtypuops = int.parse(qtyPuController.text);
        await service.putline(distline);
        alert(context, "success", "Information", "Put product to grid success");

        setState(() {
          isLoading = false;
          // clear input after call api
          qtyPuController.text = "";
          emptyHuController.text = "";
          enablePutline = false;
          enableFinish = false;
          unitDesc = "";

          // next scan empty
          emptyFocusNode.requestFocus();
        });

        // refresh screen data
        await refresh();
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // finish preperation XD
  Future<void> finishDistr() async {
    try {
      // check if no scan hu
      if (distinfo.prepno == null || distinfo.prepno.isEmpty) {
        alert(context, "error", "Warning", "HU No. Is required");
      } else if (distinfo.lines.where((x) => x.qtypuops < x.qtypuorder).length > 0) {
        alert(context, "error", "Error", "Pick < Order please try again");
      } else {
        setState(() => isLoading = true);
        // finish preperation and distribute

        await service.finishDistr(distinfo);
        alert(context, "success", "Information", "Confirm Finished Successfully.");

        setState(() {
          isLoading = false;
          distinfo.tflow = "ED";
          resetScreen();
        });
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
    await showDialog(
      context: ctx,
      builder: (BuildContext context) => alert,
    );
  }

  @override
  void initState() {
    super.initState();
    distinfo.lines = <DistrbLine>[];
    // initail
    _getunit();
    // _clearForm();
    // _factHUs();
  }

  @override
  void dispose() {
    super.dispose();

    //TextFiled
    hunoController.dispose();
    barcodeController.dispose();
    emptyHuController.dispose();
    qtyPuController.dispose();
    qtySkuController.dispose();
    lines = null;
    // focus
    hunoFocusNode.dispose();
    docknoFocusNode.dispose();
    barcodeFocusNode.dispose();
    qtyFocusNode.dispose();
    // Object
    lov = null;
    service = null;
    prepdtrs = null;
    profile = null;
    // product = null;
    prepdtr = null;
    distinfo = null;
    distline = null;
    emptyInfo = null;
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
    // Scan HU Input Text
    var huTextField = TextField(
      controller: hunoController,
      focusNode: hunoFocusNode,
      keyboardType: TextInputType.number,
      onSubmitted: (huno) async {
        FocusScope.of(context).requestFocus(new FocusNode());
        if (huno.isNotEmpty) {
          await scanhu(huno);
        }
      },
      decoration: Txtheme.deco(
        icon: Icons.domain,
        label: "HU No   ",
      ),
    );

    // Store Detail
    var workingStore = Card(
      elevation: 0,
      margin: EdgeInsets.zero,
      color: iconBgColor,
      child: Column(
        children: [
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 15, vertical: 10),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    Icon(
                      Icons.store,
                      color: primaryColor,
                      size: 20,
                    ),
                    SizedBox(
                      height: 5,
                    ),
                    Text("store", style: TextStyle(fontSize: 13)),
                  ],
                ),
                SizedBox(
                  width: 10,
                ),
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      "${emptyInfo.thcode ?? ""} ",
                      style: TextStyle(
                        color: dangerColor,
                        fontWeight: FontWeight.bold,
                        fontSize: 18,
                      ),
                    ),
                    Text(
                      "${emptyInfo.thname ?? ""}",
                      style: TextStyle(color: primaryColor, fontSize: 13),
                    )
                  ],
                ),
              ],
            ),
          ),
          Divider(),
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 10),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                Text("Order Qty"),
                Text(
                  "${distline.qtypuorder ?? ""} ",
                  style: TextStyle(
                    color: dangerColor,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                Text("${unitDesc ?? ""}", style: TextStyle(color: primaryColor, fontSize: 12)),
              ],
            ),
          ),
          // Padding(
          //   padding: const EdgeInsets.symmetric(horizontal: 15, vertical: 10),
          //   child: Row(
          //     // mainAxisAlignment: MainAxisAlignment.spaceBetween,
          //     crossAxisAlignment: CrossAxisAlignment.center,
          //     children: [
          //       Expanded(
          //         child: Text("Put"),
          //       ),
          //       Expanded(
          //         child: TextField(
          //           textAlign: TextAlign.center,
          //           style: TextStyle(color: dangerColor),
          //         ),
          //       ),
          //       Expanded(
          //         child: Text(
          //           "${unitDesc ?? "Unit"}",
          //           textAlign: TextAlign.right,
          //         ),
          //       ),
          //     ],
          //   ),
          // ),
        ],
      ),
    );

    var barcodeTextField = TextField(
        readOnly: isscanbar,
        keyboardType: TextInputType.number,
        focusNode: barcodeFocusNode,
        controller: barcodeController,
        onSubmitted: (value) async => {
              if (value.isNotEmpty) {await scanBarcode(value)}
            },
        decoration: Txtheme.deco(icon: Icons.qr_code, label: "Barcode ")
        // decoration: Txtheme.decoration(
        //   Icons.qr_code,
        //   "Barcode ",
        //   "Barcode ",
        //   null,
        // ),
        );
    var productName = Padding(
      padding: EdgeInsets.only(top: 5),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              SizedBox(
                width: 80,
                child: Text(
                  "Target ",
                  style: TextStyle(fontSize: 13, color: secondaryColor),
                ),
              ),
              Expanded(
                child: Text(
                  "${distinfo.loccode ?? ""}",
                  style: TextStyle(fontSize: 13, color: dangerColor),
                ),
              ),
              SizedBox(
                width: 80,
                child: Text(
                  "Plan.No ",
                  style: TextStyle(fontSize: 13, color: secondaryColor),
                ),
              ),
              Expanded(
                child: Text(
                  "${prepdtr.prepno ?? ""}",
                  style: TextStyle(fontSize: 13, color: dangerColor),
                ),
              ),
            ],
          ),
          SizedBox(height: 10),
          Row(
            mainAxisAlignment: MainAxisAlignment.start,
            children: [
              Expanded(
                child: Text(
                  "Order ",
                  style: TextStyle(fontSize: 13, color: secondaryColor),
                ),
              ),
              Expanded(
                child: Text(
                  "${prepdtr.routeno ?? ""}",
                  style: TextStyle(fontSize: 13, color: dangerColor),
                ),
              ),
              Expanded(
                child: Text(
                  " Worker ",
                  style: TextStyle(fontSize: 13, color: secondaryColor),
                ),
              ),
              Expanded(
                child: Text(
                  "${prepdtr.picker ?? ""}",
                  style: TextStyle(fontSize: 13, color: dangerColor),
                ),
              ),
            ],
          ),
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 8),
            child: Row(
              children: [
                SizedBox(width: 80, child: Text("${prepdtr.thcode ?? ""}", style: TextStyle(fontSize: 13, color: dangerColor))),
                Expanded(child: Text("${prepdtr.thname ?? ""}", style: TextStyle(fontSize: 13, color: primaryColor))),
              ],
            ),
          ),
        ],
      ),
    );
    var productMaster = Card(
      elevation: 0,
      margin: EdgeInsets.zero,
      color: iconBgColor,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          productName,
          SizedBox(
            height: 10,
          )
          // Divider(color: Colors.grey),
          // Row(
          //   mainAxisAlignment: MainAxisAlignment.center,
          //   children: [
          //     Padding(
          //       padding: const EdgeInsets.only(left: 15, bottom: 5),
          //       child: Row(
          //         children: [
          //           Text(
          //             "PU Code :",
          //             style: TextStyle(color: Colors.grey, fontSize: 12),
          //           ),
          //         ],
          //       ),
          //     ),
          //     Spacer(),
          //     Padding(
          //       padding: const EdgeInsets.only(right: 15, bottom: 10),
          //       child: Row(
          //         children: [
          //           Text("${distline.unitname ?? ""}",
          //               style: TextStyle(color: Colors.red)),
          //         ],
          //       ),
          //     ),
          //   ],
          // ),
        ],
      ),
    );
    var emptyTextField = Container(
      child: Row(
        children: [
          Expanded(
            child: TextField(
              readOnly: isscanempty,
              controller: emptyHuController,
              focusNode: emptyFocusNode,
              onSubmitted: (value) async {
                if (value.isNotEmpty) {
                  await scanEmpty(value);
                }
              },
              decoration: Txtheme.deco(icon: CupertinoIcons.tray_full, label: "Empty  "),
            ),
          ),
        ],
      ),
    );
    var qtyTextFiled = Row(
      children: [
        Expanded(
          child: TextField(
            enabled: enablePutline,
            focusNode: qtyFocusNode,
            controller: qtyPuController,
            textAlign: TextAlign.center,
            keyboardType: TextInputType.number,
            autocorrect: true,
            onSubmitted: (qty) async {
              var conf = DialogConfirm(
                title: "Confirm",
                content: "Do you confirm put product ?",
                onYes: () async => await putline(),
                onNo: () {},
              );
              showDialog(
                context: context,
                builder: (BuildContext context) => conf,
              );
            },
            decoration: Txtheme.deco(icon: Icons.save, label: "Put : ", suffix: unitDesc, enabled: enablePutline),
          ),
        ),
        SizedBox(width: 10),
        Center(
          child: SizedBox(
            width: 110,
            child: ElevatedButton.icon(
              icon: Icon(
                CupertinoIcons.checkmark_alt,
                size: 13,
              ),
              label: Text("Put line"),
              style: ElevatedButton.styleFrom(primary: primaryColor),
              onPressed: enablePutline
                  ? () async {
                      var conf = DialogConfirm(
                        title: "Confirm",
                        content: "Do you confirm put product ?",
                        onYes: () async => await putline(),
                        onNo: () {},
                      );
                      showDialog(
                        context: context,
                        builder: (BuildContext context) => conf,
                      );
                    }
                  : null,
            ),
          ),
        )
        // Expanded(
        //   child: TextField(
        //     readOnly: true,
        //     controller: qtySkuController,
        //     decoration: Txtheme.decoration(
        //       null,
        //       "SKU Qty : ",
        //       "SKU.Qty : ",
        //       null,
        //     ),
        //   ),
        // ),
      ],
    );
    // var confirmButton = Center(
    //   child: SizedBox(
    //     width: 200,
    //     child: ElevatedButton.icon(
    //       icon: Icon(CupertinoIcons.checkmark_alt),
    //       label: Text("Put line"),
    //       style: ElevatedButton.styleFrom(primary: warningColor),
    //       onPressed: () async {
    //         var conf = DialogConfirm(
    //           title: "Confirm Putline",
    //           content: "Are you sure ?",
    //           // onYes: () async => await putLine(),
    //           onNo: () {},
    //         );
    //         showDialog(
    //           context: context,
    //           builder: (BuildContext context) => conf,
    //         );
    //       },
    //     ),
    //   ),
    // );
    var footerButtons = Container(
      width: MediaQuery.of(context).copyWith().size.width,
      child: Row(
        children: [
          Expanded(
            child: ElevatedButton.icon(
              style: ElevatedButton.styleFrom(primary: Colors.lightGreen),
              label: Text("Finish"),
              icon: Icon(
                Icons.check_circle,
                color: Colors.white,
                size: 16,
              ),
              onPressed: enableFinish
                  ? () async {
                      var conf = DialogConfirm(
                        title: "Finish",
                        content: "Do you confirm to end preparation ?",
                        onYes: () async => await finishDistr(),
                        onNo: () {},
                      );

                      showDialog(
                        context: context,
                        builder: (BuildContext context) => conf,
                      );
                    }
                  : null,
            ),
          ),
        ],
      ),
    );
    // return main
    // Tables
    const colTextStyle = TextStyle(fontSize: 12);
    const blueStyle = TextStyle(color: primaryColor);
    const redStyle = TextStyle(color: dangerColor);
    // const greenStyle = TextStyle(color: successColor);
    const dataStyle = TextStyle(fontSize: 12, color: primaryColor);
    var rowColor = MaterialStateColor.resolveWith((states) => Colors.white);
    var detailTable = Container(
        height: MediaQuery.of(context).size.height / 2,
        width: MediaQuery.of(context).size.width,
        padding: EdgeInsets.all(2),
        decoration: BoxDecoration(
          border: Border.all(color: Color(0xFFCCCCCC)),
          borderRadius: BorderRadius.all(Radius.circular(5.0)),
          color: Colors.white,
        ),
        // padding: const EdgeInsets.fromLTRB(10, 5, 20, 5),
        child: SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: DataTable(
            dataRowHeight: 30,
            dataTextStyle: dataStyle,
            columnSpacing: 15,
            headingRowHeight: 40,
            headingRowColor: rowColor,
            columns: const <DataColumn>[
              DataColumn(label: Text("Store", style: colTextStyle)),
              // DataColumn(label: Text("Name", style: colTextStyle)),
              DataColumn(
                label: SizedBox(
                  width: 100,
                  child: Text("HU No", style: colTextStyle),
                ),
              ),
              DataColumn(
                label: SizedBox(
                  width: 100,
                  child: Text("Empty HU", style: colTextStyle),
                ),
              ),
              DataColumn(label: Text("Ord Qty", style: colTextStyle)),
              DataColumn(label: Text("Dist Qty", style: colTextStyle)),
            ],
            rows: (lines ?? <DistrbLine>[])
                .map(
                  (rw) => DataRow(cells: [
                    DataCell(Text(rw.thcode, style: redStyle)),
                    // DataCell(Text(rw.thcode, style: greenStyle)),
                    DataCell(Text(rw.hunosource, style: blueStyle)),
                    DataCell(Text(rw.huno, style: blueStyle)),
                    DataCell(Text("${rw.qtypuorder}", style: blueStyle)),
                    DataCell(Text("${rw.qtypuops}", style: blueStyle)),
                  ]),
                )
                .toList(),
          ),
        ));
    return Scaffold(
      appBar: AppBar(
        leadingWidth: 50,
        leading: IconButton(
          onPressed: () => Navigator.pop(context),
          icon: Icon(CupertinoIcons.home, size: 20),
        ),
        title: Text(
          'Distribution',
          style: TextStyle(fontWeight: FontWeight.bold),
        ),
        actions: <Widget>[
          IconButton(
            icon: const Icon(
              CupertinoIcons.tray_full,
            ),
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => GenEmptyScreen(
                    profile: profile,
                  ),
                ),
              );
            },
          ),
          IconButton(
            icon: const Icon(CupertinoIcons.plus_circle),
            onPressed: () => resetScreen(),
          ),
        ],
      ),
      body: SafeArea(
        child: SingleChildScrollView(
          child: Container(
            padding: const EdgeInsets.only(left: 20, right: 20, bottom: 30),
            child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
              SizedBox(height: 15),
              huTextField,
              SizedBox(height: 10),
              productMaster,
              SizedBox(height: 10),
              barcodeTextField,
              SizedBox(height: 10),
              emptyTextField,
              SizedBox(height: 10),
              workingStore,
              SizedBox(height: 10),
              qtyTextFiled,
              // SizedBox(height: 10),
              // confirmButton,
              SizedBox(height: 20),
              detailTable,
              SizedBox(height: 10),

              footerButtons
            ]),
          ),
        ),
      ),
      // persistentFooterButtons: <Widget>[footerButtons],
    );
  }
}
