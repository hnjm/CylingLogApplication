﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyclingLogApplication
{
    public partial class LegacyImport : Form
    {
        static int legacyImportIndex = -1;


        public LegacyImport()
        {
            InitializeComponent();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            setLegacyIndexSelection(-1);
            //Close();
            this.Invoke(new MethodInvoker(delegate { this.Close(); }), null);
        }

        public int getLegacyIndexSelection()
        {
            return legacyImportIndex;
        }

        public void setLegacyIndexSelection(int index)
        {
            legacyImportIndex = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setLegacyIndexSelection(cbLegacyIndexSelection.SelectedIndex);
            this.Invoke(new MethodInvoker(delegate { this.Close(); }), null);
        }
    }
}
