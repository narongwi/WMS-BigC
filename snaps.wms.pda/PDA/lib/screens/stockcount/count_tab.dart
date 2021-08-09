import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:intl/intl.dart';
import 'package:wms/components/datepick_theme.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/dialogconfirm_component.dart';
import 'package:wms/components/input_decoration.dart';
import 'package:wms/components/progress_component.dart';
import 'package:wms/constants.dart';
import 'package:wms/models/lov_model.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:wms/screens/receive/models/product_active.dart';
import 'package:wms/screens/stockcount/models/countline_model.dart';
import 'package:wms/screens/stockcount/models/findcount_model.dart';
import 'package:wms/screens/stockcount/services/count_services.dart';
import 'package:wms/screens/stockcount/sheet_tab.dart';
import 'package:wms/services/lov_services.dart';

import 'count_screen.dart';
import 'count_selection.dart';
import 'models/countplan_model.dart';

class CountTab extends StatefulWidget {
  @override
  State<StatefulWidget> createState() {
    return _CountTab();
  }
}

class _CountTab extends State<CountTab> with SingleTickerProviderStateMixin {
  String unitcount = "";
  String nextLocation = "";
  int prevLocseq = -1;
  bool specialloc = false;
  final CountServices sv = CountServices();
  bool isLoading = false;
  // bool isScanHU = false;

  Profiles profile = Profiles();
  Product product = Product();
  List<Countline> countSheet = <Countline>[];

  Countline lineOps = Countline();
  List<Lov> lov = <Lov>[];

  bool requireScanhu = false;

