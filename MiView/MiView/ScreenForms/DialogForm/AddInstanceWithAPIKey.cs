using MiView.Common.Connection.WebSocket.Structures;
using MiView.ScreenForms.Controls.Combo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiView.ScreenForms.DialogForm
{
    public partial class AddInstanceWithAPIKey : Form
    {
        private MainForm? _MainForm = null;

        public AddInstanceWithAPIKey(MainForm MainForm)
        {
            InitializeComponent();
            this._MainForm = MainForm;
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            // 状況的に発生しない
            if (_MainForm == null)
            {
                return;
            }

            // タイムラインの種類
            string SelectTL = string.Empty;
            if (cmbTLKind.SelectedItem == null || cmbTLKind.SelectedItem.ToString() == "")
            {
                MessageBox.Show("TLの種類を選択してください。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CmbInstance SelectedTLKind = ((CmbInstance)cmbTLKind.SelectedItem);
            bool InputChecked = false;

            // 入力が足らない
            if (SelectedTLKind._TLKind == Common.Connection.WebSocket.TimeLineBasic.ConnectTimeLineKind.Home)
            {
                InputChecked = (txtAPIKey.Text == string.Empty ||
                                txtInstanceURL.Text == string.Empty ||
                                txtTabName.Text == string.Empty);
            }
            else
            {
                InputChecked = (txtInstanceURL.Text == string.Empty ||
                                txtTabName.Text == string.Empty);
            }
            if (InputChecked)
            {
                MessageBox.Show("インスタンスURLもしくはAPIキー、タブ名称が入力されていません。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _ = Task.Run(async () =>
            {
                // URLが存在するかチェック
                HttpClient Clt = new HttpClient();
                try
                {
                    var HttpResult = await Clt.GetAsync(string.Format(@"http://{0}/", txtInstanceURL.Text));

                    if (HttpResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("存在しないURLです。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                }

                this._MainForm.AddTimeLine(txtInstanceURL.Text, txtTabName.Text, txtAPIKey.Text, ((CmbInstance)cmbTLKind.SelectedItem)._TLKind);
            });
        }

        private void AddInstanceWithAPIKey_Load(object sender, EventArgs e)
        {
            this.cmbTLKind.Items.Clear();

            // ホームTL
            this.cmbTLKind.Items.Add(new CmbInstance(Common.Connection.WebSocket.TimeLineBasic.ConnectTimeLineKind.Home,
                                     "ホームTL"));

            // ソーシャルTL
            this.cmbTLKind.Items.Add(new CmbInstance(Common.Connection.WebSocket.TimeLineBasic.ConnectTimeLineKind.Social,
                                     "ソーシャル"));

            // ローカルTL
            this.cmbTLKind.Items.Add(new CmbInstance(Common.Connection.WebSocket.TimeLineBasic.ConnectTimeLineKind.Local,
                                     "ローカルTL"));

            // グローバルTL
            this.cmbTLKind.Items.Add(new CmbInstance(Common.Connection.WebSocket.TimeLineBasic.ConnectTimeLineKind.Global,
                                     "グローバル"));
        }
    }
}
