// import 'package:wms/models/ReceiveItems.dart';
import 'package:flutter/cupertino.dart';
import 'package:wms/components/dialogalert_component.dart';
import 'package:wms/components/dialogconfirm_component.dart';
import 'package:wms/components/input_decoration.dart';
import 'package:wms/components/progress_component.dart';
import 'package:flutter/material.dart';
import 'package:wms/screens/distribution/models/empty_model.dart';
import 'package:wms/screens/distribution/serivces/distribute_services.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:wms/screens/receive/models/searchpo_model.dart';

import '../../constants.dart';

class GenEmptyScreen extends StatefulWidget {
  static const routeName = '/receiveDetail';
  final Dialog dialog = Dialog();
  final Profiles profile;
  GenEmptyScreen({this.profile});

  @override
  State<StatefulWidget> createState() {
    return _GenEmptyScreen(profile);
  }
}

class _GenEmptyScreen extends State<GenEmptyScreen> {
  _GenEmptyScreen(this.profile);
  Profiles profile;
  TextStyle rowStyle = new TextStyle(fontSize: 10, color: primaryColor);
  DistributeServices service = DistributeServices();
  TextEditingController storeController = TextEditingController();
  TextEditingController qtyController = TextEditingController();

  SearchPO searchpo = SearchPO();
  int recqty = 0;
  bool isLoading = false;
  bool ispoline = false;
  String hutypeSelect;

  List<Empty> hutype = <Empty>[];
  Future<void> gethutype() async {
    try {
      setState(() => isLoading = true);
      final _filter = EmptyFilter(hutype: 'MS');
      final _hutype = await service.listEmpty(_filter);
      setState(() {
        isLoading = false;
        hutype = _hutype;
      });
    } catch (e) {
      alert(context, "error", "Error", e.toString());
    }
  }

  Future<void> genEmpty() async {
    try {
      if (hutypeSelect.isEmpty) {
        alert(context, "warning", "Warning", "HU type is required");
      } else if (storeController.text.isEmpty) {
        alert(context, "warning", "Warning", "Store Code is required!");
      } else if (qtyController.text.isEmpty) {
        alert(context, "warning", "Warning", "Quantity is required!");
      } else {
        setState(() => isLoading = true);

        EmptyGen empty = new EmptyGen(crsku: 0, crvolume: 0, crweight: 0, huno: hutypeSelect, hutype: 'XE', spcarea: "XD", thcode: storeController.text, loccode: storeController.text, routeno: storeController.text, priority: 0, quantity: int.parse(qtyController.text), mxsku: 9999, mxweight: 999999999);
        await service.genEmpty(empty);
        alert(context, "success", "Information", "generate huno for store ${storeController.text} success");
      }
    } catch (e) {
      alert(context, "error", "Error", e.toString());
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
    gethutype();
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
    var generateBotton = Expanded(
      child: ElevatedButton.icon(
        style: ElevatedButton.styleFrom(primary: colorBlue),
        label: Text("Generate"),
        icon: Icon(
          Icons.check_circle,
          color: Colors.white,
          size: 16,
        ),
        onPressed: () async {
          var conf = DialogConfirm(
            title: "Confirm",
            content: "Do you confirm to generate handling unit ?",
            onYes: () async => await genEmpty(),
            onNo: () {},
          );

          showDialog(
            context: context,
            builder: (BuildContext context) => conf,
          );
        },
      ),
    );
    var footerButtons = Container(
      margin: EdgeInsets.only(left: 30, right: 30),
      width: MediaQuery.of(context).copyWith().size.width,
      child: Row(
        children: [
          generateBotton,
        ],
      ),
    );
    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: Icon(CupertinoIcons.chevron_back),
          onPressed: () => Navigator.of(context).pop(),
        ),
        title: Column(children: [
          Text(
            'Handling unit',
            style: TextStyle(fontWeight: FontWeight.bold),
          )
        ]),
        titleSpacing: 0,
      ),
      body: SingleChildScrollView(
        child: Container(
          child: Padding(
            padding: const EdgeInsets.only(left: 30, top: 10, right: 30, bottom: 10),
            child: Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
              // TextField(
              //   readOnly: true,
              //   controller: TextEditingController(text: "XD Empty"),
              //   decoration: Txtheme.deco(
              //     icon: Icons.widgets_outlined,
              //     label: "Hu Type : ",
              //   ),
              // ),
              SizedBox(height: 10),
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Center(
                    child: Text(
                      "Generate Handling Unit type",
                      style: TextStyle(
                        fontSize: 16,
                      ),
                    ),
                  ),
                  SizedBox(height: 20),
                  Container(
                    height: 35,
                    padding: EdgeInsets.all(8),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(50),
                      color: Colors.white,
                      border: Border.all(width: 1, color: Colors.grey),
                    ),
                    child: Row(
                      children: [
                        SizedBox(
                          width: 45,
                          child: Text(
                            "Type  ",
                            style: TextStyle(color: dangerColor, fontSize: 13),
                          ),
                        ),
                        Expanded(
                          child: DropdownButton(
                            isExpanded: true,
                            isDense: true,
                            underline: Container(color: Colors.transparent),
                            value: hutypeSelect,
                            items: hutype
                                .map((el) => DropdownMenuItem(
                                    child: Padding(
                                      padding: EdgeInsets.only(left: 0),
                                      child: Text(
                                        el.huno,
                                        style: TextStyle(fontSize: 13),
                                      ),
                                    ),
                                    value: el.huno))
                                .toList(),
                            onChanged: (value) {
                              setState(() {
                                hutypeSelect = value;
                              });
                            },
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              SizedBox(height: 10),
              TextField(
                controller: storeController,
                decoration: Txtheme.deco(
                    // icon: Icons.store,
                    label: " Store  "),
              ),
              SizedBox(height: 10),
              TextField(
                controller: qtyController,
                keyboardType: TextInputType.number,
                decoration: Txtheme.deco(
                  // icon: Icons.book_online,
                  label: " Qty     ",
                ),
              ),
              SizedBox(height: 20),
              footerButtons
            ]),
          ),
        ),
      ),
      // persistentFooterButtons: <Widget>[footerButtons],
    );
  }
}
