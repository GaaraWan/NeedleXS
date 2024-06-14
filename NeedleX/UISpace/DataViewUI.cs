using Common.RecipeSpace;
using NeedleX.ProcessSpace;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace NeedleX.UISpace
{
    public partial class DataViewUI : UserControl
    {
        public event EventHandler<ProcessEventArgs> OnRowSelectionChanged;

        private string[] strColTitle = new string[]
        {
            "序号",
            "原始位置x(mm)",
            "原始位置y(mm)",
            "原始位置z(mm)",
            "调整后位置x(mm)",
            "调整后位置y(mm)",
            "调整后位置z(mm)",
            "耗时(s)",
        };

        public DataViewUI()
        {
            InitializeComponent();
            this.InitialDGV();
        }

        private void InitialDGV()
        {
            this.DGV.MultiSelect = false;
            this.DGV.BorderStyle = BorderStyle.None;
            this.DGV.AllowUserToAddRows = false;
            this.DGV.ReadOnly = true;
            for (int i = 0; i < strColTitle.Length; i++)
            {
                //if (i == 1)
                //{
                //    DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
                //    checkBoxColumn.Name = "mySelect";
                //    DGV.Columns.Add(checkBoxColumn);
                //}
                //else
                {
                    DataGridViewTextBoxColumn dataGridViewColumn = new DataGridViewTextBoxColumn();
                    this.DGV.Columns.Add(dataGridViewColumn);
                }

                //DataGridViewTextBoxColumn dataGridViewColumn = new DataGridViewTextBoxColumn();
                //this.DGV.Columns.Add(dataGridViewColumn);
                this.DGV.Columns[i].HeaderText = this.strColTitle[i];
            }



            //this.DGV.AllowUserToAddRows = false;
            this.DGV.RowHeadersVisible = false;
            //this.DGV.ReadOnly = true;
            this.DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //this.DGV.Columns[2].ReadOnly = true;
            //this.DGV.Columns[0].Width = 20;
            //this.DGV.Columns[1].Width = 50;
            //this.DGV.Columns[2].Width = 400;
            ////this.DGV.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //this.DGV.Columns[3].Width = 80;

            ////InitColumnInfo();
            ////#region  自定义组件开始
            ////自定义组件实现
            //var ch = new DatagridviewCheckboxHeaderCell();
            //ch.OnCheckBoxClicked += new DatagridviewcheckboxHeaderEventHander(ch_OnCheckBoxClicked);
            //var checkboxCol = this.DGV.Columns[1] as DataGridViewCheckBoxColumn;
            //checkboxCol.HeaderCell = ch;
            //checkboxCol.HeaderCell.Value = string.Empty;

            //DGV.SelectionChanged += DGV_SelectionChanged;
        }

        private void DGV_SelectionChanged(object sender, EventArgs e)
        {

            //if (DGV.SelectedCells.Count > 0)
            //{
            //    DataGridViewCell cell = DGV.SelectedCells[0];
            //    int i = cell.RowIndex;
            //    XRowEventArgs xRow = new XRowEventArgs();
            //    xRow.Index = int.Parse(DGV.Rows[i].Cells[0].Value.ToString());
            //    xRow.IsSelect = DGV.Rows[i].Cells[1].Value.ToString() == "1";
            //    xRow.FileInfo = new FileInfo(DGV.Rows[i].Cells[2].Value.ToString());
            //    xRow.Transparency = int.Parse(DGV.Rows[i].Cells[3].Value.ToString());

            //    FireUpdate(xRow);
            //}
        }
        //去除datagridview列表头排序
        //    foreach (DataGridViewColumn item in dgv.Columns)
        //        item.SortMode = DataGridViewColumnSortMode.NotSortable;
        /// <summary>
        /// 单击事件
        /// </summary>
        //private void ch_OnCheckBoxClicked(object sender, DatagridviewCheckboxHeaderEventArgs e)
        //{
        //    //失去焦点操作
        //    DGV.EndEdit();

        //    //Console.WriteLine(e.CheckedState.ToString());

        //    //选中事件操作
        //    if (e.CheckedState)
        //    {
        //        for (int i = 0; i < DGV.Rows.Count; i++)
        //        {
        //            DGV.Rows[i].Cells[1].Value = 1;
        //        }
        //        //foreach (DataGridViewRow dgvRow in this.dgv.Rows)
        //        //{
        //        //    dgvRow.Cells["dataGridViewCheckBoxColumn1"].Value = true;
        //        //}
        //    }
        //    else
        //    {
        //        for (int i = 0; i < DGV.Rows.Count; i++)
        //        {
        //            DGV.Rows[i].Cells[1].Value = 0;
        //        }
        //        //foreach (DataGridViewRow dgvRow in this.dgv.Rows)
        //        //{
        //        //    dgvRow.Cells["dataGridViewCheckBoxColumn1"].Value = 0;
        //        //}
        //    }

        //}

        public void Invoke_AddRow(XRowEventArgs e)
        {
            try
            {
                if (base.IsHandleCreated)
                {
                    base.BeginInvoke(new XRowEventHandler(this.Fuc_AddRow), new object[]
                    {
                        e
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void Invoke_DelRow(int i = -1)
        {
            try
            {
                if (base.IsHandleCreated)
                {
                    base.BeginInvoke(new XRowIndexEventHandler(this.Fuc_DelRow), new object[]
                    {
                        i
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void Invoke_FocusLastRow(int i)
        {
            try
            {
                if (base.IsHandleCreated)
                {
                    base.BeginInvoke(new XRowIndexEventHandler(this.Fuc_FocusLastRow), new object[]
                    {
                        i
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void Invoke_ClrTable()
        {
            try
            {
                if (base.IsHandleCreated)
                {
                    base.BeginInvoke(new XClrTableEventHandler(this.Fuc_ClrTable), new object[0]);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public XRowEventArgs GetCurrentData()
        {
            if (DGV.Rows.Count <= 0)
                return null;
            int rowindex = DGV.CurrentCell.RowIndex;
            if (rowindex == -1)
                return null;
            XRowEventArgs xRowEvent = new XRowEventArgs();
            xRowEvent.Index = int.Parse(DGV.Rows[rowindex].Cells[0].Value.ToString());
            xRowEvent.Org = new NeedleXYZ(DGV.Rows[rowindex].Cells[1].Value.ToString() + "," +
                                                                  DGV.Rows[rowindex].Cells[2].Value.ToString() + "," +
                                                                  DGV.Rows[rowindex].Cells[3].Value.ToString());

            xRowEvent.Adjust = new NeedleXYZ(DGV.Rows[rowindex].Cells[4].Value.ToString() + "," +
                                                                 DGV.Rows[rowindex].Cells[5].Value.ToString() + "," +
                                                                 DGV.Rows[rowindex].Cells[6].Value.ToString());

            xRowEvent.ElapsedTime = double.Parse(DGV.Rows[rowindex].Cells[7].Value.ToString());

            return xRowEvent;
        }
        public void Invoke_SaveData(string epath)
        {
            if (string.IsNullOrEmpty(epath)) return;

            //List<XRowEventArgs> xRows = new List<XRowEventArgs>();
            string Str = string.Empty;

            for (int i = 0; i < strColTitle.Length; i++)
            {
                Str += this.strColTitle[i] + ",";
            }
            Str += Environment.NewLine;

            if (DGV.Rows.Count > 0)
            {
                for (int i = 0; i < DGV.RowCount; i++)
                {
                    XRowEventArgs xRowEvent = new XRowEventArgs();
                    xRowEvent.Index = int.Parse(DGV.Rows[i].Cells[0].Value.ToString());
                    xRowEvent.Org = new NeedleXYZ(DGV.Rows[i].Cells[1].Value.ToString() + "," +
                                                                          DGV.Rows[i].Cells[2].Value.ToString() + "," +
                                                                          DGV.Rows[i].Cells[3].Value.ToString());

                    xRowEvent.Adjust = new NeedleXYZ(DGV.Rows[i].Cells[4].Value.ToString() + "," +
                                                                         DGV.Rows[i].Cells[5].Value.ToString() + "," +
                                                                         DGV.Rows[i].Cells[6].Value.ToString());

                    xRowEvent.ElapsedTime = double.Parse(DGV.Rows[i].Cells[7].Value.ToString());
                    //xRows.Add(xRowEvent);

                    Str += xRowEvent.ToDataString() + Environment.NewLine;
                }
            }
            SaveData(Str, epath);

        }

        private void Fuc_AddRow(XRowEventArgs e)
        {
            try
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                for (int i = 0; i < strColTitle.Length; i++)
                {
                    //if (i == 1)
                    //{
                    //    DataGridViewCheckBoxCell dataGridViewCell = new DataGridViewCheckBoxCell();
                    //    dataGridViewRow.Cells.Add(dataGridViewCell);
                    //}
                    //else
                    {
                        DataGridViewTextBoxCell dataGridViewCell = new DataGridViewTextBoxCell();
                        dataGridViewRow.Cells.Add(dataGridViewCell);
                    }
                    //DataGridViewTextBoxCell dataGridViewCell = new DataGridViewTextBoxCell();
                    //dataGridViewRow.Cells.Add(dataGridViewCell);
                }

                dataGridViewRow.Cells[0].Value = this.DGV.Rows.Count + 1;
                dataGridViewRow.Cells[1].Value = e.Org.XStr;
                dataGridViewRow.Cells[2].Value = e.Org.YStr;
                dataGridViewRow.Cells[3].Value = e.Org.ZStr;

                dataGridViewRow.Cells[4].Value = e.Adjust.XStr;
                dataGridViewRow.Cells[5].Value = e.Adjust.YStr;
                dataGridViewRow.Cells[6].Value = e.Adjust.ZStr;

                dataGridViewRow.Cells[7].Value = e.ElapsedTime.ToString("0.00");

                //dataGridViewRow.DefaultCellStyle.BackColor = Color.White;

                this.DGV.Rows.Add(dataGridViewRow);
            }
            catch (Exception ex)
            {

            }
        }
        private void Fuc_DelRow(int i = -1)
        {
            try
            {
                foreach (DataGridViewCell cell in DGV.SelectedCells)
                {
                    i = cell.RowIndex;
                    // 处理选中行的数据
                    break;
                }
                if (i > -1)
                    this.DGV.Rows.RemoveAt(i);
            }
            catch (Exception ex)
            {

            }
        }
        private void Fuc_FocusLastRow(int i)
        {
            try
            {
                this.DGV.CurrentCell = this.DGV.Rows[this.DGV.Rows.Count - 1].Cells[0];
            }
            catch (Exception ex)
            {

            }
        }
        private void Fuc_ClrTable()
        {
            try
            {
                this.DGV.Rows.Clear();
            }
            catch (Exception ex)
            {

            }
        }
        private void Fuc_LoopPlay()
        {
            // 假设dataGridView是一个DataGridView控件

            // 选中第一行
            DGV.Rows[0].Selected = true;

            // 或者通过行索引选中指定行
            int rowIndex = 3;
            DGV.Rows[rowIndex].Selected = true;

            // 或者通过行对象选中指定行
            DataGridViewRow row = DGV.Rows[rowIndex];
            row.Selected = true;

        }
        public void SetDiff(Bitmap bmporg, Bitmap bmpcover, int trans)
        {
            int Grade = 0;

            Rectangle rectbmp = SimpleRect(bmporg.Size);

            rectbmp.Width = Math.Min(bmporg.Width, bmpcover.Width);
            rectbmp.Height = Math.Min(bmporg.Height, bmpcover.Height);

            BitmapData bmpData = bmporg.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmpData1 = bmpcover.LockBits(rectbmp, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr Scan0 = bmpData.Scan0;
            IntPtr Scan1 = bmpData1.Scan0;

            try
            {
                unsafe
                {
                    byte* scan0 = (byte*)(void*)Scan0;
                    byte* pucPtr;
                    byte* pucStart;

                    byte* scan1 = (byte*)(void*)Scan1;
                    byte* pucPtr1;
                    byte* pucStart1;

                    int xmin = rectbmp.X;
                    int ymin = rectbmp.Y;
                    int xmax = xmin + rectbmp.Width;
                    int ymax = ymin + rectbmp.Height;

                    int x = xmin;
                    int y = ymin;

                    int iStride = bmpData.Stride;
                    int iStride1 = bmpData1.Stride;

                    y = ymin;

                    pucStart = scan0 + ((x - xmin) << 2) + (iStride * (y - ymin));
                    pucStart1 = scan1 + ((x - xmin) << 2) + (iStride1 * (y - ymin));

                    while (y < ymax)
                    {
                        x = xmin;

                        pucPtr = pucStart;
                        pucPtr1 = pucStart1;

                        while (x < xmax)
                        {
                            int Grade0 = GrayscaleInt(pucPtr[2], pucPtr[1], pucPtr[0]);
                            int Grade1 = GrayscaleInt(pucPtr1[2], pucPtr1[1], pucPtr1[0]);

                            int Trans = (int)(((float)Grade0 * (float)trans / 255f) + ((float)Grade1 * ((float)(255 - trans) / 255f)));

                            Trans = Math.Min(255, Trans);

                            pucPtr[2] = (byte)Trans;
                            pucPtr[1] = (byte)Trans;
                            pucPtr[0] = (byte)Trans;

                            pucPtr += 4;
                            pucPtr1 += 4;

                            x++;
                        }

                        pucStart += iStride;
                        pucStart1 += iStride1;

                        y++;
                    }

                    bmporg.UnlockBits(bmpData);
                    bmpcover.UnlockBits(bmpData1);
                }
            }
            catch (Exception ex)
            {
                bmporg.UnlockBits(bmpData);
                bmpcover.UnlockBits(bmpData1);
                //JetEazy.LoggerClass.Instance.WriteException(ex);
                //if (IsDebug)
                //    MessageBox.Show("Error :" + ex.ToString());
            }
        }
        int GrayscaleInt(byte R, byte G, byte B)
        {
            return (int)((double)R * 0.3 + (double)G * 0.59 + (double)B * 0.11);
        }
        Rectangle SimpleRect(Size sz)
        {
            return new Rectangle(0, 0, sz.Width, sz.Height);
        }
        void SaveData(string DataStr, string FileName)
        {
            StreamWriter Swr = new StreamWriter(FileName, false, Encoding.Default);

            Swr.Write(DataStr);

            Swr.Flush();
            Swr.Close();
        }
        protected void FireUpdate(XRowEventArgs eRow)
        {
            try
            {
                OnRowSelectionChanged?.Invoke(this, new ProcessEventArgs("update.select", eRow));
            }
            catch (Exception ex)
            {

            }
        }
        protected void FireUpdateImage(Bitmap bmp)
        {
            try
            {
                OnRowSelectionChanged?.Invoke(this, new ProcessEventArgs("update.image", bmp));
            }
            catch (Exception ex)
            {

            }
        }
    }

    public delegate void XClrTableEventHandler();
    public delegate void XRowEventHandler(XRowEventArgs e);
    public delegate void XRowIndexEventHandler(int rowIndex);
    public class XRowEventArgs : EventArgs
    {
        private int index = 0;
        private NeedleXYZ needleXYZOrg = new NeedleXYZ();
        private NeedleXYZ needleXYZAdjust = new NeedleXYZ();
        private double elapsedTime = 0;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        public NeedleXYZ Org
        {
            get { return needleXYZOrg; }
            set { needleXYZOrg = value; }
        }
        public NeedleXYZ Adjust
        {
            get { return needleXYZAdjust; }
            set { needleXYZAdjust = value; }
        }
        public double ElapsedTime
        {
            get { return elapsedTime; }
            set { elapsedTime = value; }
        }
        public XRowEventArgs()
        {

        }
        public XRowEventArgs(int index, NeedleXYZ e1, NeedleXYZ e2, double eMs)
        {
            this.index = index;
            needleXYZOrg = new NeedleXYZ(e1.ToString());
            needleXYZAdjust = new NeedleXYZ(e2.ToString());
            elapsedTime = eMs;
        }
        public string ToDataString()
        {
            string Str = "";

            Str += index.ToString() + ",";
            Str += needleXYZOrg.ToString() + ",";
            Str += needleXYZAdjust.ToString() + ",";
            Str += elapsedTime.ToString();

            return Str;
        }
    }
}
