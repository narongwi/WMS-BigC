import 'dart:core';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/progress_component.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:wms/screens/loosepick/models/prep_filter.dart';
import 'package:wms/screens/loosepick/services/prep_services.dart';

import '../../constants.dart';
import 'models/prep_lists.dart';

class PrepListScreen extends StatefulWidget {
  @override
  State<StatefulWidget> createState() {
    return _PrepListScreen();
  }
}

class _PrepListScreen extends State<PrepListScreen> {
  final rowStyle = new TextStyle(fontSize: 13, color: dangerColor);
  final colStyle = new TextStyle(fontSize: 13, color: primaryColor);
  bool isLoading = false;
  String prepnosl;
  PrepLists selPrep;
  List<PrepLists> preps = <PrepLists>[];
  LoosePickServices service = LoosePickServices();

  Future<void> _list() async {
    try {
      prepnosl = "";
      setState(() => isLoading = true);
      final _filter = PrepFilter(spcarea: 'ST', preptype: 'P', tflow: 'IO');
      final _prepls = await service.listprep(_filter);
      setState(() {
        isLoading = false;
        preps = _prepls;
      });
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
    _list();
  }

  @override
  Widget build(BuildContext context) {
    return ProgressContainer(child: buildScreen(context), inAsyncCall: isLoading, opacity: 0.3);
  }

  Widget buildScreen(BuildContext context) {
    final double pageWidth = MediaQuery.of(context).size.width;
    return Scaffold(
      appBar: AppBar(
        leadingWidth: 35,
        leading: IconButton(
          onPressed: () => Navigator.pop(context, new PrepLists()),
          icon: Icon(CupertinoIcons.arrow_left),
        ),
        title: Text(
          'Perparation List',
          style: TextStyle(fontWeight: FontWeight.bold),
        ),
        actions: <Widget>[
          IconButton(
            icon: const Icon(CupertinoIcons.arrow_2_circlepath_circle, color: colorBlue),
            onPressed: () async => await _list(),
          ),
          IconButton(
            icon: Icon(
              CupertinoIcons.checkmark_alt_circle,
              color: colorBlue,
            ),
            onPressed: (prepnosl ?? "").isNotEmpty
                ? () {
                    Navigator.pop(context, selPrep);
                  }
                : null,
          ),
        ],
      ),
      body: Container(
        padding: EdgeInsets.zero,
        height: MediaQuery.of(context).size.height,
        width: MediaQuery.of(context).size.width,
        // decoration: BoxDecoration(
        //   border: Border.all(color: Color(0xFFCCCCCC)),
        //   borderRadius: BorderRadius.all(Radius.circular(5.0)),
        //   color: Colors.white,
        // ),
        child: SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: SingleChildScrollView(
            scrollDirection: Axis.vertical,
            child: DataTable(
                // dataRowHeight: 30,
                dataTextStyle: rowStyle,
                columnSpacing: 10,
                // headingRowHeight: 50,
                headingRowHeight: 0,
                headingRowColor: MaterialStateColor.resolveWith((states) => Colors.white),
                dataRowColor: MaterialStateProperty.resolveWith<Color>((Set<MaterialState> states) {
                  if (states.contains(MaterialState.selected)) return Theme.of(context).colorScheme.primary.withOpacity(0.09);
                  return null; // Use the default value.
                }),
                columns: [
                  DataColumn(
                    label: SizedBox(
                      width: pageWidth * 1.3,
                      child: null,
                    ),
                  ),
                  //   DataColumn(label: Text("HU No.", style: colStyle)),
                  //   DataColumn(label: Text("Route", style: colStyle)),
                  //   DataColumn(label: SizedBox(width: 300, child: Text("Customer", style: colStyle))),
                ],
                showCheckboxColumn: false,
                rows: preps
                    .map((item) => DataRow(
                            selected: prepnosl == item.prepno,
                            onSelectChanged: (b) {
                              setState(() {
                                prepnosl = item.prepno;
                                selPrep = item;
                              });
                            },
                            cells: [
                              DataCell(SizedBox(
                                width: pageWidth,
                                child: Column(
                                  mainAxisAlignment: MainAxisAlignment.start,
                                  children: [
                                    Container(
                                      alignment: Alignment.topLeft,
                                      padding: EdgeInsets.only(left: 8, top: 5, right: 8),
                                      child: Row(
                                        children: [
                                          Text("Prep  :", style: TextStyle(fontWeight: FontWeight.bold, fontSize: 12, color: defaultColor)),
                                          SizedBox(width: 5),
                                          Text("${item.prepno}", style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14, color: primaryColor)),
                                          SizedBox(width: 10),
                                          Text("HU: ${item.huno}", style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14)),
                                        ],
                                      ),
                                    ),
                                    Container(
                                      padding: EdgeInsets.only(left: 8, top: 2, bottom: 5, right: 8),
                                      alignment: Alignment.topLeft,
                                      child: Row(
                                        children: [
                                          Text("Route :", style: TextStyle(fontWeight: FontWeight.bold, fontSize: 12, color: defaultColor)),
                                          SizedBox(width: 5),
                                          Text("${item.routeno}", style: TextStyle(fontWeight: FontWeight.bold, fontSize: 12, color: dangerColor)),
                                          SizedBox(width: 5),
                                          Text("${item.thcode} ${item.thname}", style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: primaryColor)),
                                        ],
                                      ),
                                    ),
                                  ],
                                ),
                              )),
                              //   DataCell(Text(item.huno,
                              //       style: TextStyle(
                              //         fontSize: 12,
                              //       ))),
                              //   DataCell(Text(item.routeno,
                              //       style: TextStyle(
                              //         fontSize: 12,
                              //       ))),
                              //   DataCell(
                              //     SizedBox(
                              //       width: 300,
                              //       child: Text("${item.thcode}-${item.thname}",
                              //           textAlign: TextAlign.left,
                              //           style: TextStyle(
                              //             fontSize: 12,
                              //           )),
                              //     ),
                              //   ),
                            ]))
                    .toList()),
          ),
        ),
      ),
      persistentFooterButtons: <Widget>[
        Container(
          width: MediaQuery.of(context).size.width,
          padding: EdgeInsets.all(8),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                "Prep no ",
              ),
              SizedBox(width: 10),
              Text(
                prepnosl ?? "",
                style: TextStyle(
                  color: Colors.red,
                  fontWeight: FontWeight.bold,
                ),
              ),
              SizedBox(width: 10),
              Text("selected"),
            ],
          ),
        ),
      ],
    );
  }
}
