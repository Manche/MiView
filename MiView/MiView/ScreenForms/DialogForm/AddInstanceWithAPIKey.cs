using MiView.Common.Connection.REST;
using MiView.Common.Connection.VersionInfo;
using MiView.Common.Connection.WebSocket.Structures;
using MiView.ScreenForms.Controls.Combo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiView.ScreenForms.DialogForm
{
    public partial class AddInstanceWithAPIKey : Form
    {
        private MainForm? _MainForm = null;
        private CSoftwareVersionInfo? _VerInfo { get; set; } = new CSoftwareVersionInfo();

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
                                txtInstanceURL.Text == string.Empty);
            }
            else
            {
                InputChecked = (txtInstanceURL.Text == string.Empty);
            }
            if (InputChecked)
            {
                MessageBox.Show("インスタンスURLもしくはAPIキーが入力されていません。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cmdGetVersionInfo_Click(sender, new EventArgs());
            _ = Task.Run(async () =>
            {
                // URLが存在するかチェック
                HttpClient Clt = new HttpClient();
                try
                {
                    var HttpResult = await Clt.GetAsync(string.Format(@"https://{0}/", txtInstanceURL.Text));

                    if (HttpResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("存在しないURLです。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                }

                this._MainForm.BeginInvoke(new Action(() => _MainForm.AddTimeLine(InstanceURL: txtInstanceURL.Text,
                                                                                  TabName: txtTabName.Text,
                                                                                  APIKey: txtAPIKey.Text,
                                                                                  sTLKind: ((CmbInstance)cmbTLKind.SelectedItem)._TLKind,
                                                                                  SoftwareVersionInfo: _VerInfo)));
            });
        }

        private void cmdGetVersionInfo_Click(object sender, EventArgs e)
        {
            GetCommon VersionInfo = new GetCommon();
            _VerInfo = VersionInfo.GetSoftwareVersion(this.txtInstanceURL.Text,
                                                                            this.txtAPIKey.Text,
                                                                            Common.Connection.VersionInfo.CSoftwareVersionInfo.SOFTWARE_LIST.MISSKEY);
            if (_VerInfo == null)
            {
                MessageBox.Show("バージョン情報が取得できませんでした。");
                this.cmdApply.Enabled = false;
                return;
            }
            this.txtSoftwareName.Text = _VerInfo.SoftwareName;
            this.txtSoftwareVersion.Text = $"{_VerInfo.Version.MajorVersion}.{_VerInfo.Version.MinorVersion}.{_VerInfo.Version.Revision}.{_VerInfo.Version.BuildVersion}";
            this.cmdApply.Enabled = true;
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
