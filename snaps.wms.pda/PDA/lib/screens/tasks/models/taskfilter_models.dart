class TaskFilter {
  String taskno;
  String tflow;
  String tasktype;
  String sourcehuno;
  String sourceloc;
  String article;

  TaskFilter({
    this.taskno,
    this.tflow,
    this.tasktype,
    this.sourcehuno,
    this.sourceloc,
    this.article,
  });

  TaskFilter.fromJson(Map<String, dynamic> json) {
    tflow = json['tflow'];
    tasktype = json['tasktype'];
    sourcehuno = json["sourcehuno"];
    taskno = json["taskno"];
    sourceloc = json["sourceloc"];
    article = json["article"];
  }

  Map<String, dynamic> toJson() {
    final Map<String, dynamic> data = new Map<String, dynamic>();
    data['tflow'] = this.tflow;
    data['tasktype'] = this.tasktype;
    data["sourcehuno"] = this.sourcehuno;
    data["taskno"] = this.taskno;
    data["sourceloc"] = this.sourceloc;
    data["article"] = this.article;
    return data;
  }
}
