using MiView.Common.Fonts;
using MiView.Common.Fonts.Material;
using MiView.Common.TimeLine;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace MiView
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// �^�C�����C���}�l�[�W��
        /// </summary>
        private TimeLineCreator TimeLineManage = new TimeLineCreator();

        /// <summary>
        /// ���̃t�H�[��
        /// </summary>
        private Form MainFormObj;

        public MainForm()
        {
            InitializeComponent();

            this.label1.Font = new FontLoader().LoadFontFromFile(FontLoader.FONT_SELECTOR.MATERIALICONS, this.label1.Font.Size);
            this.label1.Text = MaterialIcons.Keyboard;

            this.MainFormObj = this;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            TimeLineManage.CreateTimeLine(ref this.MainFormObj, "Main", "tpMain");
            // TimeLineManage.DeleteTimeLine(ref this.MainFormObj, "Main", "tpMain");


            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { PROTECTED = TimeLineContainer.PROTECTED_STATUS.SemiPublic });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { PROTECTED = TimeLineContainer.PROTECTED_STATUS.Home });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { PROTECTED = TimeLineContainer.PROTECTED_STATUS.Follower });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { RENOTED = true, DETAIL = "���m�[�g" });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { REPLAYED = true, DETAIL = "���v���C" });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { ISLOCAL = true, DETAIL = "�A��" });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { USERNAME = "�ق��ق�" });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { USERID = "ANKIMO" });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { DETAIL = "DataGridView�̍s�ǉ��C�x���g������܂�ɂ������񂱂Ȃ�" });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { SOFTWARE = "misskey 2024.1.0" });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { SOURCE = "misskey.niri.la" });
            //this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { UPDATEDAT = "2000/01/01 01:01:01" });
        }
    }
}
