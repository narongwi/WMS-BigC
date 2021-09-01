import 'package:wms/components/datepick_theme.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/dialogconfirm_component.dart';
import 'package:wms/components/input_decoration.dart';
import 'package:wms/components/progress_component.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:flutter/services.dart';
import 'package:intl/intl.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:wms/screens/receive/models/product_radio.dart';
import 'package:wms/screens/receive/models/save_payload.dart';
import 'package:wms/screens/receive/models/searchpo_line.dart';
import 'package:wms/screens/receive/models/searchpo_model.dart';
import 'package:wms/screens/receive/services/receive_services.dart';

import '../../constants.dart';
import '../../components/card_label.dart';
import 'detail_screen.dart';
import 'models/product_active.dart';
import '../../models/lov_model.dart';
import '../../services/lov_services.dart';

class ReceiveScreen extends StatefulWidget {
  static const routeName = '/receive';

  @override
  State<StatefulWidget> createState() {
    return _ReceiveScreen();
  }
}

class _ReceiveScreen extends State<ReceiveScreen> {
  final suppoController = TextEditingController();
  final docknoController = TextEditingController();
  final barcodeController = TextEditingController();
  final batchController = TextEditingController();
  final serialController = TextEditingController();
  final mfgController = TextEditingController();
  final expController = TextEditingController();
  final qtyController = TextEditingController();
  final suppoFocusNode = FocusNode();
  final docknoFocusNode = FocusNode();
  final barcodeFocusNode = FocusNode();
  final batchFocusNode = FocusNode();
  final serialFocusNode = FocusNode();
  final mfgFocusNode = FocusNode();
  final expFocusNode = FocusNode();
  final qtyFocusNode = FocusNode();
  bool isLoading = false;
  bool eanblescanpo = true;
  bool enableStaging = false;
  bool enableStart = false;
  bool enablescanbar = false;

  Profiles profile;
  ReceiveService service = ReceiveService();
  SearchPOLines poline = SearchPOLines();
  SearchPO searchpo = SearchPO();
  Product product = Product();
  List<ProductRadio> radio = <ProductRadio>[];
  List<Lov> lov = <Lov>[];
  // Todo initial unit lov
  Future<void> _getunit() async {
    LovService lovSerivce = new LovService();
    lov = await lovSerivce.getUnit();
  }

