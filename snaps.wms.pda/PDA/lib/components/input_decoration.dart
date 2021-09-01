import 'package:flutter/material.dart';
import 'package:wms/constants.dart';

// ignore: must_be_immutable
class Txtheme {
  static double _fontSize = 12.0;
  static InputDecoration decoration(
    IconData icon,
    String prefixText,
    String label,
    String suffix,
  ) {
    return InputDecoration(
      fillColor: Colors.white,
      filled: true,
      prefixIconConstraints: BoxConstraints(minHeight: 24, minWidth: 36),
      prefixIcon: icon == null
          ? null
          : Icon(
              icon,
              size: 13.0,
              color: dangerColor,
            ),
      prefixText: prefixText,
      // prefixStyle: TextStyle(fontSize: _fontSize, color: Colors.red),
      // suffix: Text('Suffix'),
      suffixText: suffix == null ? null : suffix,
      //suffixStyle: TextStyle(color: Colors.blue, fontSize: _fontSize),
      floatingLabelBehavior: FloatingLabelBehavior.never,
      // hintText: 'Enter Preperation No.',
      // hintStyle: TextStyle(fontSize: _fontSize),
      labelText: label,
      labelStyle: TextStyle(fontSize: _fontSize),
      // isCollapsed: false,
      //isDense: true, // <-- required for set width
      // contentPadding: EdgeInsets.symmetric(
      //   vertical: 8,
      //   horizontal: 10.0,
      // ),
      // border: OutlineInputBorder(
      //   borderSide: BorderSide(color: Colors.red),
      //   borderRadius: BorderRadius.circular(6),
      // ),
      // enabledBorder: OutlineInputBorder(
      //     borderSide: BorderSide(color: Colors.grey.shade500)),
      // focusedBorder: InputBorder.none,
      // focusedBorder: OutlineInputBorder(
      //     borderSide: BorderSide(color: Colors.blue)),
    );
  }

  static InputDecoration deco({
    IconData icon,
    String prefix,
    String label,
    String suffix,
    bool enabled = true,
  }) {
    return InputDecoration(
      fillColor: enabled ? Colors.white : Colors.grey[90],
      filled: true,
      prefixIconConstraints: BoxConstraints(minHeight: 24, minWidth: 30),
      prefixIcon: icon == null ? null : Icon(icon, size: _fontSize, color: dangerColor),
      prefixText: prefix == null ? label : prefix,
      suffixText: suffix,
      floatingLabelBehavior: FloatingLabelBehavior.never,
      labelText: label,
      labelStyle: TextStyle(fontSize: _fontSize),
    );
  }

  static InputDecoration deco2({
    IconData icon,
    String prefix,
    String label,
    String suffix,
    bool enabled = true,
  }) {
    return InputDecoration(
      fillColor: enabled ? Colors.white : Colors.grey[90],
      filled: true,
      prefixIconConstraints: BoxConstraints(minHeight: 24, minWidth: 30),
      prefixIcon: icon == null ? null : Icon(icon, size: _fontSize, color: dangerColor),
      prefixText: prefix == null ? label : prefix,
      suffixText: suffix,
      floatingLabelBehavior: FloatingLabelBehavior.never,
      labelText: label,
      labelStyle: TextStyle(fontSize: _fontSize),
    );
  }
}
