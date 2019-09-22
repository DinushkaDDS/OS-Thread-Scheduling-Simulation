using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSProject
{

    public partial class Form1 : Form
    {
        static LinkedList<LinkedList<ProcessX>> pqs = new LinkedList<LinkedList<ProcessX>>();
        int maxProcessSize = 0;
        Color[] colors = { Color.Pink, Color.Orange, Color.Yellow,Color.LightGreen, Color.LightBlue, Color.Purple };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            proc.Rows.Add("A", 0, 3);
            proc.Rows.Add("B", 2, 5);
            proc.Rows.Add("C", 4, 2);
            proc.Rows.Add("D", 6, 5);
            proc.Rows.Add("E", 8, 2);
            proc.Rows.Add("F", 9, 2);
        }

        public void queueUpdater()
        {

            d1.Rows.Clear();
            d1.Refresh();
            for (int j = 0; j < maxProcessSize; j++)
            {
                d1.Rows.Add("Q" + j.ToString());
            }

            for (int i = 0; i < pqs.Count; i++)
            {
                //Console.Write(i.ToString() + " ");
                LinkedList<ProcessX> p = pqs.ElementAt(i);
                for (int j = 0; j < p.Count; j++)
                {

                    d1[1 + j, i].Value = p.ElementAt(j).id;
                    d1[1 + j, i].Style.BackColor = colors[p.ElementAt(j).k];
                    d1[1 + j, i].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    //Console.Write(p.ElementAt(j).id + " ");

                }
                //Console.WriteLine("");
            }
            d1[0, 0].Selected = false;
            timeline[0, 0].Selected = false;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            // MessageBox.Show(clicked.ToString());
            proc[0, 0].Selected = false;

            //creating cols and rows in d1
            for (int i = 0; i < proc.Rows.Count + 1; i++)
            {

                d1.Columns.Add(new DataGridViewTextBoxColumn());
                for (int j = 0; j < maxProcessSize; j++)
                {
                    d1.Rows.Add("Q" + j.ToString());
                }

            }

            int time = 0;
            string processLine = "";

            ProcessX[] processes = new ProcessX[proc.Rows.Count];


            for (int i = 0; i < proc.Rows.Count; i++)
            {
                String proname = (String)proc[0, i].Value.ToString();
                int starttime = int.Parse(proc[1, i].Value.ToString());
                int runtime = int.Parse(proc[2, i].Value.ToString());

                processes[i] = new ProcessX(proname, starttime, runtime, i);
                timeline.Rows.Add(processes[i].id);
                proc[0, i].Style.BackColor = colors[processes[i].k];

                if (processes[i].serviceTime > maxProcessSize)
                {
                    maxProcessSize = processes[i].serviceTime;
                }
            }

            int numOfQueues = 0;
            foreach (ProcessX p in processes)
            {
                if (p.serviceTime > numOfQueues)
                {
                    numOfQueues = p.serviceTime;
                }
            }

            for (int i = 0; i < numOfQueues; i++)
            {
                pqs.AddLast(new LinkedList<ProcessX>());
            }


            bool allDone = true;

            while (allDone)
            {

                //**************printQueues(pqs);*******************uncomment to see process queues while updating

                allDone = false;
                foreach (ProcessX p in processes)
                {
                    if (p.remainingTime > 0)
                    {
                        allDone = true;
                        break;
                    }
                }

                foreach (ProcessX p in processes)
                {
                    if (p.arrivalTime == time) //pqs.ElementAt(0).ElementAt(0).serviceTime>0
                    {
                        pqs.ElementAt(0).AddLast(p);
                    }
                }
                //Console.WriteLine(processLine);
                //continue msg box
                // queueUpdater();
                time++;

                bool processRan = false;

                foreach (LinkedList<ProcessX> pl in pqs)
                {

                    queueUpdater();

                    if (pl.Count > 0)
                    {
                        MessageBox.Show(this,"Continue to next cycle?","Continue?",MessageBoxButtons.OK,MessageBoxIcon.Question);
                        processRan = true;
                        ProcessX nextProcessToRun = pl.ElementAt(0);
                        processLine += nextProcessToRun.id + " ";

                        int col = time;

                        int row = nextProcessToRun.k;

                        // label1.Text += (nextProcessToRun.id + " ");

                        timeline[col, row].Style.BackColor = colors[row];

                        nextProcessToRun.remainingTime--;
                        if (nextProcessToRun.remainingTime == 0)
                        {
                            LinkedList<ProcessX> a = pqs.ElementAt(nextProcessToRun.queueNumber);
                            a.Remove(a.ElementAt(0));
                        }
                        else if (processes[1].arrivalTime <= time)
                        {

                            pullDown(nextProcessToRun);

                        }

                        break;
                    }

                }
                if (!processRan)
                {
                    processLine += "_ ";
                }
            }
        }

        void pullDown(ProcessX p)
        {
            ProcessX temp = pqs.ElementAt(p.queueNumber).ElementAt(0);
            pqs.ElementAt(p.queueNumber).Remove(temp);

            pqs.ElementAt(p.queueNumber + 1).AddLast(temp);
            p.queueNumber += 1;
        }
    }

    class ProcessX
    {
        public String id;
        public int k;
        public int arrivalTime;
        public int serviceTime;
        public int remainingTime;
        public int queueNumber;

        public ProcessX(String id, int arrivalTime, int serviceTime, int k)
        {
            this.k = k;
            this.id = id;
            this.arrivalTime = arrivalTime;
            this.serviceTime = serviceTime;
            this.remainingTime = serviceTime;
            this.queueNumber = 0;
        }
    }
}
