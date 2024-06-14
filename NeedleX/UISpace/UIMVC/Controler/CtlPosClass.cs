using Common.RecipeSpace;
using JetEazy.BasicSpace;
using JetEazy.Interface;
using NeedleX.ProcessSpace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VsCommon.ControlSpace.MachineSpace;

namespace NeedleX.UISpace.UIMVC.Controler
{
    public class CtlPosClass
    {
        public event EventHandler<ProcessEventArgs> OnMessage;

        PosUI posUI;

        Button btnADD;
        Button btnDEL;
        Button btnUpdate;
        Button btnGO;
        Button btnRelationGo;
        Button btnToEyes;

        DataGridView DGVIEW;

        NeedleMachineClass MACHINE;
        private string m_pos = string.Empty;
        private string m_ToRelationMicro_pos = "0,0,0";
        private string m_ToRelationEyes_pos = "0,0,0";

        bool IsZToGo
        {
            get { return posUI.chkOpenZGo.Checked; }
        }

        public CtlPosClass(PosUI ePos)
        {
            this.posUI = ePos;
            InitialInternal();
        }
        public void Init(NeedleMachineClass eMACHINE,string ePOS,string eToRelationMicroPos,string eToRelationEyesPos)
        {
            MACHINE = eMACHINE;
            m_pos = ePOS;
            m_ToRelationMicro_pos = eToRelationMicroPos;
            m_ToRelationEyes_pos = eToRelationEyesPos;

            DGVIEW.SelectionChanged += DGVIEW_SelectionChanged;
            DGVIEW.RowPostPaint += DGVIEW_RowPostPaint;

            FillDisplay();
        }
        public string GetPositionList()
        {
            string strresult = string.Empty;
            int i = 0;
            while (i < DGVIEW.Rows.Count)
            {
                strresult += DGVIEW.Rows[i].Cells[0].Value.ToString() + ",";
                strresult += DGVIEW.Rows[i].Cells[1].Value.ToString() + ",";
                strresult += DGVIEW.Rows[i].Cells[2].Value.ToString() + ";";
                i++;
            }

            strresult = RemoveLastChar(strresult, 1);
            return strresult;
        }
        public void VisableControl(bool eVisable)
        {
            btnRelationGo.Visible = eVisable;
            btnToEyes.Visible = eVisable;
        }

        void InitialInternal()
        {
            btnADD = this.posUI.button8;
            btnDEL = this.posUI.button6;
            btnUpdate = this.posUI.button7;
            btnGO = this.posUI.button5;
            DGVIEW = this.posUI.dataGridView1;
            btnRelationGo = this.posUI.button1;
            btnToEyes = this.posUI.button2;

            btnADD.Click += BtnADD_Click;
            btnDEL.Click += BtnDEL_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnGO.Click += BtnGO_Click;
            btnRelationGo.Click += BtnRelationGo_Click;
            btnToEyes.Click += BtnToEyes_Click;

        }

        private void BtnToEyes_Click(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;

            string onStrMsg = "更新 表第 " + rowindex + " 行？";
            string offStrMsg = "相对定位至 表第 " + rowindex + " 行？";
            string msg = (true ? offStrMsg : onStrMsg);

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            string strresult = string.Empty;
            int i = rowindex;
            strresult += DGVIEW.Rows[i].Cells[0].Value.ToString() + ",";
            strresult += DGVIEW.Rows[i].Cells[1].Value.ToString() + ",";
            strresult += DGVIEW.Rows[i].Cells[2].Value.ToString();

            NeedleXYZ basexyz = new NeedleXYZ(strresult);
            NeedleXYZ relationxyz = new NeedleXYZ(m_ToRelationMicro_pos);
            NeedleXYZ resultxyz = new NeedleXYZ(strresult);
            resultxyz.X = basexyz.X - relationxyz.X;
            resultxyz.Y = basexyz.Y - relationxyz.Y;
            resultxyz.Z = basexyz.Z - relationxyz.Z;
            
            NeedleXYZ relationxyz1 = new NeedleXYZ(m_ToRelationEyes_pos);
            NeedleXYZ resultxyz1 = new NeedleXYZ(strresult);
            resultxyz1.X = resultxyz.X - relationxyz1.X;
            resultxyz1.Y = resultxyz.Y - relationxyz1.Y;
            resultxyz1.Z = resultxyz.Z - relationxyz1.Z;

            MACHINE.GoPosition(resultxyz1.ToString(), IsZToGo);
            FireMessage(new ProcessEventArgs("2"));
        }

        private void BtnRelationGo_Click(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;

            string onStrMsg = "更新 表第 " + rowindex + " 行？";
            string offStrMsg = "相对定位至 表第 " + rowindex + " 行？";
            string msg = (true ? offStrMsg : onStrMsg);

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            string strresult = string.Empty;
            int i = rowindex;
            strresult += DGVIEW.Rows[i].Cells[0].Value.ToString() + ",";
            strresult += DGVIEW.Rows[i].Cells[1].Value.ToString() + ",";
            strresult += DGVIEW.Rows[i].Cells[2].Value.ToString();

            NeedleXYZ basexyz = new NeedleXYZ(strresult);
            NeedleXYZ relationxyz = new NeedleXYZ(m_ToRelationMicro_pos);
            NeedleXYZ resultxyz = new NeedleXYZ(strresult);
            resultxyz.X = basexyz.X - relationxyz.X;
            resultxyz.Y = basexyz.Y - relationxyz.Y;
            resultxyz.Z = basexyz.Z - relationxyz.Z;

            MACHINE.GoPosition(resultxyz.ToString(), IsZToGo);
            FireMessage(new ProcessEventArgs("2"));
        }