  //
  String decodeUnit(unitcode) {
    print("Unit Ops : $unitcode");
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

  // Todo Search PO
  Future<void> _searchpo(String pono) async {
    resetPO();
    setState(() => isLoading = true);
    SearchPO _searchpo = SearchPO();
    _searchpo.lines = <SearchPOLines>[];
    try {
      // get po
      _searchpo = await service.searchPO(pono);
      _searchpo.lines = _searchpo.lines.where((c) => c.qtypnd > 0).toList();
      var _count = _searchpo.lines.where((c) => c.qtypnd > 0).toList().length;
      print("tflow as ${_searchpo.tflow}");
      print("Is Start Load $enableStart");
      print("Count line $_count");

      if (_searchpo.tflow == "ED") {
        alert(context, "info", "Infomation", "$pono PO is already Closed");
      } else {
        // Display Data
        setState(() {
          isLoading = false;
          eanblescanpo = false;
          searchpo = _searchpo;
          enableStaging = _searchpo.dockrec == '' ? true : false;
          if (enableStaging) {
            enableStart = false;
          } else {
            enableStart = _searchpo.tflow == "SA" ? true : false;
          }

          // check already assign dock
          if (searchpo.dockrec.isEmpty) {
            docknoFocusNode.requestFocus();
          } else {
            docknoController.text = searchpo.dockrec;
            if (_count > 0) {
              // pending receive > 0
              enablescanbar = true;
              barcodeFocusNode.requestFocus();
            }
          }
        });
      }
    } catch (e) {
      setState(() {
        alert(context, "error", "Error", "$pono $e");
        searchpo = _searchpo;
        docknoController.text = "";
        suppoFocusNode.requestFocus();
      });
    }
  }

  // Set Staging
  Future<void> _staging(dockno) async {
    if (searchpo.inorder == null) {
      alert(context, "error", "Warning", "Supplier Po Is Not Empty");
    } else if (searchpo.inorder.isEmpty) {
      alert(context, "error", "Warning", "Supplier Po Is Not Empty");
    } else if (dockno.isEmpty) {
      alert(context, "error", "Warning", "Receive Dock Is Not Empty");
    } else {
      try {
        setState(() => isLoading = true);
        await service.setStaging(searchpo.inorder, dockno);
        await _searchpo(suppoController.text);
      } catch (e) {
        alert(context, "error", "Set Staging", e.toString());
      }
    }
  }

  Future<void> _startload() async {
    if (searchpo.inorder == null || searchpo.inorder == '') {
      alert(context, "error", "Warning", "PO No is required");
    } else if (docknoController.text.isEmpty) {
      alert(context, "error", "Warning", "Staging is not Assign!");
    } else {
      try {
        setState(() => isLoading = true);
        await service.setStart(searchpo.inorder);
        await _searchpo(suppoController.text);
      } catch (e) {
        alert(context, "error", "Start Loading", e.toString());
      }
    }
  }

  // Get Barcode
  Future<void> _scanbarcode(String _barcode) async {
    // unFocus();
    resetScanproduct();
    setState(() => barcodeController.text = _barcode);

    setState(() => isLoading = true);
    Product _product = Product();
    List<ProductRadio> _radio = <ProductRadio>[];
    SearchPOLines _poline = SearchPOLines();

    try {
      // get product master
      _product = await service.getProductInfo(_barcode);

      if (_product.barcode == null) {
        throw Exception("Product Data Not Found");
      }
      // get product radio
      _radio = await service.getRadio(
        _product.article,
        _product.pv,
        _product.lv,
      );
      // filter line
      print("bar ${_product.barcode}");
      _poline = searchpo.lines.firstWhere(
        (s) => (s.article == _product.article && s.lv == _product.lv),
        orElse: () => null,
      );
      // check data
      if (_poline == null || _radio == null) {
        throw Exception("Product Data Not Found");
      }

      // Convert unit
      _poline.unitopsdesc = decodeUnit(_poline.unitreceipt);

      // Update Screen
      setState(() {
        isLoading = false;
        product = _product;
        radio = _radio;
        poline = _poline;
        if (poline.isbatchno == 1) {
          batchFocusNode.requestFocus();
        } else if (poline.isunique == 1) {
        } else if (poline.isdlc == 1) {
          mfgFocusNode.requestFocus();
        }
      });
    } catch (e) {
      // Update Screen
      setState(() {
        alert(context, "error", "Error", "$_barcode $e");
        resetScanproduct();
        barcodeFocusNode.requestFocus();
      });
    }
  }

  void resetAllScreen() {
    setState(() {
      searchpo = SearchPO();
      searchpo.lines = <SearchPOLines>[];
      product = Product();
      radio = <ProductRadio>[];
      poline = SearchPOLines();
      docknoController.text = "";
      enableStaging = false;
      enableStart = false;
      enablescanbar = false;
      eanblescanpo = true;
      suppoController.text = "";
      barcodeController.text = "";
      batchController.text = "";
      serialController.text = "";
      mfgController.text = "";
      expController.text = "";
      qtyController.text = "";
      suppoFocusNode.requestFocus();
    });
  }

  void resetPO() {
    setState(() {
      searchpo = SearchPO();
      searchpo.lines = <SearchPOLines>[];
      product = Product();
      radio = <ProductRadio>[];
      poline = SearchPOLines();
      docknoController.text = "";
      enableStaging = false;
      enableStart = false;
      enablescanbar = false;
      eanblescanpo = true;
      barcodeController.text = "";
      batchController.text = "";
      serialController.text = "";
      mfgController.text = "";
      expController.text = "";
      qtyController.text = "";
      suppoFocusNode.requestFocus();
    });
  }

  void resetScanproduct() {
    setState(() {
      product = Product();
      radio = <ProductRadio>[];
      poline = SearchPOLines();
      barcodeController.text = "";
      batchController.text = "";
      serialController.text = "";
      mfgController.text = "";
      expController.text = "";
      qtyController.text = "";
    });
  }

  Future<void> _selectExp(expDate) async {
    if (searchpo.inorder == null) {
      alert(context, "error", "Warning", "Supplier Po Is Not Empty");
    } else if (poline.inorder.isEmpty) {
      alert(context, "error", "Warning", "Please Scan Barcode");
    } else if (expDate.isEmpty) {
      alert(context, "error", "Warning", "Please Select EXP Date");
    } else if (poline.article.isEmpty) {
      alert(context, "error", "Warning", "Please Scan Barcode!");
    } else {
      print("dlcall : ${poline.dlcall}");
      print("dlcwarehouse : ${poline.dlcwarehouse}");
      DateFormat _format = DateFormat("dd/MM/yyyy");
      DateTime _nowDate = _format.parse(_format.format(DateTime.now()));
      DateTime _tempDate = _format.parse(expDate);
      DateTime _mfgdate = _tempDate.subtract(Duration(days: poline.dlcall));
      var acceptExp = _tempDate.difference(_nowDate).inDays;
      setState(() {
        mfgController.text = _format.format(_mfgdate);
        if (acceptExp < poline.dlcwarehouse) {
          alert(context, "warning", "Warning", "UBD Date is Over DC% Accept");
        }
        qtyFocusNode.requestFocus();
      });
    }
  }

  Future<void> _selectMfg(String mfgDate) async {
    if (poline.inorder == null) {
      alert(context, "error", "Warning", "Supplier Po Is Not Empty");
    } else if (poline.inorder.isEmpty) {
      alert(context, "error", "Warning", "Please Scan Barcode");
    } else if (mfgDate.isEmpty) {
      alert(context, "error", "Warning", "Please Select EXP Date");
    } else if (poline.article.isEmpty) {
      alert(context, "error", "Warning", "Please Scan Barcode!");
    } else {
      print("dlcall : ${poline.dlcall}");
      print("dlcwarehouse : ${poline.dlcfactory}");
      DateFormat _format = DateFormat("dd/MM/yyyy");
      DateTime _nowDate = _format.parse(_format.format(DateTime.now()));
      DateTime _tempDate = _format.parse(mfgDate);
      DateTime _expdate = _tempDate.add(Duration(days: poline.dlcall));
      int acceptMfg = _expdate.difference(_nowDate).inDays;
      print("acceptMfg: $acceptMfg");
      setState(() {
        expController.text = _format.format(_expdate);
        if (acceptMfg < poline.dlcfactory) {
          alert(context, "warning", "Warning", "UBD Date is Over DC% Accept");
        }

        qtyFocusNode.requestFocus();
      });
    }
  }

  bool isNumeric(String s) {
    if (s == null) {
      return false;
    }
    return double.tryParse(s) != null;
  }

  bool isNumber(String s) {
    if (s == null) {
      return false;
    }
    return int.tryParse(s) != null;
  }

  Future<void> _confirm() async {
    if (poline.inorder.isEmpty) {
      alert(context, "error", "Warning", "Supplier Po Is Not Empty");
    } else if (poline.article == null || poline.article.isEmpty) {
      alert(context, "error", "Warning", "Please Scan Barcode!");
    } else if (qtyController.text.isEmpty) {
      alert(context, "error", "Warning", "Please Enter Receive Qty");
    } else if (!isNumber(qtyController.text)) {
      alert(context, "error", "Warning", "Quantity must more than 0");
    } else if (int.parse(qtyController.text) < 0) {
      alert(context, "error", "Warning", " Quantity must more than 0");
    } else if (docknoController.text.isEmpty) {
      alert(context, "error", "Warning", " Dock receipt must be setp before");
    } else if (poline.isbatchno == 1 && this.batchController.text.isEmpty) {
      alert(context, "error", "Warning", " Batch no is require");
    } else if (poline.isunique == 1 && this.serialController.text.isEmpty) {
      alert(context, "error", "Warning", " Serial no is require");
    } else if (poline.isdlc == 1 && this.mfgController.text.isEmpty) {
      alert(context, "error", "Warning", "MFG date is require");
    } else if (poline.isdlc == 1 && this.expController.text.isEmpty) {
      alert(context, "error", "Warning", "Expire date is require");
    } else {
      final _radio = radio.firstWhere((s) => s.valopnfirst == poline.unitreceipt);
      final _radiohu = radio.firstWhere((s) => s.valopnfirst == '5');
      final _skurec = int.parse(qtyController.text) * int.parse(_radio.value);
      final _purec = int.parse(qtyController.text);
      final _hurec = _skurec / int.parse(_radiohu.value);
      final _nowdate = DateTime.now();
      final _format = DateFormat("dd/MM/yyyy");

      if (_purec > poline.qtysku) {
        alert(context, "error", "Warning", "Quantity more than order > 0");
      } else {
        setState(() => isLoading = true);
        try {
          // generate save parameter
          final playload = SavePlayload(
            lnix: 0,
            orgcode: poline.orgcode,
            site: poline.site,
            depot: poline.depot,
            spcarea: poline.spcarea,
            inorder: poline.inorder,
            inln: poline.inln,
            inrefno: poline.inrefno,
            inrefln: poline.inrefln,
            barcode: poline.barcode,
            article: poline.article,
            pv: poline.pv,
            lv: poline.lv,
            unitops: poline.unitops,
            qtyskurec: _skurec,
            qtypurec: _purec,
            qtyhurec: _hurec.truncate().toInt(),
            qtyweightrec: 0,
            qtynaturalloss: 0,
            daterec: _nowdate,
            datemfg: mfgController.text.isEmpty ? null : _format.parse(mfgController.text),
            dateexp: expController.text.isEmpty ? null : _format.parse(expController.text),
            batchno: batchController.text.isEmpty ? null : batchController.text,
            lotno: batchController.text.isEmpty ? null : batchController.text,
            serialno: serialController.text.isEmpty ? null : serialController.text,
            datecreate: _nowdate,
            accncreate: poline.accncreate,
            datemodify: _nowdate,
            accnmodify: profile.accncode,
            procmodify: poline.procmodify,
            inagrn: poline.inagrn,
            inseq: poline.inseq,
          );

          await service.confirm(playload);
          await _searchpo(suppoController.text);
          alert(context, "success", "Receive", "  Confirm line receipt success");
          resetScanproduct();
          barcodeFocusNode.requestFocus();
          print("Saved!");
        } catch (e) {
          alert(context, "error", "Error", "${e.message}");
        }
      }
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
    resetAllScreen();
    _getunit();
  }

  @override
  void dispose() {
    suppoController.dispose();
    docknoController.dispose();
    barcodeController.dispose();
    serialController.dispose();
    batchController.dispose();
    mfgController.dispose();
    expController.dispose();
    qtyController.dispose();
    suppoFocusNode.dispose();
    docknoFocusNode.dispose();
    barcodeFocusNode.dispose();
    qtyFocusNode.dispose();
    serialFocusNode.dispose();
    mfgFocusNode.dispose();
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

  // void unFocus() {
  //   FocusScope.of(context).unfocus();
  //   //FocusScope.of(context).requestFocus(new FocusNode());
  // }

  Widget buildScreen(BuildContext context) {
    var suppoTextFiled = TextField(
      enabled: eanblescanpo,
      controller: suppoController,
      focusNode: suppoFocusNode,
      keyboardType: TextInputType.number,
      onSubmitted: (pono) async {
        await _searchpo(pono);
      },
      decoration: Txtheme.deco(
        icon: Icons.domain,
        label: "PO No : ",
        suffix: decodeType(searchpo.spcarea),
        enabled: eanblescanpo,
      ),
    );

    var supnameTextFiled = searchpo.thcode == null
        ? SizedBox(height: 1)
        : Container(
            width: MediaQuery.of(context).size.width,
            padding: EdgeInsets.symmetric(
              vertical: 5,
              horizontal: 10,
            ),
            child: RichText(
              text: TextSpan(
                text: "${searchpo.thcode ?? ""} ${searchpo.thname ?? ""}",
                style: TextStyle(color: primaryColor),
              ),
            ),
          );

    var recdateTextFiled = searchpo.daterec == null
        ? SizedBox(height: 1)
        : Padding(
            padding: EdgeInsets.symmetric(vertical: 5, horizontal: 10),
            child: Text(
              "Rec. Date ${searchpo.daterec ?? ""}",
              style: TextStyle(color: dangerColor),
            ),
          );

    var dockTextFiled = TextField(
      enabled: enableStaging,
      focusNode: docknoFocusNode,
      controller: docknoController,
      onSubmitted: (dockno) async {
        if (dockno.isNotEmpty) {
          await _staging(dockno);
        }
      },
      decoration: Txtheme.deco(
        icon: Icons.account_balance,
        label: "Staging : ",
        enabled: enableStaging,
      ),
    );
    var barcodeTextField = TextField(
      enabled: enablescanbar,
      keyboardType: TextInputType.number,
      focusNode: barcodeFocusNode,
      controller: barcodeController,
      onSubmitted: (barcode) async {
        // unFocus();
        await _scanbarcode(barcode);
      },
      decoration: Txtheme.deco(
        icon: Icons.qr_code,
        label: "Barcode : ",
        enabled: enablescanbar,
      ),
    );
    var batchTextFiled = TextField(
      enabled: poline.isbatchno == 1 ? true : false,
      controller: batchController,
      focusNode: batchFocusNode,
      decoration: Txtheme.deco(
        label: "Batch: ",
        enabled: poline.isbatchno == 1 ? true : false,
      ),
    );
    var serialTextField = TextField(
      enabled: poline.isunique == 1 ? true : false,
      focusNode: serialFocusNode,
      controller: serialController,
      decoration: Txtheme.deco(
        label: "Serial: ",
        enabled: poline.isunique == 1 ? true : false,
      ),
    );
    var mfgTextField = TextField(
      enabled: poline.isdlc == 1 ? true : false,
      focusNode: mfgFocusNode,
      controller: mfgController,
      decoration: Txtheme.deco(
        label: "MFG: ",
        enabled: poline.isdlc == 1 ? true : false,
      ),
      onTap: () async {
        if (poline.isdlc != 1) return;
        await showDatePicker(
          context: context,
          initialDate: mfgController.text.isEmpty ? DateTime.now() : DateFormat("dd/MM/yyyy").parse(mfgController.text),
          firstDate: DateTime(DateTime.now().year - 10, 1),
          lastDate: DateTime(DateTime.now().year + 20, 12),
          builder: (BuildContext context, Widget picker) {
            return DatePickTheme(picker: picker);
          },
        ).then(
          (selectedDate) {
            print("selectedDate : $selectedDate");
            if (selectedDate != null) {
              mfgController.text = DateFormat(
                'dd/MM/yyyy',
              ).format(selectedDate);

              _selectMfg(mfgController.text);
            } else {
              expController.text = "";
              mfgController.text = "";
            }
          },
        );
      }, // ontab,
    );
    var expTextField = TextField(
      enabled: poline.isdlc == 1 ? true : false,
      controller: expController,
      focusNode: expFocusNode,
      decoration: Txtheme.deco(
        label: "EXP: ",
        enabled: poline.isdlc == 1 ? true : false,
      ),
      onTap: () async {
        if (poline.isdlc != 1) return;
        await showDatePicker(
          context: context,
          initialDate: expController.text.isEmpty ? DateTime.now() : DateFormat("dd/MM/yyyy").parse(expController.text),
          firstDate: DateTime(DateTime.now().year - 10, 1),
          lastDate: DateTime(DateTime.now().year + 20, 12),
          builder: (BuildContext context, Widget picker) {
            return DatePickTheme(picker: picker);
          },
        ).then(
          (selectedDate) {
            if (selectedDate != null) {
              expController.text = DateFormat(
                'dd/MM/yyyy',
              ).format(selectedDate);
              _selectExp(expController.text);
            } else {
              mfgController.text = "";
              expController.text = "";
            }
          },
        );
      }, // ontab
    );
    var qtyTextField = TextField(
      enabled: enablescanbar,
      focusNode: qtyFocusNode,
      controller: qtyController,
      keyboardType: TextInputType.number,
      autocorrect: true,
      decoration: Txtheme.deco(
        icon: Icons.shopping_bag_outlined,
        label: "Quantity : ",
        suffix: "${poline.unitopsdesc ?? ""}",
        enabled: enablescanbar,
      ),
    );
    var confirmButton = ElevatedButton.icon(
      icon: Icon(
        Icons.save_alt_outlined,
        size: 16,
      ),
      label: Text("Confirm"),
      style: ElevatedButton.styleFrom(primary: successColor),
      onPressed: enablescanbar && !enableStart
          ? () async {
              FocusScope.of(context).requestFocus(new FocusNode());
              if (searchpo.inorder == null) {
                suppoFocusNode.requestFocus();
              } else if (poline.barcode == null) {
                barcodeFocusNode.requestFocus();
              } else if (qtyController.text.isEmpty) {
                qtyFocusNode.requestFocus();
              } else {
                var conf = DialogConfirm(
                  title: "Confirm Receive",
                  content: "Do you accept to confirm receipt ?",
                  onYes: () async => await _confirm(),
                  onNo: () {},
                );

                showDialog(
                  context: context,
                  builder: (BuildContext context) => conf,
                );
              }
            }
          : null,
    );
    var startButton = ElevatedButton.icon(
      icon: Icon(
        CupertinoIcons.time,
        size: 14,
      ),
      label: Text("Start"),
      style: ElevatedButton.styleFrom(primary: infoColor),
      onPressed: enableStart
          ? () async {
              // unFocus();
              if (searchpo.inorder == null) {
                suppoFocusNode.requestFocus();
              } else {
                var conf = DialogConfirm(
                  title: "Start Unloading",
                  content: "Do you start loading receipt ?",
                  onYes: () async => await _startload(),
                  onNo: () {},
                );

                showDialog(
                  context: context,
                  builder: (BuildContext context) => conf,
                );
              }
            }
          : null,
    );
    Container taskTable(BuildContext context) {
      const colTextStyle = TextStyle(fontSize: 12);
      const blueStyle = TextStyle(color: primaryColor);
      const redStyle = TextStyle(color: dangerColor);
      // const greenStyle = TextStyle(color: successColor);
      const dataStyle = TextStyle(fontSize: 12, color: primaryColor);
      var rowColor = MaterialStateColor.resolveWith((states) => Colors.white);
      return Container(
          height: MediaQuery.of(context).size.height / 2,
          width: MediaQuery.of(context).size.width,
          padding: EdgeInsets.all(1),
          decoration: BoxDecoration(
            border: Border.all(color: Color(0xFFCCCCCC)),
            borderRadius: BorderRadius.all(Radius.circular(5.0)),
            color: Colors.white,
            // borderRadius: BorderRadius.circular(15),
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
                columns: [
                  DataColumn(label: SizedBox(width: 50, child: Text("Product", style: colTextStyle))),
                  DataColumn(label: Text("LV", style: colTextStyle)),
                  DataColumn(
                    label: SizedBox(width: 100, child: Text("Description", style: colTextStyle)),
                  ),
                  DataColumn(label: Text("Status", style: colTextStyle)),
                  DataColumn(label: Text("PO PU", style: colTextStyle), numeric: true),
                  DataColumn(label: Text("PO SKU", style: colTextStyle), numeric: true)
                ],
                rows: searchpo.lines
                    .map(
                      (item) => DataRow(
                        cells: [
                          DataCell(Text(item.article, style: redStyle)),
                          DataCell(Text(item.lv.toString())),
                          DataCell(Text(item.description, style: blueStyle)),
                          DataCell(
                            Text(item.tflow == 'IO' ? 'Active' : 'Confirm', style: blueStyle),
                          ),
                          DataCell(
                            Text(item.qtypu.toString(), style: redStyle),
                          ),
                          DataCell(
                            Text(item.qtysku.toString(), style: colTextStyle),
                          )
                        ],
                      ),
                    )
                    .toList(),
              ),
            ),
          ));
    }

    AppBar appBar(BuildContext context) {
      return AppBar(
        leadingWidth: 50,
        leading: IconButton(
          onPressed: () => Navigator.pop(context),
          icon: Icon(CupertinoIcons.home, size: 20),
        ),
        title: Text(
          'Receive',
          style: TextStyle(fontWeight: FontWeight.bold),
        ),
        actions: <Widget>[
          IconButton(
            icon: const Icon(CupertinoIcons.refresh_circled),
            onPressed: () => resetAllScreen(),
          ),
          IconButton(
            icon: const Icon(
              CupertinoIcons.arrow_up_right_circle,
              color: colorBlue,
            ),
            onPressed: () {
              // unFocus();
              if (searchpo.inorder == null) {
                alert(context, "error", "Warning", "Supplier Po Is Not Empty");
              } else if (searchpo.inorder.isEmpty) {
                alert(context, "error", "Warning", "Supplier Po Is Not Empty");
              } else {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                      builder: (context) => ReceiveDetailScreen(
                            pono: suppoController.text,
                            profile: profile,
                          )),
                );
              }
            },
          ),
        ],
      );
    }

