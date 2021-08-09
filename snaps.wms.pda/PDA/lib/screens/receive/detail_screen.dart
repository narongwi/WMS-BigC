// import 'package:wms/models/ReceiveItems.dart';
import 'package:flutter/cupertino.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/dialogconfirm_component.dart';
import 'package:wms/components/progress_component.dart';
import 'package:flutter/material.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:wms/screens/receive/models/searchpo_line.dart';
import 'package:wms/screens/receive/models/searchpo_model.dart';
import 'package:wms/screens/receive/services/receive_services.dart';

import '../../constants.dart';

class ReceiveDetailScreen extends StatefulWidget {
  static const routeName = '/receiveDetail';
  final String pono;
  final Dialog dialog = Dialog();
  final Profiles profile;
  ReceiveDetailScreen({this.pono, this.profile});

  @override
  State<StatefulWidget> createState() {
    return _ReceiveDetailScreen(pono, profile);
  }
}

class _ReceiveDetailScreen extends State<ReceiveDetailScreen> {
  _ReceiveDetailScreen(this.currentPO, this.profile);
  String currentPO = "";
  Profiles profile;
  TextStyle rowStyle = new TextStyle(fontSize: 10, color: primaryColor);
  ReceiveService service = ReceiveService();
  SearchPO searchpo = SearchPO();
  int recqty = 0;
  bool isLoading = false;
  bool ispoline = false;

  @override
  void initState() {
    super.initState();
    setState(() {
      searchpo = SearchPO();
      searchpo.lines = <SearchPOLines>[];
      _getpo();
    });
  }

  @override
  void dispose() {
    super.dispose();
  }

  Future<void> _getpo() async {
    setState(() => isLoading = true);
    try {
      SearchPO _searchpo = await service.searchPO(currentPO);
      ispoline = _searchpo.lines.toList().length > 0 ? true : false;
      if (_searchpo.tflow == 'ED') {
        await alert(context, "info", "Information", "PO is already Closed");
        Navigator.of(context).pop();
      } else {
        setState(() {
          recqty = _searchpo.lines.map((e) => e.qtypurec).reduce(
                (v, e) => v + e,
              );

          print("rec total : $recqty");
          searchpo = _searchpo;
          isLoading = false;
        });
      }
    } catch (e) {
      alert(context, "error", "Error", "${e.message}");
    }
  }

  Future<void> _finish() async {
    if (searchpo.inorder == null || searchpo.inorder == '') {
      alert(context, "error", "Warning", "PO No is Required");
    } else if (!ispoline) {
      alert(context, "error", "Warning", "PO Item Not Found");
    } else {
      try {
        setState(() => isLoading = true);

        currentPO = searchpo.inorder;
        await service.setFinished(searchpo.inorder);
        alert(context, "success", "Finishload", "finish unload success");
        setState(() {
          currentPO = searchpo.inorder;
          searchpo.tflow = "SE";
        });
        // refresh
        await _getpo();
      } catch (e) {
        alert(context, "error", "Error", "${e.message}");
      }
    }
  }

  Future<void> _close() async {
    if (searchpo.inorder == null || searchpo.inorder == '') {
      alert(context, "error", "Warning", "PO No is Required");
    } else if (!ispoline) {
      alert(context, "error", "Warning", "PO Item Not Found");
    } else {
      try {
        setState(() => isLoading = true);
        await service.setClose(searchpo.inorder);
        await alert(context, "success", "Close Order", "Close order success ");
        setState(() => isLoading = false);
        Navigator.of(context).pop();
      } catch (e) {
        alert(context, "error", "Error", "${e.message}");
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
  Widget build(BuildContext context) {
    return ProgressContainer(
      child: buildScreen(context),
      inAsyncCall: isLoading,
      opacity: 0.3,
    );
  }

  Widget buildScreen(BuildContext context) {
    var finishBotton = Expanded(
      child: ElevatedButton.icon(
        style: ElevatedButton.styleFrom(primary: successColor),
        label: Text("Finish Load"),
        icon: Icon(
          Icons.check,
          color: Colors.white,
        ),
        onPressed: () async {
          var conf = DialogConfirm(
            title: "Finish Load",
            content: "Do you confirm finish unload? ",
            onYes: () async => await _finish(),
            onNo: () {},
          );

          showDialog(
            context: context,
            builder: (BuildContext context) => conf,
          );
        },
      ),
    );

    var closeBotton = Expanded(
      child: ElevatedButton.icon(
        style: ElevatedButton.styleFrom(primary: warningColor),
        label: Text("Close Order"),
        icon: Icon(Icons.close),
        onPressed: () async {
          var conf = DialogConfirm(
              title: "Close Order",
              content: "Do you confirm close order ?",
              onYes: () async => await _close(),
              onNo: () {});

          showDialog(
            context: context,
            builder: (BuildContext context) => conf,
          );
        },
      ),
    );
    var backButton = Expanded(
      child: ElevatedButton.icon(
        style: ElevatedButton.styleFrom(primary: infoColor),
        icon: Icon(Icons.arrow_back),
        label: Text("Back"),
        onPressed: () {
          Navigator.of(context).pop();
        },
      ),
    );
    var footerButtons = Container(
      margin: EdgeInsets.only(left: 30, right: 30),
      width: MediaQuery.of(context).copyWith().size.width,
      child: Row(
        children: [
          recqty > 0 && searchpo.tflow != "SE"
              ? finishBotton
              : searchpo.tflow == "SE"
                  ? closeBotton
                  : backButton
        ],
      ),
    );

    const colTextStyle = TextStyle(fontSize: 12);
    // const greenStyle = TextStyle(color: successColor);
    const dataStyle = TextStyle(fontSize: 12, color: primaryColor);
    var rowColor = MaterialStateColor.resolveWith((states) => Colors.white);

    var dataColumns = [
      DataColumn(label: Text("Product", style: colTextStyle)),
      DataColumn(label: Text("LV", style: colTextStyle)),
      DataColumn(label: Text("Description", style: colTextStyle)),
      // DataColumn(label: Text("Status", style: colTextStyle)),
      DataColumn(label: Text("PO PU", style: colTextStyle), numeric: true),
      DataColumn(label: Text("REC.PU", style: colTextStyle), numeric: true),
    ];

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: Icon(CupertinoIcons.chevron_back),
          onPressed: () => Navigator.of(context).pop(),
        ),
        title: Column(children: [Text('Receive')]),
        titleSpacing: 0,
      ),
      body: Container(
        height: MediaQuery.of(context).size.height,
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
                  columns: dataColumns,
                  rows: searchpo.lines.map((item) {
                    var dataRow = DataRow(cells: [
                      DataCell(Text(item.article,
                          style: TextStyle(color: dangerColor))),
                      DataCell(Text("${item.lv}")),
                      DataCell(
                          SizedBox(width: 200, child: Text(item.description))),
                      // DataCell(Text(item.tflow,
                      //     style: TextStyle(color: Colors.blue))),
                      DataCell(Text("${item.qtypu}",
                          style: TextStyle(color: dangerColor))),
                      DataCell(Text("${item.qtypurec}",
                          style: TextStyle(color: dangerColor)))
                    ]);
                    return dataRow;
                  }).toList())),
        ),
      ),
      persistentFooterButtons: <Widget>[footerButtons],
    );
  }
}
