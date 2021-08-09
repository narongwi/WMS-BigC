using snaps.wms.notify;
using System;
using Microsoft.Win32.TaskScheduler;
using System.Text;
using System.IO;
using Microsoft.Extensions.Configuration;
namespace snaps.wms.notifyapp {
    class Program {
        static void Main(string[] args) {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("snaps.configuration.json");

            var configuration = builder.Build();
            using(TaskService sched = new TaskService()) {
                StringBuilder messages = new StringBuilder();

                try {
                    // configuration
                    string proxyServer = configuration["notify:proxy"];
                    string notifyToken = configuration["notify:token"];
                    bool issticker = Convert.ToBoolean(configuration["notify:issticker"]);
                    string folder = configuration["task:folder"];
                    bool errorOnly = Convert.ToBoolean(configuration["task:errorOnly"]);


                    int errorTask = 0;
                    using(var taskFolder = sched.GetFolder(folder)){
                        foreach(var task in taskFolder.Tasks) {
                            if(task.State.ToString() == TaskState.Ready.ToString() && task.LastTaskResult != 0) errorTask++;
                            if(errorOnly) {
                                // get message status is error only 
                                if(errorTask > 0) {
                                    messages.AppendLine(task.State + getResult(task.State,task.LastTaskResult) + task.Name);
                                }
                            } else {
                                // all task
                                messages.AppendLine(task.State + getResult(task.State,task.LastTaskResult) + task.Name);
                            }
                        }

                        Console.WriteLine(messages.ToString());
                        if(messages.Length > 0) {
                            Console.WriteLine("Notify Sending...");
                            messages.Insert(0,configuration["notify:title"] + Environment.NewLine);
                            var notify = new LineNotify(notifyToken,proxyServer);
                            notify.Send(messages.ToString(),issticker,errorTask == 0 ? true : false).Wait();
                            Console.WriteLine("Notify Successfully!");
                        }
                    }
                } catch(Exception ex) {
                    Console.WriteLine(ex.Message.ToString());
                } 
            }
        }

        static string getResult(TaskState state,int code) {
            if(state == TaskState.Running) {
                return " Running ";
            }else if(state.ToString() != TaskState.Disabled.ToString() && code == 0) {
                return " Success ";
            } else if(state.ToString() == TaskState.Disabled.ToString() && code == 0) {
                return " Success ";
            } else if(state.ToString() == TaskState.Disabled.ToString() && code != 0) {
                return " Paused ";
            } else {
                return " Error ";
            }
        }
    }
}