  Future<void> countUnit() async {
    try {
      LovService lovSerivce = new LovService();
      lov = await lovSerivce.getUnit();
      SheetTab.lov = lov;
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // get line when scan hu
  Future<void> scanloc(String _loccode) async {
    try {
      setState(() {
        // isScanHU = false;
        isLoading = true;
        requireScanhu = false;
        specialloc = false;
      });
      if (currentPlan == null) {
        alert(context, "warning", "Warning", "please select plan count");
        setState(() => locfocusNode.requestFocus());
      } else if (_loccode.isEmpty) {
        alert(context, "warning", "Warning", "please enter location");
        setState(() => locfocusNode.requestFocus());
      } else {
        lineOps = Countline();

        // Todo 1 Find Product
        final _findloc = await sv.findCountLine(
          FindCountLine(
            orgcode: currentPlan.orgcode,
            site: currentPlan.site,
            depot: currentPlan.depot,
            countcode: currentPlan.countcode,
            plancode: currentPlan.plancode,
            loccode: _loccode,
            tflow: currentPlan.tflow,
          ),
        );
        // Todo 2 Check is location
        if (_findloc.length == 0) {
          print("Todo 2");

          alert(context, "error", "Error", "Location was not found");
          setState(() => locfocusNode.requestFocus());
          return;
        }

        // Todo 2.1 Empty location
        final locOps1 = _findloc.first;

        // ! setnext location

        // check is picking or reserve
        if (locOps1.locctype == 'P' || locOps1.locctype == 'R') {
          specialloc = false;
        } else {
          // bulk , sinbin , rtv , etc ...
          // fore scan all
          currentPlan.allowscanhu = 1;
          requireScanhu = true;
          specialloc = true;

          setState(() {
            // lineOps = locOps1;
            isLoading = false;
            currentPlan.allowscanhu = 1;
            requireScanhu = true;
            specialloc = true;
            product = Product();

            scanbarController.text = "";
            scanhuController.text = "";
            batchController.text = "";
            expController.text = "";
            qtyController.text = "";
            barfocusNode.requestFocus();
          });

          return;
        }

        if ((locOps1.sthuno ?? "").isEmpty) {
          print("Todo 2.1");
          setState(() {
            lineOps = locOps1;
            isLoading = false;
            requireScanhu = true;
            product = Product();
            defaultControl(locOps1);
          });
          qtyfocusNode.requestFocus();
          // Todo Next Location
          getNextLocation();
          return;
        }

        // Todo 3 Empty location
        if ((locOps1.cnflow ?? "").isEmpty && locOps1.sthuno.isEmpty) {
          print("Todo 3");
          setState(() {
            lineOps = locOps1;
            isLoading = false;
            requireScanhu = true;
            product = Product();
            defaultControl(locOps1);
          });
          qtyfocusNode.requestFocus();
          // Todo Next Location
          getNextLocation();
          return;
        }

        // Todo 4 new count without stock
        if ((locOps1.cnflow ?? "").isEmpty && locOps1.starticle.isEmpty) {
          print("Todo 4");

          setState(() {
            lineOps = locOps1;
            isLoading = false;
            product = Product();
            requireScanhu = true;
            defaultControl(locOps1);
            // allow scan barcode
            if (currentPlan?.allowscanhu == 0) {
              barfocusNode.requestFocus();
            } else {
              qtyfocusNode.requestFocus();
            }
            // Todo Next Location
            getNextLocation();
            return;
          });
        }
        // Todo 5 new count with stock
        if ((locOps1.cnflow ?? "").isEmpty && locOps1.starticle.isNotEmpty) {
          print("Todo 5");
          setState(() => isLoading = true);
          final _product = await sv.getProduct(locOps1.starticle);
          if (_product == null) {
            alert(context, "warning", "Warning", "Product not found");
            setState(() => locfocusNode.requestFocus());
            return;
          }

          setState(() {
            isLoading = false;
            lineOps = locOps1;
            product = _product;
            lineOps.cnbarcode = _product.barcode;
            lineOps.cnarticle = _product.article;
            lineOps.cnpv = _product.pv;
            lineOps.cnlv = _product.lv;
            defaultControl(locOps1);
            // allow scan barcode
            if (currentPlan?.allowscanhu == 0) {
              barfocusNode.requestFocus();
            } else {
              scanbarController.text = _product.barcode;
              qtyfocusNode.requestFocus();
            }
          });

          // Todo Next Location
          getNextLocation();
          return;
        }

        // Todo 6 already without stock
        if ((locOps1.cnflow ?? "").isEmpty && locOps1.cnarticle.isEmpty) {
          print("Todo 6");

          setState(() {
            isLoading = false;
            lineOps = locOps1;
            requireScanhu = true;
            defaultControl(locOps1);
            // allow scan barcode
            if (currentPlan?.allowscanhu == 0) {
              barfocusNode.requestFocus();
            } else {
              qtyfocusNode.requestFocus();
            }
          });
          // Todo Next Location
          getNextLocation();
          return;
        }

        // Todo 7 already count with stock
        if ((locOps1.cnflow ?? "").isNotEmpty && locOps1.cnarticle.isNotEmpty) {
          print("Todo 7");
          setState(() => isLoading = true);
          final _product = await sv.getProduct(locOps1.cnarticle);
          if (_product == null) {
            alert(context, "warning", "Warning", "Product not found");
            setState(() => locfocusNode.requestFocus());
          }

          setState(() {
            isLoading = false;
            lineOps = locOps1;
            product = _product;
            lineOps.cnbarcode = _product.barcode;
            lineOps.cnarticle = _product.article;
            lineOps.cnpv = _product.pv;
            lineOps.cnlv = _product.lv;
            defaultControl(locOps1);
            // allow scan barcode
            if (currentPlan?.allowscanhu == 0) {
              barfocusNode.requestFocus();
            } else {
              scanbarController.text = _product.barcode;
              qtyfocusNode.requestFocus();
            }
          });

          // Todo Next Location
          getNextLocation();
          return;
        }
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  void getNextLocation() {
    if (countSheet.length == 0) return;
    if ((lineOps.loccode ?? "").isEmpty) return;
    prevLocseq = lineOps.locseq;
    final _sheet = countSheet;
    int _startseq = lineOps.locseq;

    int _nextIndex = _sheet.indexWhere(
      (x) => x.locseq > _startseq && (x.cnflow ?? "") == "",
    );
    print("_nextIndex_one $_nextIndex");

    // re check is no count
    if (_nextIndex == -1) {
      _nextIndex = _sheet.indexWhere(
        (x) =>
            x.locseq > 0 &&
            x.locseq != lineOps.locseq &&
            (x.cnflow ?? "") == "",
      );
      print("_reIndex_two $_nextIndex");
    }

    print("_nextIndex : $_nextIndex");
    if (_nextIndex == -1 || _nextIndex > _sheet.length - 1) {
      setState(() => nextLocation = "");
      print("Finish Seq Location");
    } else {
      setState(() => nextLocation = countSheet[_nextIndex].loccode);
      print("Next Location $nextLocation");
    }
  }

  void displayControl(Countline m) {
    setState(() {
      scanbarController.text = m.cnbarcode;
      scanhuController.text = m.cnhuno;
      batchController.text = m.cnlotmfg;
      expController.text = _formatDate(m.cndateexp);
      mfgController.text = _formatDate(m.cndatemfg);
      qtyController.text = m.cnqtypu.toString();
      unitcount = decodeUnit(m.unitcount);
    });
  }

  void defaultControl(Countline m) {
    setState(() {
      scanbarController.text = product.barcode;
      scanhuController.text = (m.cnhuno ?? "").isEmpty ? m.sthuno : m.cnhuno;
      lineOps.cnhuno = (m.cnhuno ?? "").isEmpty ? m.sthuno : m.cnhuno;

      batchController.text =
          (m.cnlotmfg ?? "").isEmpty ? m.stlotmfg : m.cnlotmfg;

      lineOps.cnlotmfg = (m.cnlotmfg ?? "").isEmpty ? m.stlotmfg : m.cnlotmfg;

      expController.text =
          _formatDate(m.cndateexp == null ? m.stdateexp : m.cndateexp);
      lineOps.cndateexp = m.cndateexp == null ? m.stdateexp : m.cndateexp;
      mfgController.text =
          _formatDate(m.cndatemfg == null ? m.stdatemfg : m.cndatemfg);
      lineOps.cndateexp = m.cndatemfg == null ? m.stdateexp : m.cndateexp;
      qtyController.text = (m.cnflow ?? "").isEmpty && m.locctype == "R"
          ? m.stqtypu.toString()
          : m.cnqtypu.toString();
      lineOps.cnqtypu =
          (m.cnflow ?? "").isEmpty && m.locctype == "R" ? m.stqtypu : m.cnqtypu;
      unitcount = decodeUnit(m.unitcount);
    });
  }

  String _formatDate(dynamic _date) {
    if (_date == null || _date == '') {
      return "";
    } else {
      return DateFormat('dd/MM/yyyy').format(_date);
    }
  }

  Future<void> scanbar(String productCode) async {
    try {
      setState(() {
        isLoading = true;
        scanhuController.text = "";
      });

      if (currentPlan == null) {
        alert(context, "warning", "Warning", "please select plan count");
        setState(() => locfocusNode.requestFocus());
      } else if (scanLocController.text.isEmpty) {
        alert(context, "warning", "Warning", "please enter location");
        setState(() => locfocusNode.requestFocus());
      } else if (productCode.isEmpty) {
        alert(context, "warning", "Warning", "please scan barcode");
        setState(() => barfocusNode.requestFocus());
      } else {
        // get product radio master

        Product _product = await sv.getProduct(productCode);
        if (_product == null) {
          alert(context, "warning", "Warning", "Product not found");
          setState(() => barfocusNode.requestFocus());
          return;
        }
        setState(() {
          isLoading = false;
          product = _product;
          // scanbarController.text = product.barcode;
          lineOps.cnbarcode = product.barcode;
          lineOps.cnarticle = product.article;
          lineOps.cnpv = product.pv ?? 0;
          lineOps.cnlv = product.lv ?? 0;

          if (specialloc == true) {
            hufocusNode.requestFocus();
          } else if (lineOps.starticle == product.article) {
            qtyfocusNode.requestFocus();
          } else {
            hufocusNode.requestFocus();
          }
        });
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> scanhu(String scanhuno) async {
    try {
      setState(() => isLoading = true);
      if (currentPlan == null) {
        alert(context, "warning", "Warning", "please select plan count");
        setState(() => locfocusNode.requestFocus());
      } else if (scanLocController.text.isEmpty) {
        alert(context, "warning", "Warning", "location is required !");
        setState(() => locfocusNode.requestFocus());
      } else if ((product.article ?? "").isEmpty) {
        alert(context, "warning", "Warning", "Proudct is required !");
        setState(() => barfocusNode.requestFocus());
      } else if (scanhuno.isEmpty) {
        alert(context, "warning", "Warning", "HU No  is required !");
        setState(() => hufocusNode.requestFocus());
      } else {
        if (specialloc) {
          // check exists in system
          final _huno = await sv.findHU(scanhuno);
          if (_huno.length == 0) {
            alert(context, "error", "Error", "HU not exists in system");
            setState(() => hufocusNode.requestFocus());
            return;
          }

          // get line in plan
          final _linOps = countSheet.firstWhere(
            (x) =>
                x.loccode == scanLocController.text &&
                x.sthuno == scanhuController.text,
            orElse: () => null,
          );

          if (_linOps == null) {
            alert(context, "warning", "Warning",
                "HU: $scanhuno \n not found in plan !");

            // wrong pallet
            final _firstline = countSheet.first;
            final _newLine = Countline();
            _newLine.orgcode = _firstline.orgcode;
            _newLine.spcarea = _firstline.spcarea;
            _newLine.site = _firstline.site;
            _newLine.depot = _firstline.depot;
            _newLine.countcode = _firstline.countcode;
            _newLine.plancode = _firstline.plancode;
            _newLine.locctype = _firstline.locctype;
            _newLine.loccode = _firstline.loccode;
            _newLine.locseq = 0;
            _newLine.datecreate = _firstline.datecreate;
            _newLine.procmodify = _firstline.procmodify;
            _newLine.cnbarcode = product.barcode;
            _newLine.cnarticle = product.article;
            _newLine.cnpv = product.pv ?? 0;
            _newLine.cnlv = product.lv ?? 0;
            _newLine.unitcount = product.unitmanage ?? 1;
            _newLine.cnhuno = scanhuno;
            _newLine.tflow = "NW";

            setState(() {
              isLoading = false;
              lineOps = _newLine;
              qtyfocusNode.requestFocus();
            });
          } else {
            setState(() {
              isLoading = false;
              lineOps = _linOps;
              qtyfocusNode.requestFocus();
            });
          }
        } else {
          // check huno
          if (countSheet
                  .where((e) =>
                      (e.cnhuno == scanhuno || e.sthuno == scanhuno) &&
                      e.loccode != lineOps.loccode)
                  .length >
              0) {
            alert(context, "warning", "Warning",
                "HU: $scanhuno \nis duplicate !");

            return;
          }

          final _huno = await sv.findHU(scanhuno);
          if (_huno.length == 0) {
            alert(context, "error", "error", "HU not exists in system");
            setState(() => hufocusNode.requestFocus());
            return;
          }

          setState(() {
            isLoading = false;
            lineOps.cnhuno = scanhuno;
            qtyfocusNode.requestFocus();
          });
        }

        // Todo Next Location
        getNextLocation();
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> saveCount() async {
    try {
      if (qtyController.text.isEmpty) {
        qtyController.text = "0";
        return;
      }

      final _countqty = int.parse(qtyController.text);
      if (currentPlan.countcode == null) {
        alert(context, "warning", "Warning", "please select tasks");
      } else if (currentPlan.plancode == null) {
        alert(context, "warning", "Warning", "please select plan count");
      } else if (scanLocController.text.isEmpty) {
        alert(context, "warning", "Warning", "please scan location");
      } else if (_countqty > 0 && (product.barcode ?? "").isEmpty) {
        alert(context, "warning", "Warning", "please scan barcode");
      } else if (scanbarController.text != lineOps.cnbarcode) {
        alert(context, "warning", "Warning", "please scan barcode");
        print("LineOps=>${lineOps.cnbarcode}");
        print("scanbarController=>${scanbarController.text}");
        setState(() {
          lineOps.cnarticle = "";
          scanbarController.text = "";
          product = new Product();
          barfocusNode.requestFocus();
        });
        // if chanage text and save
      } else if (scanhuController.text != lineOps.cnhuno) {
        alert(context, "warning", "Warning", "please scan Huno");

        setState(() {
          lineOps.cnhuno = "";
          scanhuController.text = "";
          hufocusNode.requestFocus();
        });
      } else {
        if (_countqty > 0 && (lineOps.cnarticle ?? "").isEmpty) {
          setState(() => barfocusNode.requestFocus());
          alert(context, "warning", "Warning", "Product is required !");
          return;
        } else if ((product.article ?? "").isNotEmpty &&
            (lineOps.cnhuno ?? "").isEmpty) {
          setState(() => hufocusNode.requestFocus());
          alert(context, "warning", "Warning", "Huno is required !");
          return;
        }

        setState(() => isLoading = true);
        var countLine = <Countline>[];
        lineOps.cnlotmfg = batchController.text;
        DateFormat _format = DateFormat("dd/MM/yyyy");

        // DLC Control
        if (currentPlan.isdateexp == 1) {
          lineOps.cndateexp = expController.text.isNotEmpty
              ? _format.parse(expController.text)
              : null;
        }

        if (currentPlan.isdatemfg == 1) {
          lineOps.cndatemfg = mfgController.text.isNotEmpty
              ? _format.parse(mfgController.text)
              : null;
        }

        lineOps.cnqtypu = int.parse(qtyController.text);

        if (checkNewLine()) {
          lineOps.cnflow = "NW";
          lineOps.unitcount = product.unitmanage;
          print("Set New Line");
        }

        countLine.add(lineOps);

        // save count
        await sv.saveCount(countLine);

        alert(context, "success", "Save Result", "Save Line Count Success");
        clearCount();

        // refresh count sheet
        await getCountSheet(currentPlan);
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  bool checkNewLine() {
    if (lineOps.starticle.isEmpty) {
      return false;
    } else if (lineOps.starticle == lineOps.cnarticle &&
        lineOps.sthuno == lineOps.cnhuno) {
      return false;
    } else {
      return true;
    }
  }

  void clearScreen() {
    setState(() {
      isLoading = false;
      specialloc = false;
      lineOps = new Countline();
      currentPlan = new Countplan();
      product = Product();
      scanLocController.text = "";
      scanbarController.text = "";
      scanhuController.text = "";
      batchController.text = "";
      expController.text = "";
      mfgController.text = "";
      qtyController.text = "";
      locfocusNode.requestFocus();
      unitcount = "";
      CountScreen.currentCountCode = currentPlan.countcode;
      CountScreen.currentPlanCount = currentPlan.plancode;
      SheetTab.countPlan = new Countplan();
      SheetTab.countSheet = [];
    });
  }

  void clearCount() {
    setState(() {
      isLoading = false;
      lineOps = new Countline();
      product = Product();
      scanLocController.text = "";
      scanbarController.text = "";
      scanhuController.text = "";
      batchController.text = "";
      expController.text = "";
      mfgController.text = "";
      qtyController.text = "";
      locfocusNode.requestFocus();
      unitcount = "";
    });
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

  Color planState(String tflow) {
    try {
      switch (tflow) {
        case "XX":
          return Colors.red;
        case "ED":
          return Colors.green;
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
    setState(() {
      currentPlan = SheetTab.countPlan;
      countSheet = SheetTab.countSheet;
    });

    countUnit();
  }

  @override
  void dispose() {
    scanLocController.dispose();
    scanhuController.dispose();
    scanbarController.dispose();
    batchController.dispose();
    serialController.dispose();
    expController.dispose();
    mfgController.dispose();
    qtyController.dispose();

    locfocusNode.dispose();
    hufocusNode.dispose();
    barfocusNode.dispose();
    batchfocusNode.dispose();
    expfocusNode.dispose();
    mfgfocusNode.dispose();
    qtyfocusNode.dispose();
    super.dispose();
  }

  Future<bool> clearList() async {
    SheetTab.countPlan = null;
    SheetTab.lov.clear();
    SheetTab.countSheet.clear();
    Navigator.pop(context);
    return true;
  }

  @override
  Widget build(BuildContext context) {
    // get profile arguments
    profile = ModalRoute.of(context).settings.arguments as Profiles;
    return WillPopScope(
      onWillPop: () async => await clearList(),
      child: ProgressContainer(
        child: buildScreen(context),
        inAsyncCall: isLoading,
        opacity: 0.3,
      ),
    );
  }

  final scanLocController = TextEditingController();
  final scanhuController = TextEditingController();
  final scanbarController = TextEditingController();
  final batchController = TextEditingController();
  final serialController = TextEditingController();
  final expController = TextEditingController();
  final mfgController = TextEditingController();
  final qtyController = TextEditingController();
  final locfocusNode = FocusNode();
  final hufocusNode = FocusNode();
  final barfocusNode = FocusNode();
  final batchfocusNode = FocusNode();
  final expfocusNode = FocusNode();
  final mfgfocusNode = FocusNode();
  final qtyfocusNode = FocusNode();
  Countplan currentPlan = Countplan();
  Widget buildScreen(BuildContext context) {
    var locfromTextField = TextField(
      onSubmitted: (value) async {
        if (value.isNotEmpty) {
          await scanloc(value);
        }
      },
      controller: scanLocController,
      focusNode: locfocusNode,
      textAlign: TextAlign.right,
      decoration: Txtheme.deco(
          icon: Icons.search,
          label: "LOC ",
          suffix: "${lineOps?.locctype ?? ""}"),
    );
    var hunoTextField = TextField(
      controller: scanhuController,
      focusNode: hufocusNode,
      textAlign: TextAlign.right,
      decoration: Txtheme.deco(
        icon: Icons.tab,
        label: "HU No ",
      ),
      onSubmitted: (scanhuno) async {
        if (scanhuno.isNotEmpty) {
          await scanhu(scanhuno);
        }
      },
    );

    var barcodeTextField = TextField(
      onSubmitted: (value) async {
        if (value.isNotEmpty) {
          await scanbar(value);
        }
      },
      controller: scanbarController,
      focusNode: barfocusNode,
      textAlign: TextAlign.right,
      decoration: Txtheme.deco(
        icon: Icons.fit_screen,
        label: "Barcode ",
      ),
    );
    var batchTextField = TextField(
      onSubmitted: (value) async {
        if (value.isNotEmpty) {}
      },
      enabled: currentPlan?.isbatchno == 1 ? true : false,
      controller: batchController,
      focusNode: batchfocusNode,
      decoration: Txtheme.deco(
        icon: Icons.bookmark,
        label: "Batch No ",
        enabled: currentPlan?.isbatchno == 1 ? true : false,
      ),
    );
    var expTextField = TextField(
      enabled: currentPlan?.isdateexp == 1 ? true : false,
      controller: expController,
      focusNode: expfocusNode,
      textAlign: TextAlign.right,
      decoration: Txtheme.deco(
        icon: Icons.calendar_today,
        label: "Expire Date ",
        enabled: currentPlan?.isdateexp == 1 ? true : false,
      ),
      onTap: () async {
        await showDatePicker(
          context: context,
          initialDate: expController.text.isEmpty
              ? DateTime.now()
              : DateFormat("dd/MM/yyyy").parse(expController.text),
          firstDate: DateTime(DateTime.now().year - 10, 1),
          lastDate: DateTime(DateTime.now().year + 20, 12),
          builder: (BuildContext context, Widget picker) {
            return DatePickTheme(picker: picker);
          },
        ).then(
          (selectedDate) {
            if (selectedDate != null) {
              expController.text =
                  DateFormat('dd/MM/yyyy').format(selectedDate);
              // _selectExp(expController.text);
            } else {
              mfgController.text = "";
              expController.text = "";
            }
          },
        );
      }, // ontab
    );
    // var mfgTextField = TextField(
    //   enabled: plan.isdatemfg == 1 ? true : false,
    //   controller: mfgController,
    //   focusNode: mfgfocusNode,
    //   decoration: Txtheme.deco(
    //     label: "Mfg : ",
    //     enabled: plan.isdatemfg == 1 ? true : false,
    //   ),
    //   onTap: () async {
    //     await showDatePicker(
    //       context: context,
    //       initialDate: mfgController.text.isEmpty
    //           ? DateTime.now()
    //           : DateFormat("dd/MM/yyyy").parse(mfgController.text),
    //       firstDate: DateTime(DateTime.now().year - 10, 1),
    //       lastDate: DateTime(DateTime.now().year + 20, 12),
    //       builder: (BuildContext context, Widget picker) {
    //         return DatePickTheme(picker: picker);
    //       },
    //     ).then(
    //       (selectedDate) {
    //         print("selectedDate : $selectedDate");
    //         if (selectedDate != null) {
    //           mfgController.text = DateFormat(
    //             'dd/MM/yyyy',
    //           ).format(selectedDate);

    //           // _selectMfg(mfgController.text);
    //         } else {
    //           expController.text = "";
    //           mfgController.text = "";
    //         }
    //       },
    //     );
    //   },
    // );

    var qtyTextField = TextField(
      controller: qtyController,
      focusNode: qtyfocusNode,
      keyboardType: TextInputType.number,
      textAlign: TextAlign.center,
      decoration: Txtheme.deco(
        icon: Icons.save,
        label: "Quantity ",
        suffix: "${unitcount ?? 'SKU'}",
      ),
    );

    var confirmLocButton = ButtonTheme(
      height: 30.0,
      child: ElevatedButton.icon(
          style: ElevatedButton.styleFrom(primary: successColor),
          icon: Icon(Icons.save_alt, size: 13),
          label: Text("Save"),
          onPressed: currentPlan != null
              ? () async {
                  var conf = DialogConfirm(
                    title: "Confirm Count",
                    content: "do you confirm count line ?",
                    onYes: () async => await saveCount(),
                    onNo: () {},
                  );

                  showDialog(
                    context: context,
                    builder: (BuildContext context) => conf,
                  );
                }
              : null),
    );

    return Scaffold(
      appBar: AppBar(
        leadingWidth: 50,
        leading: IconButton(
          onPressed: () async => await clearList(),
          icon: Icon(CupertinoIcons.home, size: 20),
        ),
        title: Text("Stok Count"),
        actions: <Widget>[
          IconButton(
            icon: const Icon(CupertinoIcons.refresh_circled),
            onPressed: () {
              clearCount();
            },
          ),
          IconButton(
            icon: const Icon(CupertinoIcons.search_circle),
            onPressed: () async {
              clearScreen();
              final selPlan = await Navigator.push(
                context,
                MaterialPageRoute(builder: (context) => CountSelection()),
              );
              if (selPlan != null) {
                setState(() {
                  currentPlan = selPlan as Countplan;
                  CountScreen.currentCountCode = currentPlan.countcode;
                  CountScreen.currentPlanCount = currentPlan.plancode;
                });
                await getCountSheet(selPlan as Countplan);
              }
            },
          ),
        ],
      ),
      body: SingleChildScrollView(
        scrollDirection: Axis.vertical,
        child: Column(
          children: [
            Container(
              padding: EdgeInsets.only(left: 10, right: 10),
              child: Row(
                children: [
                  Expanded(
                    flex: 2,
                    child: ListTile(
                      title: Text(
                        "${currentPlan?.planname ?? "No Plan"}",
                        style: TextStyle(fontSize: 16, color: colorSeeds),
                      ),
                      subtitle: Text(
                        "Type: ${currentPlan?.countType ?? ""}",
                        style: TextStyle(fontSize: 11, color: colorBlue),
                      ),
                    ),
                  ),
                  Expanded(
                    child: ListTile(
                      title: Text(
                        "${currentPlan?.plancode ?? "0"}",
                        textAlign: TextAlign.center,
                        style: TextStyle(
                            fontSize: 16,
                            color: colorSeeds,
                            fontWeight: FontWeight.bold),
                      ),
                      subtitle: Text(
                        "Plan Code",
                        textAlign: TextAlign.center,
                        style: TextStyle(fontSize: 11, color: colorBlue),
                      ),
                    ),
                  )
                ],
              ),
            ),
            Card(
              elevation: 0,
              color: iconBgColor,
              margin: EdgeInsets.only(left: 20, bottom: 10, right: 20),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.start,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  locfromTextField,
                  SizedBox(height: 10),
                  barcodeTextField,
                  SizedBox(height: 20),
                  Row(
                    children: [
                      SizedBox(
                          width: 60,
                          child: Text(
                            "Product",
                            style: TextStyle(fontSize: 12),
                          )),
                      Text(
                        "${product.article ?? ""} ${product.lv ?? ""}",
                        style: TextStyle(color: colorPoppy, fontSize: 12),
                      )
                    ],
                  ),
                  SizedBox(height: 10),
                  (product.descalt ?? "").isEmpty
                      ? Text(
                          "Product Description",
                          style: TextStyle(
                              fontSize: 12,
                              color: Colors.grey,
                              fontStyle: FontStyle.italic),
                        )
                      : Text(
                          "${product.descalt ?? ""}",
                          style: TextStyle(fontSize: 12, color: colorPoppy),
                        ),

                  SizedBox(height: 15),
                  hunoTextField,
                  SizedBox(height: 10),
                  batchTextField,
                  SizedBox(height: 10),
                  expTextField,
                  // Row(
                  //   children: [
                  //     Expanded(child: mfgTextField),
                  //     SizedBox(width: 5),
                  //     Expanded(child: expTextField),
                  //   ],
                  // ),
                  SizedBox(height: 10),
                  Row(
                    children: [
                      Expanded(child: qtyTextField),
                      SizedBox(width: 10),
                      confirmLocButton
                    ],
                  ),
                  SizedBox(height: 5),
                  Row(
                    children: [
                      Text(
                        "Next",
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey,
                        ),
                      ),
                      SizedBox(
                        width: 10,
                      ),
                      Text(
                        nextLocation,
                        style: TextStyle(
                          color: colorSeeds,
                          fontSize: 13,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  )
                ],
              ),
            ),
            // Padding(
            //   padding: const EdgeInsets.symmetric(horizontal: 15),
            //   child: Row(
            //     children: [
            //       Expanded(child: confirmLocButton),
            //     ],
            //   ),
            // ),
          ],
        ),
      ),
    );
  }

  Future<void> getCountSheet(Countplan slplan) async {
    try {
      var _lines = await sv.countLine(slplan);
      if (_lines.length > 0) {
        _lines.forEach((element) {
          element.unitcount = decodeUnit(element.unitcount);
        });

        // initail count sheet
        countSheet = _lines;
        SheetTab.countSheet = _lines;
        SheetTab.countPlan = slplan;
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }
}
