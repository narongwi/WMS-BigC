import 'package:flutter/cupertino.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/dialogconfirm_component.dart';
import 'package:wms/components/input_decoration.dart';
import 'package:wms/screens/distribution/models/baseclose_model.dart';
import 'package:wms/screens/distribution/serivces/distribute_services.dart';
import 'package:wms/screens/home/comonents/whcard.dart';

import '../../constants.dart';
import 'package:flutter/material.dart';

class BaseClosedPage extends StatefulWidget {
  static const routeName = '/baseclosed';
  @override
  State<StatefulWidget> createState() {
    return _BaseClosedPage();
  }
}

class _BaseClosedPage extends State<BaseClosedPage> {
  DistributeServices service = DistributeServices();
  TextEditingController huEmptyController = TextEditingController();
  FocusNode hunoFocusNode = FocusNode();
  String txPUQty;
  String txWeight;
  String txVolume;
  String storeCode;
  String storeName;
  String routeno;
  String status;
  bool isLoading = false;
  bool enableBaseColse = false;
  BaseClose baseClose = BaseClose();
  // BaseCloseLine baseCloseLine = BaseCloseLine();

  @override
  void initState() {
    super.initState();
    resetScreen();
  }

  @override
  void dispose() {
    super.dispose();
    huEmptyController.dispose();
    hunoFocusNode.dispose();
  }

  Future<void> scanhu(String huno) async {
    try {
      setState(() => isLoading = true);

      final filter = BaseCloseFilter(spcarea: 'XD', hutype: 'XE', huno: huno);
      final _baseCloseList = await service.baseCloselist(filter);
      final _baseCloseLine = await service.baseCloseLine(_baseCloseList.single);

      double puQty = 0;
      double weithtQty = 0;
      double volumeQty = 0;
      _baseCloseLine.forEach((s) {
        puQty = puQty+s.qtypu;
        weithtQty = weithtQty+s.qtyweight;
        volumeQty = volumeQty+s.qtyvolume;
      });

      setState(() {
        isLoading = false;
        baseClose = _baseCloseList.single;
        // baseCloseLine = _baseColseLine.single;
        storeCode = baseClose.thcode;
        storeName = baseClose.thname;
        routeno = baseClose.routeno;
        txPUQty = puQty.toString();
        txWeight = weithtQty.toString();
        txVolume = volumeQty.toString();
        // convert status
        if (baseClose.tflow == 'IO') {
          enableBaseColse = true;
          status = "Active";
        } else {
          enableBaseColse = false;
          status = "Closed";
        }
      });
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  // confirm base colsing
  Future<void> close() async {
    try {
      if (baseClose == null) {
        alert(context, "warning", "Warning", "please scan HU.");
        hunoFocusNode.requestFocus();
      } else {
        setState(() => isLoading = true);
        await service.baseCLoseHU(baseClose);
        alert(context, "success", "Information", "End preparation success");
        setState(() {
          resetScreen();
        });
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  void resetScreen() {
    baseClose = BaseClose();
    huEmptyController.text = "";
    storeCode = "";
    storeName = "";
    routeno = "";
    status = "";
    txPUQty = "0";
    txWeight = "0.0";
    txVolume = "0.0";
    hunoFocusNode.requestFocus();
    enableBaseColse = false;
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
  Widget build(BuildContext context) {
    var appBar = AppBar(
      leadingWidth: 50,
      leading: IconButton(
        onPressed: () => Navigator.pop(context),
        icon: Icon(CupertinoIcons.home, size: 20),
      ),
      title: Text(
        "Base Closing",
        style: TextStyle(fontWeight: FontWeight.bold),
      ),
      actions: [
        IconButton(
          icon: const Icon(CupertinoIcons.plus_circle, color: colorBlue),
          onPressed: () {
            // Navigator.pop(context);
            resetScreen();
          },
        ),
      ],
    );
    var hunoTextFaild = TextField(
      controller: huEmptyController,
      keyboardType: TextInputType.number,
      decoration: Txtheme.decoration(Icons.domain, "HU No: ", "HU No: ", null),
      onSubmitted: (value) async {
        if (value.isNotEmpty) {
          await scanhu(value);
        }
      },
    );
    var confirmButton = Container(
      margin: EdgeInsets.only(left: 30, right: 30),
      width: MediaQuery.of(context).copyWith().size.width,
      child: Row(
        children: [
          Expanded(
            child: ElevatedButton.icon(
              style: ElevatedButton.styleFrom(primary: Colors.lightGreen),
              icon: Icon(
                Icons.check,
                size: 16,
              ),
              label: Text("Confirm Base Closed"),
              onPressed: enableBaseColse
                  ? () {
                      var conf = DialogConfirm(
                        title: "Confirm",
                        content: "Do you confirm close distribution empty ?",
                        onYes: () async => await close(),
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
    var vlaueStyle = TextStyle(
      fontSize: 20,
      color: colorSeeds,
      fontWeight: FontWeight.bold,
    );
    var titleStyle = TextStyle(fontSize: 12);
    var valuePadding = EdgeInsets.only(right: 5);

    var qtyText = Row(
      children: [
        Text("PU Qty", style: titleStyle),
        Spacer(),
        Text("${txPUQty ?? "0"}", style: vlaueStyle),
      ],
    );

    var weightText = Row(
      children: [
        Padding(
          padding: valuePadding,
          child: Text(
            "Weight",
            style: titleStyle,
          ),
        ),
        Spacer(),
        Text("${txWeight ?? "0"}", style: vlaueStyle),
      ],
    );
    var volumeText = Row(
      children: [
        Padding(
          padding: valuePadding,
          child: Text("Volume", style: titleStyle),
        ),
        Spacer(),
        Text(
          "${txVolume ?? "0"}",
          style: vlaueStyle,
        ),
      ],
    );
    var statusText = Row(
      children: [
        Padding(
          padding: valuePadding,
          child: Text("Status", style: titleStyle),
        ),
        Spacer(),
        Text(
          "${status ?? ""}",
          style: TextStyle(
            color: status == 'Active' ? Colors.grey : Colors.lightGreen,
            fontSize: 16,
          ),
        ),
      ],
    );
    var productDetail = Container(
      padding: EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(8),
      ),
      child: Column(
        children: [qtyText, Divider(color: Colors.grey), weightText, Divider(color: Colors.grey), volumeText, Divider(color: Colors.grey), statusText],
      ),
    );
    return Scaffold(
      appBar: appBar,
      body: SingleChildScrollView(
        child: Container(
          padding: const EdgeInsets.only(
            left: 20,
            right: 20,
            bottom: 30,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [SizedBox(height: 20), hunoTextFaild, SizedBox(height: 10), WHCard(label: "Store : ", value: storeCode ?? "", color: colorBlue, size: 14, textColor: primaryColor), SizedBox(height: 10), WHCard(label: "Name : ", value: storeName ?? "", color: colorStem, size: 14, textColor: primaryColor), SizedBox(height: 10), WHCard(label: "Route : ", value: routeno ?? "", color: colorPetal, size: 14, textColor: primaryColor), SizedBox(height: 10), productDetail],
          ),
        ),
      ),
      persistentFooterButtons: <Widget>[confirmButton],
    );
  }
}
