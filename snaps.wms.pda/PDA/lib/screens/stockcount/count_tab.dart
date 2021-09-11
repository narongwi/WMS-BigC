import 'dart:convert';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:intl/intl.dart';
import 'package:wms/components/datepick_theme.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/dialogconfirm_component.dart';
import 'package:wms/components/input_decoration.dart';
import 'package:wms/components/progress_component.dart';
import 'package:wms/constants.dart';
import 'package:wms/models/lov_model.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:wms/screens/stockcount/models/countline_model.dart';
import 'package:wms/screens/stockcount/models/findcount_model.dart';
import 'package:wms/screens/stockcount/models/productvld_model.dart';
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
  String unitDesrt = "";
  String locatioinType = "";
  String lineBarcode = "";
  String nextLocation = "";

  int prevLocseq = -1;
  bool specialloc = false;
  bool isLoading = false;
  bool isLineCounted = false;
  bool isLineBarcode = false;
  bool requireScanhu = false;
  bool allowScanProduct = false;
  bool requireScanProduct = false;
  bool requireGeneratehu = false;
  Profiles profile = Profiles();
  Productvld lineProduct = Productvld();
  CountServices sv = CountServices();
//   Countline countline = Countline();
  List<Countline> countSheet = <Countline>[];
  List<Lov> lov = <Lov>[];

  Future<void> listUnitCount() async {
    try {
      LovService lovSerivce = new LovService();
      lov = await lovSerivce.getUnit();
      SheetTab.lov = lov;
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> scanLocaSubmit(String _loccode) async {
    if (curPlan == null) {
      alert(context, "warning", "Warning", "please select plan count");
      setState(() => locfocusNode.requestFocus());
    } else if (_loccode.isEmpty) {
      alert(context, "warning", "Warning", "please enter location");
      setState(() => locfocusNode.requestFocus());
    } else {
      // * initialize
      setState(() {
        requireScanProduct = false;
        requireScanhu = false;
        requireGeneratehu = false;
        specialloc = false;
        unitDesrt = "";
        isLoading = true;
      });

      // * get product master
      final respline = await sv.findCountLine(
        FindCountLine(countcode: curPlan.countcode, plancode: curPlan.plancode, loccode: _loccode, tflow: curPlan.tflow),
      );

      // * Location not found
      if (respline.length == 0) {
        alert(context, "error", "Error", "Location was not found");
        setState(() => locfocusNode.requestFocus());
      } else {
        setState(() {
          isLoading = false;
          locatioinType = respline.first.locctype;
          specialloc = (locatioinType == 'P' || locatioinType == 'R') ? false : true;
        });

        // ? Single count line
        if (respline.length == 1) {
          Countline line = respline.first;
          await bindingControls(line);
          // Todo show next location
          getNextLocation(line.locseq);

          if (!isLineCounted) {
            if (scanhuController.text.isEmpty) {
              setState(() => hufocusNode.requestFocus());
            } else {
              if (allowScanProduct || requireScanProduct) {
                setState(() => barfocusNode.requestFocus());
              } else {
                setState(() => qtyfocusNode.requestFocus());
              }
            }
          } else {
            setState(() => qtyfocusNode.requestFocus());
          }
        }
        // ? Multiple count line
        else {
          Fluttertoast.showToast(msg: "Multiple Count line ", backgroundColor: colorStem);

          // Todo show next location
          if (prevLocseq == -1) {
            getNextLocation(respline.first.locseq);
          }

          // * variable
          // * call method
          // * controls
          // * bind form control
          setState(() {
            isLoading = false;
            lineProduct = Productvld();
            scanhuController.text = "";
            scanbarController.text = "";
            batchController.text = "";
            expController.text = "";
            qtyController.text = "";
            hufocusNode.requestFocus();
            requireScanhu = true; // set scan
            isLineCounted = false; // default
          });
        }
      }
    }
  }

  Future<void> scanHunoSubmit(String _hunoval) async {
    try {
      requireScanProduct = false;
      setState(() => isLoading = true);
      if (curPlan == null) {
        alert(context, "warning", "Warning", "please select plan count");
        setState(() => locfocusNode.requestFocus());
      } else if (scanLocController.text.isEmpty) {
        alert(context, "warning", "Warning", "location is required !");
        setState(() => locfocusNode.requestFocus());
      } else {
        final _locOps = scanLocController.text;
        // ? option 1 empty stock
        // * check location empty row
        if (_hunoval.trim().isEmpty) {
          final _lines = countSheet.where((x) => x.loccode == _locOps && x.sthuno.isEmpty);
          if (_lines.length > 0) {
            await bindingControls(_lines.first);
            // Todo show next location
            getNextLocation(_lines.first.locseq);

            setState(() {
              isLoading = false;
              qtyfocusNode.requestFocus();
            });
          } else {
            alert(context, "warning", "Warning", "HU is required !");
            setState(() => locfocusNode.requestFocus());
          }
        }
        // ? option 2 have stock
        else {
          // * find hu in countsheet list
          final _locOps = scanLocController.text;
          final _linOps = countSheet.where((x) => x.loccode == _locOps && (x.cnflow == 'IO' ? x.cnhuno : x.sthuno) == _hunoval);
          // ? check notfound
          if (_linOps.length == 0) {
            // alert(context, "warning", "Warning", "HU: $_hunoval \n not found !");
            Fluttertoast.showToast(msg: "Count New HU No.", backgroundColor: colorStem);
            setState(() {
              isLoading = false;
              requireScanProduct = true;
              barfocusNode.requestFocus();
            });
          }
          // ? check duplicate whit other location
          else if (countSheet.where((e) => e.cnhuno == _hunoval && e.loccode != _locOps).length > 0) {
            alert(context, "warning", "Warning", "HU: $_hunoval \nis duplicate !");
          }
          // * single product
          else if (_linOps.length == 1) {
            setState(() {
              isLoading = false;
              requireScanProduct = false;
            });

            await bindingControls(_linOps.first);

            // Todo show next location
            getNextLocation(_linOps.first.locseq);

            if (allowScanProduct || requireScanProduct) {
              setState(() {
                isLoading = false;
                barfocusNode.requestFocus();
              });
            } else {
              setState(() {
                isLoading = false;
                qtyfocusNode.requestFocus();
              });
            }
          }
          // * multiple product
          else {
            Fluttertoast.showToast(msg: "Multiple product process ", backgroundColor: colorStem);
            // todo force scan barcode
            setState(() => requireScanProduct = true);
            // * set focus
            await bindingControls(_linOps.first);

            // * set focus
            if (allowScanProduct || requireScanProduct) {
              setState(() {
                isLoading = false;
                barfocusNode.requestFocus();
              });
            } else {
              setState(() {
                isLoading = false;
                qtyfocusNode.requestFocus();
              });
            }
          }
        }
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> scanBarcSubmit(String _barcode) async {
    try {
      final _locOps = scanLocController.text;
      final _chuOps = scanhuController.text;

      if (curPlan == null) {
        alert(context, "warning", "Warning", "please seelct plan count");
        setState(() => locfocusNode.requestFocus());
      } else if (_locOps.isEmpty) {
        alert(context, "warning", "Warning", "Location is required");
        setState(() => locfocusNode.requestFocus());
      } else if (_chuOps.isEmpty && !requireGeneratehu) {
        alert(context, "warning", "Warning", "HU is required");
        setState(() => hufocusNode.requestFocus());
      } else {
        // * get product master
        setState(() => isLoading = true);
        final _product = await sv.findProduct(Productvld(barcode: _barcode, pv: -1, lv: -1, qtycount: 0, isnewhu: false, loccode: _locOps));
        if (_product == null) {
          alert(context, "warning", "Warning", "Product not found");
          setState(() {
            scanbarController.text = "";
            barfocusNode.requestFocus();
          });
        } else {
          setState(() {
            isLoading = false;
            lineProduct = _product;
            scanbarController.text = _product.barcode;
          });

          print("product=>${jsonEncode(lineProduct)}");
          final _lineOps = countSheet.firstWhere(
            (x) => x.loccode == _locOps && (x.cnflow == 'IO' ? x.cnhuno : x.sthuno) == _chuOps && (x.cnflow == 'IO' ? x.cnarticle : x.starticle) == _product.article && (x.cnflow == 'IO' ? x.cnlv : x.stlv) == _product.lv,
            orElse: () => null,
          );

          if (_lineOps != null) {
            setState(() {
              expController.text = formatDate(_lineOps.cndateexp);
              mfgController.text = formatDate(_lineOps.cndatemfg);
              qtyController.text = _lineOps.locctype == "R" && _lineOps.cnflow == "" ? _lineOps.stqtypu.toString() : _lineOps.cnqtypu.toString();
              unitDesrt = decodeUnit(_lineOps.unitcount);
              qtyfocusNode.requestFocus();
            });
          } else {
            // Fluttertoast.showToast(msg: "New count line", backgroundColor: colorStem);
            // Todo show next location
            // getNextLocation(_lineOps.locseq);

            // Todo worng product
            setState(() {
              expController.text = "";
              mfgController.text = "";
              qtyController.text = "";
              unitDesrt = decodeUnit(lineProduct.unitcount);
              qtyfocusNode.requestFocus();
            });
          }
        }
      }
    } catch (e) {
      setState(() {
        scanbarController.text = "";
        barfocusNode.requestFocus();
      });
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<Productvld> fillCountline(Countline line, bool isUnitmanage) async {
    final _format = DateFormat("dd/MM/yyyy");
    Productvld _productVld = Productvld();
    _productVld.orgcode = line.orgcode;
    _productVld.site = line.site;
    _productVld.depot = line.depot;
    _productVld.barcode = lineProduct.barcode;
    _productVld.article = lineProduct.article;
    _productVld.pv = lineProduct.pv;
    _productVld.lv = lineProduct.lv;
    _productVld.unitcount = lineProduct.unitcount;
    _productVld.lotmfg = batchController.text;
    _productVld.dateexp = curPlan.isdateexp == 1 && expController.text.trim().isNotEmpty ? _format.parse(expController.text) : line.cndateexp;
    _productVld.datemfg = curPlan.isdateexp == 1 && mfgController.text.trim().isNotEmpty ? _format.parse(mfgController.text) : line.cndatemfg;
    _productVld.serialno = "";
    _productVld.loccode = line.loccode;
    _productVld.huno = scanhuController.text.trim();
    _productVld.countcode = line.countcode;
    _productVld.plancode = line.plancode;
    _productVld.linecode = line.locseq.toString();
    _productVld.qtycount = qtyController.text.trim().isEmpty ? 0 : int.parse(qtyController.text.trim());

    if (line.cnflow == "IO") {
      // is already confirm count qty
      if (scanhuController.text.trim() == (line.cnhuno ?? "").trim() && (lineProduct.article ?? "") == (line.cnarticle ?? "") && (lineProduct.lv ?? 0) == (line.cnlv ?? 0)) {
        // no change
        _productVld.isnewhu = false;
      } else {
        // is change data
        _productVld.isnewhu = true;
      }
    } else {
      // pending not confirm count
      if (scanhuController.text.isEmpty && scanhuController.text.trim() == (line.sthuno ?? "").trim() && (lineProduct.article ?? "").trim() == (line.starticle ?? "").trim() && (lineProduct.lv ?? 0) == (line.stlv ?? 0)) {
        // no change
        _productVld.isnewhu = false;
      } else {
        // is change data
        _productVld.isnewhu = true;
      }
    }
    // if (line.cnflow == "IO" && scanhuController.text == line.cnhuno && lineProduct.article == line.cnarticle && lineProduct.lv == line.cnlv) {
    //   _productVld.isnewhu = false;
    // } else if (line.cnflow.isEmpty && scanhuController.text == line.sthuno && lineProduct.article == line.starticle && lineProduct.lv == line.stlv) {
    //   _productVld.isnewhu = false;
    // } else {
    //   _productVld.isnewhu = true;
    // }
    return _productVld;
  }

  Future<void> saveLineSubmit() async {
    try {
      final _locOps = scanLocController.text;
      final _hunOps = scanhuController.text;
      if (curPlan.countcode == null) {
        alert(context, "warning", "Warning", "Please selected count tasks");
      } else if (curPlan.plancode == null) {
        alert(context, "warning", "Warning", "Please selected plan count");
      } else if (_locOps.isEmpty) {
        alert(context, "warning", "Warning", "Location code is required");
        setState(() => locfocusNode.requestFocus());
      } else if (qtyController.text.isEmpty) {
        alert(context, "warning", "Warning", "Count Quantity is required");
        setState(() => qtyfocusNode.requestFocus());
      } else if ((lineProduct.barcode ?? "") != scanbarController.text) {
        alert(context, "warning", "Warning", "The Barcode value is Changed please try again");
        setState(() => barfocusNode.requestFocus());
      } else {
        Productvld _productVld = Productvld();

        // * Generate New HU
        if (requireGeneratehu) {
          // get location first row for copy data
          final _copyCountLine = countSheet.firstWhere((x) => x.loccode == _locOps);

          // fill objecct to model
          _productVld = await fillCountline(_copyCountLine, false);

          // Todo Calling Web api
          final _countLine = await sv.generateHU(_productVld);
          // show new hu result
          alert(context, "success", "Generate Result", "HU No : ${_countLine.cnhuno} success");

          // refresh count sheet
          await getCountSheet(curPlan);

          // Todo show next location
          getNextLocation(_countLine.locseq);

          clearCount();
        } else {
          bool _isCountline = false;

          // * step 1 check counted line
          if (!_isCountline) {
            final _countedLine = countSheet.where(
              (x) => x.cnflow == "IO" && x.loccode == _locOps && x.cnhuno == _hunOps && (x.cnarticle ?? "") == (lineProduct.article ?? "") && (x.cnlv ?? "") == (lineProduct.lv ?? ""),
            );

            if (_countedLine.length > 0) {
              _productVld = await fillCountline(_countedLine.first, false);
              _isCountline = true; // finish
            }

            print("step 1 isCountline:$_isCountline");
          }

          // * step 2 check counting line
          if (!_isCountline) {
            final _countingLine = countSheet.where(
              (x) => x.cnflow.isEmpty && x.loccode == _locOps && x.sthuno == _hunOps && (x.starticle ?? "") == (lineProduct.article ?? "") && (x.stlv ?? "") == (lineProduct.lv ?? ""),
            );

            if (_countingLine.length > 0) {
              _productVld = await fillCountline(_countingLine.first, false);
              _isCountline = true; // finish
            }

            print("step 2 isCountline:$_isCountline");
          }

          // * step 2.1 check hu is already
          if (!_isCountline) {
            final _countingLine = countSheet.where((x) => x.loccode == _locOps && (x.cnhuno ?? "").isNotEmpty && (x.cnhuno ?? "") == _hunOps && (x.starticle ?? "") != (lineProduct.article ?? ""));
            if (_countingLine.length > 0) {
              alert(context, "warning", "Warning", "huno already in use by another product");
              setState(() {
                scanbarController.text = "";
                barfocusNode.requestFocus();
              });
              return; // exit function
            }
          }

          // * step 3 check hu counted
          if (!_isCountline) {
            final _countingLine = countSheet.where((x) => x.cnflow == "IO" && x.loccode == _locOps && x.cnhuno == _hunOps);
            if (_countingLine.length > 0) {
              _productVld = await fillCountline(_countingLine.first, false);
              _isCountline = true; // finish
            }

            print("step 3 isCountline:$_isCountline");
          }

          // * step 4 check hu counting
          if (!_isCountline) {
            final _countingLine = countSheet.where((x) => x.cnflow.isEmpty && x.loccode == _locOps && (x.sthuno ?? "").trim() == (_hunOps ?? "").trim());
            if (_countingLine.length > 0) {
              _productVld = await fillCountline(_countingLine.first, false);
              _isCountline = true; // finish
            }

            print("step 4 isCountline:$_isCountline");
          }

          // * step 5 check empty stock line
          if (!_isCountline) {
            final _emptyStockLine = countSheet.where((x) => x.loccode == _locOps && x.sthuno.isEmpty && x.starticle.isEmpty);
            if (_emptyStockLine.length > 0) {
              _productVld = await fillCountline(_emptyStockLine.first, true);
              _isCountline = true; // finish
            }

            print("step 5 isCountline:$_isCountline");
          }

          // * processing count line
          final _confrimQty = int.parse(qtyController.text);
          if (!_isCountline) {
            alert(context, "error", "Warning", "invalid count line,please try again");
          } else if (_confrimQty > 0 && scanhuController.text.isEmpty) {
            alert(context, "warning", "Warning", "HU No is required");
          } else if (_confrimQty > 0 && scanbarController.text.isEmpty) {
            alert(context, "warning", "Warning", "Barcode is required");
          } else {
            // validate and save count line
            final _countLine = await sv.validateline(_productVld);

            alert(context, "success", "Save Result", "Save Linecount success");

            // * refresh count sheet
            await getCountSheet(curPlan);

            // Todo show next location
            getNextLocation(_countLine.locseq);
            clearCount();
          }
        }
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<bool> clearCountSheet() async {
    SheetTab.countPlan = null;
    SheetTab.lov.clear();
    SheetTab.countSheet.clear();
    Navigator.pop(context);
    return true;
  }

  // * local alert method
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

  void getNextLocation(int currentseq) {
    prevLocseq = currentseq;
    final _sheet = countSheet;
    if (countSheet.length == 0) return;
    if (scanLocController.text.isEmpty) return;
    int _nextIndex = _sheet.indexWhere((x) => x.locseq > currentseq && (x.cnflow ?? "") == "");
    // ! re check is no count
    if (_nextIndex == -1) {
      _nextIndex = _sheet.indexWhere((x) => x.locseq > 0 && x.locseq != currentseq && (x.cnflow ?? "") == "");
    }
    // * Display Next Location
    setState(() {
      if (_nextIndex == -1 || _nextIndex > _sheet.length - 1) {
        nextLocation = ""; // * empty with Completed
      } else {
        nextLocation = countSheet[_nextIndex].loccode; // * Show Next Location
      }
    });
  }

  Future<void> bindingControls(Countline line) async {
    // * declare vailable
    print("allowScanProduct=> $requireScanProduct");
    isLineCounted = line.cnflow.trim().isNotEmpty;
    isLineBarcode = (line.cnbarcode.isNotEmpty || line.stbarcode.isNotEmpty) ? true : false;
    lineBarcode = (line.cnbarcode.isEmpty ? line.stbarcode : line.cnbarcode);

    Productvld _product = Productvld();
    if ((!allowScanProduct || !requireScanProduct) && isLineBarcode) {
      lineBarcode = line.cnbarcode.isEmpty ? line.stbarcode : line.cnbarcode;
      _product = await sv.findProduct(Productvld(barcode: lineBarcode, pv: -1, lv: -1, qtycount: 0, isnewhu: false, loccode: line.loccode));
    }
    // print('allowScanProduct:$allowScanProduct');
    // print('requireScanProduct:$requireScanProduct');
    // print('line.unitcount:${line.unitcount}');
    // lov.forEach((element) {
    //   print('lov.unitlist:${element.desc}');
    // });

    setState(() {
      if (isLineCounted) {
        Fluttertoast.showToast(msg: "Line is Counted", backgroundColor: colorStem);
        scanhuController.text = line.cnhuno;
        expController.text = formatDate(line.cndateexp);
        mfgController.text = formatDate(line.cndatemfg);
        if ((allowScanProduct || requireScanProduct)) {
          isLineBarcode = false;
          scanbarController.text = "";
          lineBarcode = "";
          lineProduct = Productvld();
          qtyController.text = "";
          expController.text = "";
          mfgController.text = "";
          unitDesrt = "";
        } else {
          lineProduct = _product;
          scanbarController.text = line.cnbarcode;
          qtyController.text = line.cnqtypu.toString();
          expController.text = formatDate(line.cndateexp);
          mfgController.text = formatDate(line.cndatemfg);
          unitDesrt = decodeUnit(line.unitcount);
        }
      } else {
        Fluttertoast.showToast(msg: "Start Couting", backgroundColor: colorStem);
        scanhuController.text = line.sthuno;

        if (allowScanProduct || requireScanProduct) {
          isLineBarcode = false;
          requireScanProduct = true;
          scanbarController.text = "";
          lineBarcode = "";
          lineProduct = Productvld();
          expController.text = "";
          mfgController.text = "";
          qtyController.text = "";
          unitDesrt = "";
        } else {
          lineProduct = _product;
          scanbarController.text = line.cnbarcode;
          qtyController.text = line.stqtypu.toString();
          expController.text = formatDate(line.stdateexp);
          mfgController.text = formatDate(line.stdatemfg);
          unitDesrt = decodeUnit(line.unitcount);
          if (locatioinType == "R") {
            qtyController.text = line.stqtypu.toString();
          } else {
            qtyController.text = "";
          }
        }
      }
    });
  }

  void clearScreen() {
    setState(() {
      isLoading = false;
      specialloc = false;
      requireScanProduct = false;
      requireScanhu = false;
      requireGeneratehu = false;
      curPlan = new Countplan();
      lineProduct = Productvld();
      scanLocController.text = "";
      scanbarController.text = "";
      scanhuController.text = "";
      batchController.text = "";
      expController.text = "";
      mfgController.text = "";
      qtyController.text = "";
      locfocusNode.requestFocus();
      unitDesrt = "";
      locatioinType = "";
      CountScreen.currentCountCode = curPlan.countcode;
      CountScreen.currentPlanCount = curPlan.plancode;
      SheetTab.countPlan = new Countplan();
      SheetTab.countSheet = [];
    });
    // * reset next location
    getNextLocation(-1);
  }

  void clearCount() {
    setState(() {
      isLoading = false;
      allowScanProduct = false;
      requireGeneratehu = false;
      requireScanProduct = false;
      requireScanhu = false;
      lineProduct = Productvld();
      if (!specialloc) {
        scanLocController.text = "";
        locatioinType = "";
      }
      scanbarController.text = "";
      scanhuController.text = "";
      batchController.text = "";
      expController.text = "";
      mfgController.text = "";
      qtyController.text = "";
      unitDesrt = "";
      locfocusNode.requestFocus();
    });
  }

  String formatDate(dynamic _date) {
    if (_date == null || _date == '') {
      return "";
    } else {
      return DateFormat('dd/MM/yyyy').format(_date);
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

  @override
  void initState() {
    super.initState();
    setState(() {
      curPlan = SheetTab.countPlan;
      countSheet = SheetTab.countSheet;
      allowScanProduct = (curPlan?.allowscanhu ?? 0) == 0 ? false : true;
    });

    listUnitCount();
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

  @override
  Widget build(BuildContext context) {
    // get profile arguments
    profile = ModalRoute.of(context).settings.arguments as Profiles;
    return WillPopScope(
      onWillPop: () async => await clearCountSheet(),
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
  Countplan curPlan = Countplan();
  Widget buildScreen(BuildContext context) {
    var locfromTextField = TextField(
      onSubmitted: (value) async {
        if (value.isNotEmpty) {
          await scanLocaSubmit(value);
        }
      },
      controller: scanLocController,
      focusNode: locfocusNode,
      textAlign: TextAlign.right,
      // enabled: !requireGeneratehu,
      decoration: Txtheme.deco(
        icon: Icons.search,
        label: "Location ",
        suffix: "$locatioinType",
        // enabled: !requireGeneratehu,
      ),
    );
    var hunoTextField = TextField(
      controller: scanhuController,
      focusNode: hufocusNode,
      textAlign: TextAlign.right,
      enabled: !requireGeneratehu,
      decoration: Txtheme.deco(
        icon: Icons.tab,
        label: "HU ",
        enabled: !requireGeneratehu,
      ),
      onSubmitted: (scanhuno) async {
        await scanHunoSubmit(scanhuno);
      },
    );

    var barcodeTextField = TextField(
      onSubmitted: (value) async {
        if (value.isNotEmpty) {
          await scanBarcSubmit(value);
        }
      },
      // enabled: (allowScanProduct || requireScanProduct) ? true : false,
      controller: scanbarController,
      focusNode: barfocusNode,
      textAlign: TextAlign.right,
      decoration: Txtheme.deco(
        icon: Icons.fit_screen,
        label: "Barcode ",
        // enabled: (allowScanProduct || requireScanProduct) ? true : false,
      ),
    );
    var batchTextField = TextField(
      onSubmitted: (value) async {
        if (value.isNotEmpty) {}
      },
      enabled: curPlan?.isbatchno == 1 ? true : false,
      controller: batchController,
      focusNode: batchfocusNode,
      textAlign: TextAlign.right,
      decoration: Txtheme.deco(
        icon: Icons.bookmark,
        label: "Batch No ",
        prefix: "",
        suffix: "",
        enabled: curPlan?.isbatchno == 1 ? true : false,
      ),
    );
    var expTextField = TextField(
      enabled: curPlan?.isdateexp == 1 ? true : false,
      controller: expController,
      focusNode: expfocusNode,
      textAlign: TextAlign.right,
      decoration: Txtheme.deco(
        icon: Icons.calendar_today,
        label: "Expire Date",
        prefix: "",
        suffix: "",
        enabled: curPlan?.isdateexp == 1 ? true : false,
      ),
      onTap: () async {
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
              expController.text = DateFormat('dd/MM/yyyy').format(selectedDate);
              // _selectExp(expController.text);
            } else {
              mfgController.text = "";
              expController.text = "";
            }
          },
        );
      }, // ontab
    );

    var qtyTextField = TextField(
      controller: qtyController,
      focusNode: qtyfocusNode,
      textAlign: TextAlign.right,
      keyboardType: TextInputType.number,
      // enabled: (allowScanProduct || requireScanProduct) ? false : true,
      decoration: Txtheme.deco(
        // enabled: (allowScanProduct || requireScanProduct) ? false : true,
        icon: Icons.save,
        label: "Qty ",
        suffix: "${unitDesrt ?? 'SKU'}",
      ),
    );

    var confirmLocButton = ButtonTheme(
      height: 30.0,
      child: ElevatedButton.icon(
          style: ElevatedButton.styleFrom(primary: requireGeneratehu ? primaryColor : successColor),
          icon: Icon(Icons.save_alt, size: 13),
          label: Text(requireGeneratehu ? "Generate & Save" : "Save Count"),
          onPressed: curPlan != null
              ? () async {
                  var conf = DialogConfirm(
                    title: "Confirm Count line",
                    content: "do you confirm count quantity ?",
                    onYes: () async => await saveLineSubmit(),
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
          onPressed: () async => await clearCountSheet(),
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
                  curPlan = selPlan as Countplan;
                  CountScreen.currentCountCode = curPlan.countcode;
                  CountScreen.currentPlanCount = curPlan.plancode;
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
                        "${curPlan?.planname ?? "No Plan"}",
                        overflow: TextOverflow.ellipsis,
                        style: TextStyle(fontSize: 14, color: colorSeeds, fontWeight: FontWeight.bold),
                      ),
                      subtitle: Text(
                        "Type: ${curPlan?.countType ?? ""}",
                        style: TextStyle(fontSize: 11, color: colorBlue),
                      ),
                    ),
                  ),
                  Expanded(
                    child: ListTile(
                      title: Text(
                        "${curPlan?.plancode ?? "0"}",
                        textAlign: TextAlign.center,
                        style: TextStyle(fontSize: 14, color: colorSeeds, fontWeight: FontWeight.bold),
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
                  Row(
                    children: [
                      Expanded(child: locfromTextField),
                      SizedBox(width: 10),
                      SizedBox(
                        width: 50,
                        child: Column(
                          children: [
                            Text(
                              "Generate",
                              style: TextStyle(fontSize: 12),
                            ),
                            Text(
                              "NEW HU",
                              style: TextStyle(
                                fontSize: 12,
                                color: primaryColor,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                  SizedBox(height: 10),
                  Row(
                    children: [
                      Expanded(child: hunoTextField),
                      SizedBox(width: 5),
                      SizedBox(
                        width: 55,
                        child: Switch(
                          value: requireGeneratehu,
                          onChanged: (value) {
                            setState(() {
                              requireGeneratehu = value;
                              if (value) {
                                requireScanProduct = true;
                                scanhuController.text = "";
                                // requireScanProduct = false;
                              } else {
                                hufocusNode.requestFocus();
                              }
                            });
                          },
                          activeTrackColor: infoColor,
                          activeColor: primaryColor,
                        ),
                        // child: CustomSwitch(
                        //   value: requireGeneratehu,
                        //   onChanged: (bool val) {
                        //     setState(() {
                        //       requireGeneratehu = val;
                        //       if (requireGeneratehu) {
                        //         scanhuController.text = "";
                        //         requireScanProduct = true;
                        //       } else {
                        //         requireScanProduct = true;
                        //         hufocusNode.requestFocus();
                        //       }
                        //     });
                        //   },
                        // ),
                      ),
                    ],
                  ),
                  SizedBox(height: 10),
                  Row(
                    children: [
                      Expanded(child: barcodeTextField),
                      SizedBox(width: 10),
                      SizedBox(
                        width: 50,
                      ),
                    ],
                  ),

                  SizedBox(height: 15),
                  Row(
                    children: [
                      SizedBox(
                          width: 60,
                          child: Text(
                            "Product",
                            style: TextStyle(fontSize: 12),
                          )),
                      Text(
                        "${lineProduct.article ?? ""} ${lineProduct.lv ?? ""}",
                        style: TextStyle(color: colorPoppy, fontSize: 12),
                      )
                    ],
                  ),
                  SizedBox(height: 10),
                  (lineProduct.descalt ?? "").isEmpty
                      ? Text(
                          "Product Description",
                          style: TextStyle(fontSize: 12, color: Colors.grey, fontStyle: FontStyle.italic),
                        )
                      : Text(
                          "${lineProduct.descalt ?? ""}",
                          style: TextStyle(fontSize: 12, color: colorPoppy),
                        ),

                  SizedBox(height: 15),
                  Row(
                    children: [
                      Expanded(child: batchTextField),
                      SizedBox(width: 5),
                      Expanded(child: expTextField),
                    ],
                  ),
                  //   batchTextField,
                  //   SizedBox(height: 10),
                  //   expTextField,
                  SizedBox(height: 10),
                  Row(
                    children: [
                      SizedBox(
                        width: 150,
                        child: Column(
                          children: [
                            Text(
                              "",
                              style: TextStyle(
                                fontSize: 12,
                                color: primaryColor,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            Text(
                              "Next Location",
                              style: TextStyle(
                                fontSize: 12,
                              ),
                            ),
                          ],
                        ),
                      ),
                      SizedBox(width: 10),
                      Expanded(child: qtyTextField),
                      //   confirmLocButton
                    ],
                  ),
                  SizedBox(height: 5),
                  Row(
                    children: [
                      SizedBox(
                        width: 150,
                        child: Text(
                          nextLocation,
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            color: primaryColor,
                            fontSize: 14,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                      SizedBox(width: 10),
                      //  genHuButton,
                      Expanded(child: confirmLocButton),
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
        // _lines.forEach((element) {
        //   element.unitcount = decodeUnit(element.unitcount);
        // });

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
