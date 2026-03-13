using Misstab.Common.Connection.REST;
using Misstab.Common.TimeLine;
using Misstab.ScreenForms.DialogForm.Setting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misstab.ScreenForms.DialogForm.Viewer
{
    public partial class StasticTimeLine : Form
    {
        public static StasticTimeLine Instance { get; } = new StasticTimeLine();

        private Dictionary<string, DataGridTimeLine> _Grids = new Dictionary<string, DataGridTimeLine>();
        private DataGridTimeLine? _SelectedGrid = null;

        private Dictionary<StasticDisplay.INDEXS, StasticDisplay> _StasticsDisp = new Dictionary<StasticDisplay.INDEXS, StasticDisplay>();

        public StasticTimeLine()
        {
            InitializeComponent();
            this._StasticsDisp = GenStasticsDisp();
            UpdateStasticDisp();
        }

        private static Dictionary<StasticDisplay.INDEXS,  StasticDisplay> GenStasticsDisp()
        {
            Dictionary<StasticDisplay.INDEXS, StasticDisplay> _d = new Dictionary<StasticDisplay.INDEXS, StasticDisplay>
            {
                {
                    StasticDisplay.INDEXS.TabName,
                    StasticDisplay.Gen("タブ名称", string.Empty)
                },
                {
                    StasticDisplay.INDEXS.TabDefinition,
                    StasticDisplay.Gen("タブ識別子", string.Empty)
                },
                {
                    StasticDisplay.INDEXS.NoteCount,
                    StasticDisplay.Gen("ノート数", string.Empty)
                },
                {
                    StasticDisplay.INDEXS.AllNoteCount,
                    StasticDisplay.Gen("保持ノート数", string.Empty)
                },
                {
                    StasticDisplay.INDEXS.LatestPostUpdate,
                    StasticDisplay.Gen("最終投稿日時", string.Empty)
                },
                {
                    StasticDisplay.INDEXS.LatestUpdate,
                    StasticDisplay.Gen("最終更新日時", string.Empty)
                }
            };
            int m = _d.Values.Select(r => { return r.CtlTitle.Size.Width; }).Max();
            _d.Values.ToList().ForEach(r => { r.CtlDispText.Location = new Point(m + 20, r.CtlDispText.Location.Y); });
            return _d;
        }
        private void UpdateStasticDisp()
        {
            if (InvokeRequired)
            {
                this.Invoke(UpdateStasticDisp);
                return;
            }
            int t = 0;
            foreach (StasticDisplay d in this._StasticsDisp.Values)
            {
                if (!this.panel1.Contains(d.CtlTitle))
                {
                    this.panel1.Controls.Add(d.CtlTitle);
                }
                if (!this.panel1.Contains(d.CtlDispText))
                {
                    this.panel1.Controls.Add(d.CtlDispText);
                }
                try
                {
                    d.CtlTitle.Text = d.Title;
                    d.CtlTitle.Location = new Point(0, t);
                    d.CtlDispText.Text = d.DispText;
                    d.CtlDispText.Location = new Point(d.CtlDispText.Location.X, t);

                    t += d.CtlTitle.Height;
                }
                catch (Exception ex)
                {
                }
            }
            if (this._SelectedGrid != null)
            {
                try
                {
                    lock(this._StasticsDisp)
                    {
                        this._StasticsDisp[StasticDisplay.INDEXS.TabName].CtlDispText.Text = this._SelectedGrid.Stastics.TabName;
                        this._StasticsDisp[StasticDisplay.INDEXS.TabDefinition].CtlDispText.Text = this._SelectedGrid.Stastics.TabDefinition;
                        this._StasticsDisp[StasticDisplay.INDEXS.NoteCount].CtlDispText.Text = this._SelectedGrid.Stastics.NoteCount.ToString();
                        this._StasticsDisp[StasticDisplay.INDEXS.AllNoteCount].CtlDispText.Text = this._SelectedGrid.Stastics.AllNoteCount.ToString();
                        this._StasticsDisp[StasticDisplay.INDEXS.LatestUpdate].CtlDispText.Text = this._SelectedGrid.Stastics.LatestUpdate.ToString();
                        this._StasticsDisp[StasticDisplay.INDEXS.LatestPostUpdate].CtlDispText.Text = this._SelectedGrid.Stastics.LatestPostUpdate.ToString();
                    }
                }
                catch
                {
                }
            }
        }

        private void cmdSelectTimeLine_Click(object sender, EventArgs e)
        {
        }

        private void StasticTimeLine_VisibleChanged(object sender, EventArgs e)
        {
            this.cmbTimeLine.Items.Clear();

            if (this.Visible == false)
            {
                return;
            }

            foreach (KeyValuePair<string, DataGridTimeLine> Grid in this._Grids)
            {
                this.cmbTimeLine.Items.Add(new TimeLineCombo(Grid.Value._TabName, Grid.Key));
            }
        }

        private async Task UpdateStastics()
        {
            if (this._SelectedGrid == null)
            {
                return;
            }
            if (InvokeRequired)
            {
                try
                {
                    await this.Invoke(UpdateStastics);
                }
                catch
                {
                    // 例外時は何もしない
                }
                return;
            }
            this.SuspendLayout();
            UpdateStasticDisp();
            this.ResumeLayout(false);
        }

        public void SetDataGrids(Dictionary<string, DataGridTimeLine> Grids)
        {
            this._Grids = Grids;
        }

        private void cmbTimeLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimeLineCombo? Cmb = (TimeLineCombo?)this.cmbTimeLine.SelectedItem;
            if (Cmb == null)
            {
                return;
            }

            DataGridTimeLine Grid = this._Grids[Cmb.TabDefinition];
            this._SelectedGrid = Grid;
            UpdateStasticDisp();
        }

        private void StasticTimeLine_Load(object sender, EventArgs e)
        {
            Task tk = new Task(async () =>
            {
                while (true)
                {
                    await UpdateStastics();
                    await Task.Delay(1000);
                }
            });
            tk.Start();
        }
    }

    public class StasticDisplay
    {
        public enum INDEXS
        {
            None = 0,
            TabName = 1,
            TabDefinition = 2,
            LatestUpdate = 3,

            NoteCount = 10,
            AllNoteCount = 11,

            LatestPostUpdate = 20,
        }

        public static StasticDisplay Gen(string Title, string DispText)
        {
            var o = new StasticDisplay()
            {
                Title = Title,
                DispText = DispText
            };
            o.CtlTitle.Text = Title;
            o.CtlDispText.Text = DispText;
            o.CtlDispText.AutoSize = true;
            return o;
        }

        public string _i { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string DispText { get; set; } = string.Empty;

        public Label CtlTitle { get; } = new Label();
        public Label CtlDispText { get; } = new Label();
    }
}
