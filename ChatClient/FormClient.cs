using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chat_LongLe;

namespace ChatClient
{
    public partial class FormClient : Form
    {
        IPEndPoint IP;
        Socket client;

        string[] inbox = new string[]{ "","" }; //mảng để gửi qua server
        string currentinbox = "Server";

        List<object> del = new List<object>() { "","" };    // list gửi qua server để xóa tin nhắn
        int deliverport = 1;    // số thể hiện client port (= 1 nếu là server) gửi qua server

        int index = 0; // mã số của 1 icon trong Imageindex
        public FormClient()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            ketnoi();

            client.Send(maHoa(deliverport));    // gửi qua server lấy thông tin chat
        }
        private void FormClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.Close();
        }
        void ketnoi() //kết nối server
        {
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Can't connect to server", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Enabled = false;
                return;
            }
            this.Text = "Client " + ((IPEndPoint)client.LocalEndPoint).Port;

            Thread listen = new Thread(received);
            listen.IsBackground = true;
            listen.Start();
        }
        int id = -1;    // số thể hiện mã số ảnh đính kèm khi hiện hoặc xóa ảnh
        void received()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);

                    if (giaiMa(data).GetType().ToString() == "System.Collections.Generic.List`1[System.String]") //list chứa sanh sách client lấy từ server 
                    {
                        List<string> list = new List<string>();
                        list = (List<string>)giaiMa(data);

                        listBox1.Items.Clear();
                        listBox1.Items.Add("Server");
                        foreach (string i in list)
                        {
                            if (((IPEndPoint)client.LocalEndPoint).Port.ToString() != i) listBox1.Items.Add(i);
                        }

                        if (inbox[0] != "") listBox1.SelectedItem = inbox[0];
                        else listBox1.SelectedIndex = 0;
                    }
                    else if (giaiMa(data).GetType().ToString() == "System.Collections.Generic.List`1[System.Object]") //list chứa thông tin đoạn chat
                    {
                        List<object> list = new List<object>();
                        list = (List<object>)giaiMa(data);

                        listView1.Items.Clear();
                        foreach (object l in list)
                        {
                            if (!l.ToString().Contains(":")) continue;
                            else if (l.ToString().Substring(0, 5) == ((IPEndPoint)client.LocalEndPoint).Port.ToString())
                                addMessage("You   : " + l.ToString().Substring(8));
                            else addMessage(l.ToString());
                        }
                    }
                    else if (giaiMa(data).GetType().ToString() == "System.Drawing.Bitmap") // byte chứa ảnh
                    {
                        id++;
                        Image i = (Image)giaiMa(data);
                        imageList1.Images.Add(id + "", i);
                        ListViewItem item = new ListViewItem();
                        item.ImageKey = id + "";
                        addMessage("Server: ", id);
                    }
                    else if (giaiMa(data).GetType().ToString() == "System.String[]") //mảng chứa 1 tin nhắn nhận từ client khác (gửi qua server)
                    {
                        string[] message = (string[])giaiMa(data);
                        if (currentinbox == message[0]) addMessage(message[1]);
                    }
                    else         // dòng text chứa tin nhắn từ server
                    {
                        string message = (string)giaiMa(data);
                        addMessage(message);
                    }
                }
            }
            catch
            {
                Close();
            }
        }
        void addMessage(string s)   //hiện tin nhắn lên listView1
        {
            listView1.Items.Add(new ListViewItem() { BackColor = Color.LightBlue, Text = s });
            listView1.EnsureVisible(listView1.Items.Count - 1);
        }
        void addMessage(string s, int i)    //hiện tin nhắn có ảnh
        {
            listView1.Items.Add(new ListViewItem() { BackColor = Color.LightBlue, Text = s, ImageIndex = i });
            listView1.EnsureVisible(listView1.Items.Count - 1);
        }
        byte[] maHoa(object obj)    //hàm mã hóa dữ liệu để gửi
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
        object giaiMa(byte[] data)  //giải mã dữ liệu nhận được
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
            //return stream.ToArray();
        }
        private void sendbtn_Click(object sender, EventArgs e)  //gửi tin nhắn từ ô nhập text
        {
            if (messageBox.Text != string.Empty)
            {
                inbox[1] = messageBox.Text;
                client.Send(maHoa(inbox));
                addMessage("You   : " + messageBox.Text);
                messageBox.Clear();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)  //chọn 1 mục khác sẽ yêu cầu lên server để nhận đoạn chat lưu ở server
        {
            inbox[0] = listBox1.SelectedItem.ToString();
            if (inbox[0] != currentinbox)
            {
                currentinbox = inbox[0];
                if (currentinbox == "Server") deliverport = 1;
                else deliverport = int.Parse(inbox[0]);

                client.Send(maHoa(deliverport));
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)  //gọi khi listview1 được click
        {
            if (!File.Exists(listView1.FocusedItem.Text.Substring(8)) && contextMenuStrip1.Items.Count == 3)
                contextMenuStrip1.Items[0].Visible = false;
            else contextMenuStrip1.Items[0].Visible = true;

            if (listView1.FocusedItem.Text.Substring(0, 7) == "Server:") contextMenuStrip1.Items[2].Visible = false;
            else contextMenuStrip1.Items[2].Visible = true;

            if (listView1.FocusedItem.Text.Contains("(deleted)")) return;

            if (e.Button == MouseButtons.Right) //khi nhấn chuột phải vào item trong listview1 thì hiện menu (open fie, copy text, delete)
            {
                var focusedItem = listView1.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)    //nhấn đúp vào 1 tin nhắn chứa link tệp thì gọi hàm mở tệp
        {
            openToolStripMenuItem_Click(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)    // menu mở tệp
        {
            try
            {
                System.Diagnostics.Process.Start(listView1.FocusedItem.Text.Substring(8));
            }
            catch { }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)    //menu copy
        {
            if (listView1.FocusedItem.Text.Length >= 9) Clipboard.SetText(listView1.FocusedItem.Text.Substring(8));
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)  // menu xóa tin nhắn
        {
            if (MessageBox.Show("Are you sure to delete this conversation?", "",
                     MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string temp = listView1.FocusedItem.Text;
                
                if (listView1.FocusedItem.Text.Length == 8) listView1.Items.Remove(listView1.FocusedItem);
                else
                {
                    listView1.FocusedItem.Text = temp.Substring(0, 8) + "(deleted)";
                    del[0] = inbox[0];
                    del[1] = temp.Substring(8);
                    client.Send(maHoa(del));
                }
            }
        }

        private void filebtn_Click(object sender, EventArgs e)  //hàm chọn và gửi đường dẫn của tệp
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                addMessage("You   : " + open.FileName);
                inbox[1] = open.FileName;
                client.Send(maHoa(inbox));
            }
        }

        private void imagebtn_Click(object sender, EventArgs e)     //chọn và gửi ảnh
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files|*.gif; *.jpg; *.jpeg; *.bmp; *.wmf; *.png";
            if (open.ShowDialog() == DialogResult.OK)
            {
                id++;
                Image img = Image.FromFile(open.FileName);
                // call image list and fill it 
                imageList1.Images.Add(id + "", img);
                // add an item
                ListViewItem item = new ListViewItem();
                // and tell the item which image to use
                item.ImageKey = id + "";
                addMessage("You   : ", id);
            }
        }

        private void iconbtn_Click(object sender, EventArgs e)  //mở emoji box
        {
            listView2.Visible = listView2.Visible == true ? false : true;
        }

        private void FormClient_Load(object sender, EventArgs e)    //load và hiện icon lên listview2
        {
            Image img2 = Image.FromFile("D:\\VS_code\\C#\\Chat_LongLe\\Chat_LongLe\\Resources\\Emoji File\\1024px-Emoji_u263a.svg.png");
            imageList2.Images.Add(index + "", img2);
            listView2.Items.Add(new ListViewItem() { ImageIndex = index });
            index++;
            img2 = Image.FromFile("D:\\VS_code\\C#\\Chat_LongLe\\Chat_LongLe\\Resources\\Emoji File\\vuive2.png");
            imageList2.Images.Add(index + "", img2);
            listView2.Items.Add(new ListViewItem() { ImageIndex = index });
            index++;
            img2 = Image.FromFile("D:\\VS_code\\C#\\Chat_LongLe\\Chat_LongLe\\Resources\\Emoji File\\tsundere.png");
            imageList2.Images.Add(index + "", img2);
            listView2.Items.Add(new ListViewItem() { ImageIndex = index });
            index++;
            img2 = Image.FromFile("D:\\VS_code\\C#\\Chat_LongLe\\Chat_LongLe\\Resources\\Emoji File\\trithuc.png");
            imageList2.Images.Add(index + "", img2);
            listView2.Items.Add(new ListViewItem() { ImageIndex = index });
            index++;
            img2 = Image.FromFile("D:\\VS_code\\C#\\Chat_LongLe\\Chat_LongLe\\Resources\\Emoji File\\tramngam.png");
            imageList2.Images.Add(index + "", img2);
            listView2.Items.Add(new ListViewItem() { ImageIndex = index });
            index++;
            img2 = Image.FromFile("D:\\VS_code\\C#\\Chat_LongLe\\Chat_LongLe\\Resources\\Emoji File\\hoamat.png");
            imageList2.Images.Add(index + "", img2);
            listView2.Items.Add(new ListViewItem() { ImageIndex = index });
            index++;
            img2 = Image.FromFile("D:\\VS_code\\C#\\Chat_LongLe\\Chat_LongLe\\Resources\\Emoji File\\daudau.png");
            imageList2.Images.Add(index + "", img2);
            listView2.Items.Add(new ListViewItem() { ImageIndex = index });
        }

        private void listView2_Click(object sender, EventArgs e) //chọn icon paste vào ô nhập tin nhắn
        {
            switch (listView2.FocusedItem.ImageIndex)
            {
                case 0:
                    messageBox.Text += " :) ";
                    break;
                case 1:
                    messageBox.Text += " :D ";
                    break;
                case 2:
                    messageBox.Text += " (=///=) ";
                    break;
                case 3:
                    messageBox.Text += " 8D ";
                    break;
                case 4:
                    messageBox.Text += " :/ ";
                    break;
                case 5:
                    messageBox.Text += " (@ _ @) ";
                    break;
                case 6:
                    messageBox.Text += " (||> . <) ";
                    break;
                default:
                    break;
            }
        }

        private void listView2_Leave(object sender, EventArgs e) //bấm vào khu vực của control khác listView2 trong form thì ẩn listView2
        {
            listView2.Visible = false;
        }
    }
}
