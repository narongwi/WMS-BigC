import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:wms/screens/home/models/profiles.dart';
import 'package:wms/screens/stockcount/count_tab.dart';
import 'package:wms/screens/stockcount/sheet_tab.dart';

import '../../constants.dart';

class CountScreen extends StatefulWidget {
  static const routeName = '/stockcount';
  static String currentCountCode = "";
  static String currentPlanCount = "";

  @override
  State<StatefulWidget> createState() {
    return _CountScreen();
  }
}

class _CountScreen extends State<CountScreen>
    with SingleTickerProviderStateMixin {
  Profiles profile = Profiles();
  int tabIndex = 0;
  List<Widget> listScreens;
  List<Widget> _listScreens() => [
        CountTab(),
        SheetTab(),
      ];
  @override
  void initState() {
    super.initState();

    CountScreen.currentCountCode = "";
    CountScreen.currentPlanCount = "";
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    listScreens = _listScreens();
    return Scaffold(
      body: listScreens[tabIndex],
      bottomNavigationBar: BottomNavigationBar(
          selectedItemColor: colorBlue,
          unselectedItemColor: Colors.grey[400],
          backgroundColor: Colors.white,
          currentIndex: tabIndex,
          onTap: (int index) {
            setState(() {
              tabIndex = index;
            });
          },
          items: [
            BottomNavigationBarItem(
              icon: Icon(CupertinoIcons.timer),
              label: 'Process Count',
            ),
            BottomNavigationBarItem(
              icon: Icon(CupertinoIcons.square_favorites),
              label: 'Count Sheet',
            ),
          ]),
      backgroundColor: Theme.of(context).primaryColor,
    );
  }
}
