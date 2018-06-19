using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ConsoleApplication5
{
    public class DashboardManager
    {
        private int textBoxSize_X = 200;
        private int textBoxSize_Y = 62;
        private int LabelSize_X = 35;
        private int LabelSize_Y = 13;
        private int Spacing_X = 2;
        private int Spacing_Y = 10;
        private int Count_X = 7;
        private string textBoxPrefix = "TextBox";
        private string LabelPrefix = "Label";
        private string[] symbolList;
        private Form Dashboard;
        delegate void StringArgReturningVoidDelegate(int i, string text, bool Line);

        public DashboardManager()
        {
            Spacing_Y += LabelSize_Y + textBoxSize_Y;
            Dashboard = new Form();
        }

        public void createForm(string[] _symbolList)
        {
            symbolList = _symbolList;

            int n = symbolList.Length;

            // Declare and initialize an array of textboxes and labels
            System.Windows.Forms.TextBox[] AllTextBoxes = new System.Windows.Forms.TextBox[n];
            System.Windows.Forms.Label[] AllLabels = new System.Windows.Forms.Label[n];
            System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker();

            for (int i = 0; i < n; i++)
            {
                AllTextBoxes[i] = new System.Windows.Forms.TextBox();
                AllLabels[i] = new System.Windows.Forms.Label();
            }
            Dashboard.SuspendLayout();

            // Configurate all textboxes
            for (int i = 0; i < n; i++)
            {
                System.Windows.Forms.TextBox iTextBox = AllTextBoxes[i];
                iTextBox.Location = new System.Drawing.Point(6 + (i % Count_X) * (textBoxSize_X + Spacing_X),
                    25 + (i / Count_X) * Spacing_Y);
                iTextBox.Multiline = true;
                iTextBox.Name = textBoxPrefix + i;
                iTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
                iTextBox.WordWrap = false;
                iTextBox.Size = new System.Drawing.Size(textBoxSize_X, textBoxSize_Y);
                iTextBox.TabIndex = 0;
                iTextBox.ReadOnly = true;
            }

            // Configurate all labels
            for (int i = 0; i < n; i++)
            {
                System.Windows.Forms.Label iLabel = AllLabels[i];
                iLabel.AutoSize = true;
                iLabel.Location = new System.Drawing.Point(6 + (i % Count_X) * (textBoxSize_X + Spacing_X),
                    9 + (i / Count_X) * Spacing_Y);
                iLabel.Name = LabelPrefix + i;
                iLabel.Size = new System.Drawing.Size(LabelSize_X, LabelSize_Y);
                iLabel.TabIndex = 1;
                iLabel.Text = symbolList[i];
            }


            // Construct Windows Form
            Dashboard.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            Dashboard.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Dashboard.AutoScroll = true;
            Dashboard.ClientSize = new System.Drawing.Size(852, 715);
            Dashboard.Name = "Dashboard";
            Dashboard.Text = "Dashboard";
            Dashboard.AutoScroll = true;
            Dashboard.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Add controls
            for (int i = 0; i < n; i++)
            {
                Dashboard.Controls.Add(AllLabels[i]);
                Dashboard.Controls.Add(AllTextBoxes[i]);
            }

            // Finalize
            Dashboard.ResumeLayout(false);
            Dashboard.PerformLayout();
            Dashboard.ShowDialog();
        }


        public void WriteLine(int i, string text , bool Line)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true.
            System.Windows.Forms.Control theTextBox = this.Dashboard.Controls.Find(textBoxPrefix + i, false)[0];
            string InputText = text + ((Line) ? System.Environment.NewLine : "");

            if (theTextBox.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(WriteLine);
                theTextBox.Invoke(d, new object[] { i, text, Line });
            }
            else
            {
                if (!Line)
                {
                    try
                    {
                        ((TextBox)theTextBox).Text = ((TextBox)theTextBox).Text.Remove(((TextBox)theTextBox).Text.LastIndexOf(Environment.NewLine));
                        ((TextBox)theTextBox).AppendText(Environment.NewLine);
                    }
                    catch (ArgumentOutOfRangeException)
                    {}
                }

                ((TextBox)theTextBox).AppendText(InputText);
                //((TextBox)theTextBox).SelectionStart = theTextBox.Text.Length;
            }
        }

        public void ChangeCellColor(int i,Color targetColor)
        {
            System.Windows.Forms.Control theTextBox = this.Dashboard.Controls.Find(textBoxPrefix + i, false)[0];
            theTextBox.BackColor = targetColor;
        }
    }
}
