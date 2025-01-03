using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace winHydra
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear(); // Önceki sonuçları temizle

            string characters = textBox1.Text; // Kullanıcıdan alınan karakterler
            int minLength = (int)numericUpDown1.Value; // Minimum uzunluk
            int maxLength = (int)numericUpDown2.Value; // Maksimum uzunluk

            if (string.IsNullOrEmpty(characters))
            {
                MessageBox.Show("Lütfen karakterleri giriniz!");
                return;
            }

            // Şifreleri oluştur ve ListBox'a ekle
            List<string> passwords = GeneratePasswords(characters, minLength, maxLength);
            foreach (var password in passwords)
            {
                listBox1.Items.Add(password);
            }

            MessageBox.Show($"{passwords.Count} adet şifre oluşturuldu.");
        }

        private List<string> GeneratePasswords(string characters, int minLength, int maxLength)
        {
            var result = new List<string>();

            for (int length = minLength; length <= maxLength; length++)
            {
                result.AddRange(GenerateCombinations(characters, length));
            }

            return result;
        }

        private IEnumerable<string> GenerateCombinations(string characters, int length)
        {
            if (length == 1)
                return characters.Select(c => c.ToString());

            var smallerCombinations = GenerateCombinations(characters, length - 1);
            return smallerCombinations.SelectMany(smaller => characters.Select(c => smaller + c));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("Kaydedilecek şifre yok!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text File|*.txt",
                Title = "Şifre Listesini Kaydet"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (var item in listBox1.Items)
                    {
                        writer.WriteLine(item);
                    }
                }

                MessageBox.Show("Şifre listesi başarıyla kaydedildi.");
            }
        }
    }
}
