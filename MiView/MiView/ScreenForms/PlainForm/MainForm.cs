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
        public MainForm()
        {
            InitializeComponent();

            this.label1.Font = new FontLoader().LoadFontFromFile(FontLoader.FONT_SELECTOR.MATERIALICONS, this.label1.Font.Size);
            this.label1.Text = MaterialIcons.Keyboard;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
#if DEBUG
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct,
                ISLOCAL = true,
                DETAIL = "����̓f�o�b�O���s���ɕ\������܂��B",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct,
                ISLOCAL = true,
                RENOTED = true,
                DETAIL = "���m�[�g�\���B",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
                ISLOCAL = true,
                RENOTED = false,
                REPLAYED = true,
                DETAIL = "���v���C�\���B",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
                ISLOCAL = true,
                RENOTED = false,
                REPLAYED = true,
                CW = true,
                DETAIL = "CW",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
                ISLOCAL = true,
                RENOTED = true,
                REPLAYED = true,
                CW = true,
                DETAIL = "��������",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
                ISLOCAL = true,
                RENOTED = false,
                DETAIL = "�p�u���b�N",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.SemiPublic,
                ISLOCAL = true,
                RENOTED = false,
                DETAIL = "�Z�~�p�u���b�N",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Home,
                ISLOCAL = true,
                RENOTED = false,
                DETAIL = "�z�[��",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Follower,
                ISLOCAL = true,
                RENOTED = false,
                DETAIL = "�t�H�����[",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct,
                ISLOCAL = true,
                RENOTED = false,
                DETAIL = "�_�C���N�g���b�Z�[�W",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct,
                ISLOCAL = false,
                RENOTED = false,
                DETAIL = "abcdefghijklmnopqrstuvwxyz",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct,
                ISLOCAL = false,
                RENOTED = false,
                DETAIL = "abcdefghijklmnopqrstuvwxyz".ToUpper(),
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "�A�v��",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = "1960/01/01 00:00:00:000"
            });
#endif

            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { PROTECTED = TimeLineContainer.PROTECTED_STATUS.SemiPublic });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { PROTECTED = TimeLineContainer.PROTECTED_STATUS.Home });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { PROTECTED = TimeLineContainer.PROTECTED_STATUS.Follower });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { RENOTED = true, DETAIL = "���m�[�g" });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { REPLAYED = true, DETAIL = "���v���C" });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { ISLOCAL = true, DETAIL = "�A��" });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { USERNAME = "�ق��ق�" });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { USERID = "ANKIMO" });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { DETAIL = "DataGridView�̍s�ǉ��C�x���g������܂�ɂ������񂱂Ȃ�" });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { SOFTWARE = "misskey 2024.1.0" });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { SOURCE = "misskey.niri.la" });
            this.dataGridTimeLine1.InsertTimeLineData(new TimeLineContainer() { UPDATEDAT = "2000/01/01 01:01:01" });
        }
    }
}