    Card productCard() {
      final productDescription = Padding(
        padding: EdgeInsets.only(top: 8),
        child: Column(
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
            SizedBox(height: 10),
            // Text("${poline.description ?? ""}", style: productDesc),
            // SizedBox(height: 5),
            // Text(
            //     "${poline.article ?? ""}  ${poline.pv ?? ""}  ${poline.lv ?? ""} : ${poline.barcode ?? ""}",
            //     style: productStyle),
          ],
        ),
      );
      return Card(
        elevation: 0,
        color: iconBgColor,
        margin: EdgeInsets.zero,
        child: Column(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            productDescription,
            // Divider(color: Colors.grey),
            Divider(color: Colors.grey[80], height: 10),
            Padding(
              padding: const EdgeInsets.only(top: 3, bottom: 10),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  CardLabel(title: poline.unitopsdesc, subTitle: "PU Code"),
                  CardLabel(title: product.rtoskuofpu, subTitle: "SKU/PU"),
                  CardLabel(title: product.rtoskuofhu, subTitle: "SKU/Pal"),
                  CardLabel(title: product.rtopckoflayer, subTitle: "PCK/Lay"),
                  CardLabel(title: product.rtolayerofhu, subTitle: "Lay/Pal"),
                  CardLabel(title: product.rtopckofpallet, subTitle: "PCK/Pal"),
                ],
              ),
            ),
            SizedBox(height: 10)
          ],
        ),
      );
    }

    return Scaffold(
      appBar: appBar(context),
      body: SafeArea(
          child: SingleChildScrollView(
        child: Container(
          padding: const EdgeInsets.only(left: 20, right: 20, bottom: 30),
          child: Column(mainAxisAlignment: MainAxisAlignment.start, crossAxisAlignment: CrossAxisAlignment.start, children: [
            SizedBox(height: 10),
            suppoTextFiled,
            SizedBox(height: 10),
            supnameTextFiled,
            recdateTextFiled,
            SizedBox(height: 10),
            Row(
              children: [
                Expanded(child: dockTextFiled),
                SizedBox(width: 10),
                startButton,
              ],
            ),
            SizedBox(height: 5),
            barcodeTextField,
            SizedBox(height: 10),
            productCard(),
            Row(
              children: [
                Expanded(child: batchTextFiled),
                SizedBox(width: 5),
                Expanded(child: serialTextField),
              ],
            ),
            SizedBox(height: 10),
            Row(
              children: [
                Expanded(child: mfgTextField),
                SizedBox(width: 5),
                Expanded(child: expTextField),
              ],
            ),
            SizedBox(height: 10),
            Row(
              children: [
                Expanded(child: qtyTextField),
                SizedBox(width: 10),
                confirmButton,
              ],
            ),
            // SizedBox(height: 20),
            // Text("Pending Receive", style: tsStyle),
            SizedBox(height: 20),
            searchpo == null ? Container() : taskTable(context),
          ]),
        ),
      )),
    );
  }
}
