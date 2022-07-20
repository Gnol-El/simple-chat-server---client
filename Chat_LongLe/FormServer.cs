using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Chat_LongLe
{
    public partial class FormServer : Form
    {
        IPEndPoint IP;
        Socket server;
        private List<Socket> clientlist;    //chứa danh sách client
        private List<string> inboxlist;     //chứa danh sách client và server để gửi cho client

        string path = "D:\\VS_code\\C#\\Chat_LongLe\\Chat_LongLe\\bin\\Debug\\inbox-lines\\";   //đường dẫn file lưu các đoạn chat 
        private List<object> lines;         //chứa các dòng chat để mã hóa gửi cho client

        int index = 0;
        public FormServer()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            ketnoi();

            inboxlist = new List<string>();
            lines = new List<object>();
        }
        private void FormServer_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }                           //xóa tất cả file trong thư mục chứa thông tin chat khi form đóng
            server.Close();     //đóng server
        }
        void ketnoi()
        {
            clientlist = new List<Socket>();

            IP = new IPEndPoint(IPAddress.Any, 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(IP);

            Thread Listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();

                        clientlist.Add(client);
                        listView1.Items.Add(new ListViewItem() { ForeColor = Color.Gray,
                            Text = ((IPEndPoint)client.RemoteEndPoint).Port + " has connected" });
                        listBox1.Items.Add(((IPEndPoint)client.RemoteEndPoint).Port);
                        inboxchange();

                        Thread receive = new Thread(received);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any, 9999);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
            });
            Listen.IsBackground = true;
            Listen.Start();
        }
        void received(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    
                    if (giaiMa(data).GetType().ToString() == "System.Int32") //hiện dữ liệu chat cho client
                    {
                        int showmessage = (int)giaiMa(data);
                        if (showmessage == 1)
                        {
                            if (File.Exists(path + "Server.txt"))
                            {
                                lines.Clear();
                                foreach (string l in File.ReadAllLines(path + "Server.txt"))
                                {
                                    lines.Add(l);
                                }
                                client.Send(maHoa(lines));
                            }
                            else
                            {
                                File.AppendAllText(path + "Server.txt", "Server" + Environment.NewLine);
                                lines.Clear();
                                foreach (string l in File.ReadAllLines(path + "Server.txt"))
                                {
                                    lines.Add(l);
                                }
                                client.Send(maHoa(lines));
                            }
                        }
                        else
                        {
                            string temp;
                            if (File.Exists(path + ((IPEndPoint)client.RemoteEndPoint).Port + "-" + showmessage + ".txt"))
                                temp = ((IPEndPoint)client.RemoteEndPoint).Port + "-" + showmessage + ".txt";
                            else if (File.Exists(path + showmessage + "-" + ((IPEndPoint)client.RemoteEndPoint).Port + ".txt"))
                                temp = showmessage + "-" + ((IPEndPoint)client.RemoteEndPoint).Port + ".txt";
                            else temp = null;

                            if (temp != null)
                            {
                                lines.Clear();
                                foreach (string l in File.ReadAllLines(path + temp))
                                {
                                    lines.Add(l);
                                }
                                client.Send(maHoa(lines));
                            }
                            else
                            {
                                File.AppendAllText(path + ((IPEndPoint)client.RemoteEndPoint).Port + "-" + showmessage + ".txt",
                                    ((IPEndPoint)client.RemoteEndPoint).Port + Environment.NewLine +
                                    showmessage + Environment.NewLine);
                                lines.Clear();
                                foreach (string l in File.ReadAllLines(path + ((IPEndPoint)client.RemoteEndPoint).Port + "-" + showmessage + ".txt"))
                                {
                                    lines.Add(l);
                                }
                                client.Send(maHoa(lines));
                            }
                        }
                    }
                    else if (giaiMa(data).GetType().ToString() == "System.Collections.Generic.List`1[System.Object]") //xóa tin nhắn của client
                    {
                        List<object> list = new List<object>();
                        list = (List<object>)giaiMa(data);

                        if (list[0].ToString() == "Server")
                        {
                            deletedwritefile(client, list, "Server.txt");
                            foreach (Socket item in clientlist)
                            {
                                if (item != null && item != client) item.Send(maHoa(lines));
                            }

                            listView1.Items.Clear();
                            foreach (object l in lines)
                            {
                                if (!l.ToString().Contains(":")) continue;
                                else addMessage(l.ToString());
                            }
                        }
                        else
                        {
                            string filename = ((IPEndPoint)client.RemoteEndPoint).Port + "-" + list[0] + ".txt";
                            if (!File.Exists(path + filename)) filename = list[0] + "-" + ((IPEndPoint)client.RemoteEndPoint).Port + ".txt";

                            deletedwritefile(client, list, filename);
                            foreach (Socket item in clientlist)
                            {
                                if (item != null && item == client) item.Send(maHoa(lines));
                            }
                        }
                    }
                    else    // gửi tin nhắn của client
                    {
                        string[] message = (string[])giaiMa(data);
                        message[1] = ((IPEndPoint)client.RemoteEndPoint).Port + " : " + message[1];

                        if (message[0] == "Server")
                        {
                            if (!File.Exists(path + "Server.txt")) File.AppendAllText(path + "Server.txt", message[0] + Environment.NewLine);
                            File.AppendAllText(path + "Server.txt", message[1] + Environment.NewLine);

                            foreach (Socket item in clientlist)
                            {
                                if (item != null && item != client) item.Send(maHoa(message));
                            }
                            addMessage(message[1]);
                        }
                        else
                        {
                            string filename = ((IPEndPoint)client.RemoteEndPoint).Port + "-" + message[0] + ".txt";
                            if (File.Exists(path + filename))
                                File.AppendAllText(path + filename, message[1] + Environment.NewLine);
                            else
                            {
                                string temp = message[0] + "-" + ((IPEndPoint)client.RemoteEndPoint).Port + ".txt";
                                if (File.Exists(path + temp))
                                {
                                    File.AppendAllText(path + temp, message[1] + Environment.NewLine);
                                    filename = temp;
                                }
                                else
                                {
                                    File.AppendAllText(path + filename, ((IPEndPoint)client.RemoteEndPoint).Port + Environment.NewLine);
                                    File.AppendAllText(path + filename, message[0] + Environment.NewLine);
                                    File.AppendAllText(path + filename, message[1] + Environment.NewLine);
                                }
                            }
                            
                            foreach (Socket item in clientlist)
                            {
                                if (item != null && ((IPEndPoint)item.RemoteEndPoint).Port.ToString() == message[0])
                                {
                                    message[0] = ((IPEndPoint)client.RemoteEndPoint).Port.ToString();
                                    item.Send(maHoa(message));
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                clientlist.Remove(client);
                inboxlist.Remove(((IPEndPoint)client.RemoteEndPoint).Port.ToString());
                listBox1.Items.Remove(((IPEndPoint)client.RemoteEndPoint).Port);
                listView1.Items.Add(new ListViewItem() { ForeColor = Color.Red,
                    Text = ((IPEndPoint)client.RemoteEndPoint).Port + " has disconnected" });
                inboxchange();
                client.Close();
            }
        }
        private void deletedwritefile(Socket c, List<object> l, string name)
        {
            string firsthalf = Path.GetTempFileName();
            string lasthalf = Path.GetTempFileName();
            string fullpart = Path.GetTempFileName();
            bool check = false;

            lines.Clear();
            foreach (var line in File.ReadAllLines(path + name))
            {
                if (line == ((IPEndPoint)c.RemoteEndPoint).Port + " : " + l[1].ToString() ||
                    line == l[0] + " : " + l[1].ToString())
                {
                    check = true;
                    File.AppendAllText(firsthalf, ((IPEndPoint)c.RemoteEndPoint).Port + " : (deleted)" + Environment.NewLine);
                    lines.Add(((IPEndPoint)c.RemoteEndPoint).Port + " : (deleted)");
                }
                else if (check)
                {
                    File.AppendAllText(lasthalf, line + Environment.NewLine);
                    lines.Add(line);
                }
                else
                {
                    File.AppendAllText(firsthalf, line + Environment.NewLine);
                    lines.Add(line);
                }
            }

            File.AppendAllText(fullpart, File.ReadAllText(firsthalf) + File.ReadAllText(lasthalf));
            File.Delete(path + name);
            File.Move(fullpart, path + name);
        }

        byte[] maHoa(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
        object giaiMa(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
            //return stream.ToArray();
        }
        void addMessage(string s)
        {
            listView1.Items.Add(new ListViewItem() { BackColor = Color.LightBlue, Text = s });
            listView1.EnsureVisible(listView1.Items.Count - 1);
        }
        void addMessage(string s, int id)
        {
            listView1.Items.Add(new ListViewItem() { BackColor = Color.LightBlue, Text = s, ImageIndex = id });
            listView1.EnsureVisible(listView1.Items.Count - 1);
        }

        private void sendbtn_Click(object sender, EventArgs e)
        {
            if (messageBox.Text != "")
            {
                messageBox.Text = "Server: " + messageBox.Text;
                foreach (Socket item in clientlist)
                {
                    if (item != null && messageBox.Text != string.Empty) item.Send(maHoa(messageBox.Text));
                }
                if (!File.Exists(path + "Server.txt")) File.AppendAllText(path + "Server.txt", "Server" + Environment.NewLine);
                File.AppendAllText(path + "Server.txt", messageBox.Text + Environment.NewLine);
                addMessage(messageBox.Text);
                messageBox.Clear();
            }
        }

        private void inboxchange()
        {
            inboxlist.Clear();
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                inboxlist.Add(listBox1.Items[i].ToString());
            }
            foreach (Socket i in clientlist)
            {
                if (i != null) i.Send(maHoa(inboxlist));
            }
        }

        private void filebtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.DefaultExt = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                addMessage("Server: " + open.FileName);
                File.AppendAllText(path + "Server.txt", "Server: " + open.FileName + Environment.NewLine);
                foreach (Socket item in clientlist)
                {
                    if (item != null) item.Send(maHoa("Server: " + open.FileName));
                }
            }
        }

        int id = -1;
        private void imagebtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.DefaultExt = "Image Files|*.gif; *.jpg; *.jpeg; *.bmp; *.wmf; *.png";
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
                addMessage("Server: " , id);

                foreach (Socket i in clientlist)
                {
                    if (i != null) i.Send(maHoa(img));
                }
            }
        }

        private void iconbtn_Click(object sender, EventArgs e)
        {
            listView2.Visible = listView2.Visible == true ? false : true;
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!File.Exists(listView1.FocusedItem.Text.Substring(8)) && contextMenuStrip1.Items.Count == 3) 
                contextMenuStrip1.Items[0].Visible = false;
            else contextMenuStrip1.Items[0].Visible = true;
            if (listView1.FocusedItem.Text.Contains("(deleted)")) return;
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = listView1.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(listView1.FocusedItem.Text.Substring(8));
            }
            catch{}
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.FocusedItem.Text.Length >= 9) Clipboard.SetText(listView1.FocusedItem.Text.Substring(8));
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string firsthalf = Path.GetTempFileName();
            string lasthalf = Path.GetTempFileName();
            string fullpart = Path.GetTempFileName();
            bool check = false;
            string temp = listView1.FocusedItem.Text;
            if (MessageBox.Show("Are you sure to delete this conversation?", "",
                     MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (listView1.FocusedItem.Text.Length == 8) listView1.Items.Remove(listView1.FocusedItem);
                else
                {
                    listView1.FocusedItem.Text = temp.Substring(0, 8) + "(deleted)";
                    lines.Clear();
                    foreach (var line in File.ReadAllLines(path + "Server.txt"))
                    {
                        if (line == temp)
                        {
                            check = true;
                            File.AppendAllText(firsthalf, temp.Substring(0, 8) + "(deleted)" + Environment.NewLine);
                            lines.Add(temp.Substring(0, 8) + "(deleted)");
                        }
                        else if (check)
                        {
                            File.AppendAllText(lasthalf, line + Environment.NewLine);
                            lines.Add(line);
                        }
                        else
                        {
                            File.AppendAllText(firsthalf, line + Environment.NewLine);
                            lines.Add(line);
                        }
                    }

                    File.AppendAllText(fullpart, File.ReadAllText(firsthalf) + File.ReadAllText(lasthalf));
                    File.Delete(path + "Server.txt");
                    File.Move(fullpart, path + "Server.txt");

                    foreach (Socket item in clientlist)
                    {
                        if (item != null) item.Send(maHoa(lines));
                    } 
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        private void listView2_Leave(object sender, EventArgs e)
        {
            listView2.Visible = false;
        }

        private void FormServer_Load(object sender, EventArgs e)
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

        private void listView2_Click(object sender, EventArgs e)
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
    }
}