        private void DGVIEW_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                                                                               e.RowBounds.Location.Y,
                                                                               DGVIEW.RowHeadersWidth - 4,
                                                                               e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                                                       DGVIEW.RowHeadersDefaultCellStyle.Font,
                                                       rectangle,
                                                       DGVIEW.RowHeadersDefaultCellStyle.ForeColor,
                                                       TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void DGVIEW_SelectionChanged(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;


        }

        private void BtnGO_Click(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;

            string onStrMsg = "更新 表第 " + rowindex + " 行？";
            string offStrMsg = "定位至 表第 " + rowindex + " 行？";
            string msg = (true ? offStrMsg : onStrMsg);

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            string strresult = string.Empty;
            int i = rowindex;
            strresult += DGVIEW.Rows[i].Cells[0].Value.ToString() + ",";
            strresult += DGVIEW.Rows[i].Cells[1].Value.ToString() + ",";
            strresult += DGVIEW.Rows[i].Cells[2].Value.ToString();

            MACHINE.GoPosition(strresult, IsZToGo);

            FireMessage(new ProcessEventArgs("1"));
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;

            string onStrMsg = "更新 表第 " + rowindex + " 行？";
            string offStrMsg = "更新 表第 " + rowindex + " 行？";
            string msg = (true ? offStrMsg : onStrMsg);

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            string[] strpos = _getModulePosition().Split(',').ToArray();

            if (strpos.Length >= 3)
            {
                this.DGVIEW.Rows[rowindex].Cells[0].Value = strpos[0];
                this.DGVIEW.Rows[rowindex].Cells[1].Value = strpos[1];
                this.DGVIEW.Rows[rowindex].Cells[2].Value = strpos[2];
            }
        }

        private void BtnDEL_Click(object sender, EventArgs e)
        {
            if (DGVIEW.Rows.Count <= 0)
                return;

            int rowindex = DGVIEW.CurrentCell.RowIndex;
            if (rowindex == -1)
                return;

            string onStrMsg = "删除 表第 " + rowindex + " 行？";
            string offStrMsg = "删除 表第 " + rowindex + " 行？";
            string msg = (true ? offStrMsg : onStrMsg);

            if (VsMSG.Instance.Question(msg) != DialogResult.OK)
            {
                return;
            }

            DGVIEW.Rows.RemoveAt(rowindex);
        }

        private void BtnADD_Click(object sender, EventArgs e)
        {
            //if (this.DGVIEW.Rows.Count < m_pos_count)
            {
                string[] strpos = _getModulePosition().Split(',').ToArray();
                if (strpos.Length >= 3)
                {
                    int index = this.DGVIEW.Rows.Add();
                    this.DGVIEW.Rows[index].Cells[0].Value = strpos[0];
                    this.DGVIEW.Rows[index].Cells[1].Value = strpos[1];
                    this.DGVIEW.Rows[index].Cells[2].Value = strpos[2];
                }
            }
        }

        protected void FireMessage(ProcessEventArgs e)
        {
            OnMessage?.Invoke(this, e);
        }

        string _getModulePosition()
        {
            string str = string.Empty;
            str += GetAxis(0).GetPos().ToString("0.000000") + ",";
            str += GetAxis(1).GetPos().ToString("0.000000") + ",";
            str += GetAxis(2).GetPos().ToString("0.000000");
            return str;
        }
        private IAxis GetAxis(int axisID)
        {
            return MACHINE.PLCMOTIONCollection[axisID];
        }
        string RemoveLastChar(string Str, int Count)
        {
            if (Str.Length < Count)
                return "";

            return Str.Remove(Str.Length - Count, Count);
        }
        void FillDisplay()
        {
            DGVIEW.Rows.Clear();
            List<string> listpos = m_pos.Split(';').ToList();
            if (listpos.Count > 0)
            {
                foreach (string str in listpos)
                {
                    List<string> listpostemp = str.Split(',').ToList();
                    if (listpostemp.Count == 3)
                    {
                        if (string.IsNullOrEmpty(listpostemp[0]) || string.IsNullOrEmpty(listpostemp[1]) || string.IsNullOrEmpty(listpostemp[2]))
                        {

                        }
                        else
                        {
                            //if (this.DGVIEW.Rows.Count < m_pos_count)
                            {
                                int index = this.DGVIEW.Rows.Add();
                                this.DGVIEW.Rows[index].Cells[0].Value = listpostemp[0];
                                this.DGVIEW.Rows[index].Cells[1].Value = listpostemp[1];
                                this.DGVIEW.Rows[index].Cells[2].Value = listpostemp[2];
                            }
                        }
                    }
                }
            }
        }


    }
}
