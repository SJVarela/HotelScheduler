using HotelSchedulerControl.Chart;
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
        private Scheduler projectManager = new Scheduler();
        public Form1()
        {
            InitializeComponent();
            schedulerControl1.Init(projectManager);
            projectManager.Start = DateTime.Now;

            var task = new TimeBar() { Name = "Hello" };
            projectManager.Add(task);
            projectManager.SetStart(task, TimeSpan.FromDays(5));
            projectManager.SetDuration(task, TimeSpan.FromDays(5));


        }
    }
}
