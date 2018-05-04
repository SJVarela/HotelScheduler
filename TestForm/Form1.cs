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
            var task = new SchedulerEvent() { Name = "Hello" };
            projectManager.Add(task);
            projectManager.SetStart(task, DateTime.Now.AddDays(3));
            projectManager.SetEnd(task, DateTime.Now.AddDays(5));
        }
    }
}
