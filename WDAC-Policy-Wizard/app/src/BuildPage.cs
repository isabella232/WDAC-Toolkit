﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

namespace WDAC_Wizard
{
    public partial class BuildPage : UserControl
    {
        public string FilePath { get; set; }
        private MainWindow _MainWindow;

        const int PATH_LENGTH_LIMIT = 100; 

        public BuildPage(MainWindow pMainWindow)
        {
            InitializeComponent();
            progressBar.Maximum = 100;
            progressBar.Step = 1;
            progressBar.Value = 0;
            this.FilePath = String.Empty;

            this._MainWindow = pMainWindow; 
        }

        /// <summary>
        /// Sets the value of the Progres bar, the percent label and the process label on the UserControl 
        /// </summary>
        /// <param name="progress">Int between 0-100 representing the percent progress of the CI policy 
        /// build operations</param>
        /// <param name="process">String shown on UI representing the CI policy build process currently executing.</param>
        public void UpdateProgressBar(int progress, string process)
        {
            progressBar.Value = progress;
            progress_Label.Visible = true; 
            progress_Label.Text = String.Format("{0} %", Convert.ToString(progress));
            progressString_Label.Text = process;
        }

        /// <summary>
        /// Launches the finished message panel showing the location of the finished CI policy when the build 
        /// process is successfully finished. /// 
        /// </summary>
        public void ShowFinishMsg(string policyFilePath)
        {
            // Check if the policyFilePath contains a .bin/.cip file too
            // Parse for the xml file only to add to this.FilePath
            if(policyFilePath.Contains(Environment.NewLine))
            {
                var eol = policyFilePath.IndexOf(Environment.NewLine);
                this.FilePath = policyFilePath.Substring(0, eol);

                string xmlString = policyFilePath.Substring(0, eol);
                string binString = policyFilePath.Substring(eol);

                policyFilePath = FormatText(xmlString) + FormatText(binString); 
            }
            else
            {
                policyFilePath = FormatText(policyFilePath);
                this.FilePath = policyFilePath;
            }

            this.hyperlinkLabel.Text = policyFilePath;
            this.hyperlinkLabel.Enabled = true;
            this.finishPanel.Visible = true;
            this.label_WaitMsg.Visible = false; 
        }

        public string FormatText(string longstring)
        {
            if(longstring.Length > PATH_LENGTH_LIMIT)
            {
                // Get the last instance of the \ between the start and the path limit (80)
                int splitLoc = longstring.Substring(0, PATH_LENGTH_LIMIT).LastIndexOf("\\");
                if(splitLoc > 1)
                {
                    longstring = longstring.Substring(0, splitLoc) + Environment.NewLine + longstring.Substring(splitLoc);
                }
                else
                {
                    // Split midway through
                    int mid_pt = longstring.Length / 2; 
                    longstring = longstring.Substring(0, mid_pt) + Environment.NewLine + longstring.Substring(mid_pt);
                }
            }

            return longstring; 
        }

        /// <summary>
        /// Shows an error message if the CI policy build process throws an error 
        /// </summary>
        public void ShowError()
        {
            finishPanel.Visible = true;
            this.finishLabel.Text = "There was an error building your policy. Press home to begin again.";
            UpdateProgressBar(0, "Error");

            this.hyperlinkLabel.Enabled = false;
            this.label_WaitMsg.Visible = false;
        }

        /// <summary>
        /// Opens the file explorer to the CI policy just built located in the folder path defined in the FilePath param . 
        /// </summary>
        private void hyperlinkLabel_Click(object sender, EventArgs e)
        {
            
            // open text file in notepad (or another default text editor)
            if (File.Exists(this.FilePath))
            {
                try
                {
                    System.Diagnostics.Process.Start(this.FilePath);
                }
                catch (Exception excpt)
                {
                    // Log file already closed/uploaded
                    Console.WriteLine(String.Format("{0} exception caught", excpt));
                }
            }
            else
            {
                Console.WriteLine(String.Format("Unable to open {0}", this.FilePath));
            }
        }
    }
}
