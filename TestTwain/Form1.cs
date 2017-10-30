using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwainLib;

namespace TestTwain
{
    public class Form1 : Form, IMessageFilter
    {
        private bool msgfilter;
        private Twain tw;
        private int picnumber = 0;

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox PicForm;


      
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.PicForm = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PicForm)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(88, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 33);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(89, 72);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 55);
            this.button2.TabIndex = 1;
            this.button2.Text = "Select";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // PicForm
            // 
            this.PicForm.Location = new System.Drawing.Point(337, 33);
            this.PicForm.Name = "PicForm";
            this.PicForm.Size = new System.Drawing.Size(337, 265);
            this.PicForm.TabIndex = 2;
            this.PicForm.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 434);
            this.Controls.Add(this.PicForm);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.PicForm)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public Form1()
        {
            InitializeComponent();
            tw = new Twain();
            tw.Init(this.Handle);
        }




        [STAThread]
        static void Main()
        {
            if (Twain.ScreenBitDepth < 15)
            {
                MessageBox.Show("Need high/true-color video mode!", "Screen Bit Depth", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form1 mf = new Form1();
            Application.Run(mf);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (tw != null)
                {
                    tw.Dispose();
                }
                tw = new Twain();
                tw.Init(this.Handle);
                if (!msgfilter)
                {
                    this.Enabled = false;
                    msgfilter = true;
                    Application.AddMessageFilter(this);
                }
                tw.Acquire();
                this.Enabled = true;
            }
            catch (Exception ex)
            {
                this.Enabled = true;
                MessageBox.Show(ex.Message);
            }
        }

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            TwainCommand cmd = tw.PassMessage(ref m);
            if (cmd == TwainCommand.Not || (int)cmd == 0)
            {
                return false;
            }
            switch (cmd)
            {
                case TwainCommand.CloseRequest:
                    {
                        EndingScan();
                        tw.CloseSrc();
                        break;
                    }
                case TwainCommand.CloseOk:
                    {
                        EndingScan();
                        tw.CloseSrc();
                        break;
                    }
                case TwainCommand.DeviceEvent:
                    {
                        break;
                    }
                case TwainCommand.TransferReady:
                    {
                        ArrayList pics = tw.TransferPictures();
                        EndingScan();
                        tw.CloseSrc();
                        picnumber++;
                        for (int i = 0; i < pics.Count; i++)
                        {
                            //IntPtr img = (IntPtr)pics[i];
                            //PicForm newpic = new PicForm(img);
                            //newpic.MdiParent = this;
                            int picnum = i + 1;
                            //newpic.Text = "ScanPass" + picnumber.ToString() + "_Pic" + picnum.ToString();
                            //newpic.Show();
                        }
                        break;
                    }
            }

            return true;
        }

        private void EndingScan()
        {
            if (msgfilter)
            {
                Application.RemoveMessageFilter(this);
                msgfilter = false;
                this.Enabled = true;
                this.Activate();
            }
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(tw!=null)
                tw.Finish();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                tw.Select();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
