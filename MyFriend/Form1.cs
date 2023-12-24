using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFriend
{
    public partial class Form1 : Form
    {
        List<Friend> MyFriendList = new List<Friend>();
        Friend MyFriend;
        public Form1()
        {
            InitializeComponent();
        }

        //친구 추가 버튼
        private void button1_Click(object sender, EventArgs e)
        {
            string[] strArrFriend = new string[] { textBox1.Text, numericUpDown1.Value.ToString(), radioButton1.Checked ? "남성" : "여성" };
            MyFriend = new Friend(textBox1.Text, Convert.ToInt32(numericUpDown1.Value), radioButton1.Checked ? true : false);
            MyFriendList.Add(MyFriend);
            ListViewItem item = new ListViewItem(strArrFriend);
            listView1.Items.Add(item);
            textBox1.Text = "";
            numericUpDown1.Value = 0;
        }

        //친구 삭제 버튼
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = listView1.Items.Count - 1; i >= 0; i--)
            {
                if (listView1.Items[i].Checked)
                {
                    listView1.Items[i].Remove();
                    MyFriendList.RemoveAt(i);
                }
            }
        }

        //친구 수정 (LabelEdit)
        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            //e.Item.ToString(); //수정한Index
            //e.Label; 수정내용
            listView1.Items[e.Item].Text = e.Label;
        }

        //메뉴 - 끝내기
        private void 끝내기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //메뉴 - 파일 저장하기
        private void 저장ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ret = saveFileDialog1.ShowDialog();
            if (ret != DialogResult.OK) return;
            FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(MyFriendList.Count());
            foreach (var fr in MyFriendList)
                   fr.WriteInfo(bw);
            fs.Close();
        }

        //파일 - 불러오기
        private void 불러오기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ret = openFileDialog1.ShowDialog();
            if (ret != DialogResult.OK) return;

            //기존 데이터 삭제
            listView1.Items.Clear();
            MyFriendList.Clear();

            try
            {
                FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                int nCount = br.ReadInt32();

                for (int i = 0; i < nCount; i++)
                {
                    MyFriendList.Add(Friend.ReadInfo(br));
                    string[] strArrFriend = new string[] { MyFriendList[i].m_strName, MyFriendList[i].m_nAge.ToString(), MyFriendList[i].m_bGender ? "남성" : "여성" };
                    ListViewItem item = new ListViewItem(strArrFriend);
                    listView1.Items.Add(item);
                }
                fs.Close();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        //친구 클래스
        [Serializable]
        class Friend
        {
            public string m_strName;
            public int m_nAge;
            public bool m_bGender;

            //생성자
            public Friend(string _strName, int _nAge, bool _Gender)
            {
                m_strName = _strName;
                m_nAge = _nAge;
                m_bGender = _Gender;
            }

            //Freind 저장
            public void WriteInfo(BinaryWriter bw)
            {
                bw.Write(m_strName);
                bw.Write(m_nAge);
                bw.Write(m_bGender);
            }

            //Friend 불러오기
            public static Friend ReadInfo(BinaryReader br)
            {
                string m_strName = br.ReadString();
                int m_nAge = br.ReadInt32();
                bool m_bGender = br.ReadBoolean();
                return new Friend(m_strName, m_nAge, m_bGender);
            }
        }

        private void 저장SeriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ret = saveFileDialog1.ShowDialog();
            if (ret != DialogResult.OK) return;
            FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(fs, MyFriendList.Count());
            bf.Serialize(fs, MyFriendList);
            fs.Close();
        }

        private void 불러오기SeriToolStripMenuItem_Click(object sender, EventArgs e)
        {    
            var ret = openFileDialog1.ShowDialog();
            if (ret != DialogResult.OK) return;

            //기존 데이터 삭제
            listView1.Items.Clear();
            MyFriendList.Clear();
            try
            {
                FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();

                int nCount = (int)bf.Deserialize(fs);
                MyFriendList = (List<Friend>)bf.Deserialize(fs);
                for (int i = 0; i < nCount; i++)
                {
                    string[] strArrFriend = new string[] { MyFriendList[i].m_strName, MyFriendList[i].m_nAge.ToString(), MyFriendList[i].m_bGender ? "남성" : "여성" };
                    ListViewItem item = new ListViewItem(strArrFriend);
                    listView1.Items.Add(item);
                }
                fs.Close();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }
    }
}
