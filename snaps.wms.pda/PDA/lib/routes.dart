import 'package:flutter/widgets.dart';
import 'package:wms/screens/Distribution/basecose_screen.dart';
import 'package:wms/screens/Distribution/distribute_screen.dart';
import 'package:wms/screens/home/home_screen.dart';
import 'package:wms/screens/login/login_screen.dart';
import 'package:wms/screens/loosepick/loosepick_screen.dart';
import 'package:wms/screens/receive/detail_screen.dart';
import 'package:wms/screens/receive/receive_screen.dart';
import 'package:wms/screens/stockcount/count_screen.dart';
import 'package:wms/screens/tasks/fullpallet_screen.dart';
import 'package:wms/screens/tasks/putaway_screen.dart';
import 'package:wms/screens/tasks/replenish_screen.dart';
import 'package:wms/screens/tasks/transfer_screen.dart';

final Map<String, WidgetBuilder> routes = {
  LoginScreen.routeName: (context) => LoginScreen(),
  HomeScreen.routeName: (context) => HomeScreen(),
  ReceiveScreen.routeName: (context) => ReceiveScreen(),
  ReceiveDetailScreen.routeName: (context) => ReceiveDetailScreen(),
  PutawayScreen.routeName: (context) => PutawayScreen(),
  ReplenScreen.routeName: (context) => ReplenScreen(),
  ApproachScreen.routeName: (context) => ApproachScreen(),
  LoosePickScreen.routeName: (context) => LoosePickScreen(),
  DistributeScreen.routeName: (context) => DistributeScreen(),
  BaseClosedPage.routeName: (context) => BaseClosedPage(),
  TranferStockScreen.routeName: (context) => TranferStockScreen(),
  CountScreen.routeName: (context) => CountScreen()
};
