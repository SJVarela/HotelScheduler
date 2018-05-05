using HotelSchedulerControl.Scheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestForm
{
    public partial class Form1 : Form
    {
        private TaskScheduler projectManager = new TaskScheduler();
        public Form1()
        {
            InitializeComponent();
            schedulerControl1.Init(projectManager);
            projectManager.Start = DateTime.Now.AddDays(-3);
            var task = new SchedulerEvent(1) { Name = "Hello" , Slack = TimeSpan.FromDays(0.5)};
            projectManager.Add(task);
            projectManager.SetStart(task, DateTime.Now.AddDays(3));
            projectManager.SetEnd(task, DateTime.Now.AddDays(5));

            var task2 = new SchedulerEvent(0) { Name = "Hello" };
            projectManager.Add(task2);
            projectManager.SetStart(task2, DateTime.Now.AddDays(3));
            projectManager.SetEnd(task2, DateTime.Now.AddDays(5));

            var task3 = new SchedulerEvent(0) { Name = "Hello" };
            projectManager.Add(task3);
            projectManager.SetStart(task3, DateTime.Now.AddDays(6));
            projectManager.SetEnd(task3, DateTime.Now.AddDays(7));
        }
    }
}
