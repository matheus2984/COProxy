using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Logger
{
    public partial class frmLogger : Form
    {
        private static readonly List<ushort> UnWantedPackets = new List<ushort>
        {1033, 1009, 2061, 2054, 10010, 2081, 1101, 1004, 10014, 10005, 10017};

        public frmLogger()
        {
            InitializeComponent();
        }

        private void dataLogger_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public void AddData(byte[] data)
        {
            DataLogger logger = new DataLogger(data);
            if (UnWantedPackets.Contains(logger.Type)) return;

            dataLogger.Invoke(new Action(() =>
            {
                var index = dataLogger.Rows.Add();
                dataLogger.Rows[index].Cells[0].Value = logger.Size;
                dataLogger.Rows[index].Cells[1].Value = logger.Type;
                dataLogger.Rows[index].Cells[2].Value = logger;
                dataLogger.Rows[index].Cells[3].Value = logger.Seal;
            }));
        }
    }
}