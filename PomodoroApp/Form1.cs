using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace PomodoroApp
{
    
    public partial class Form1: Form
    {
        // Çalışma ve mola sürelerini tanımla (saniye cinsinden)
        private int workTime = 1 * 10;   // 25 dakika çalışma
        private int shortBreak = 5 * 1;  // 5 dakika kısa mola
        private int longBreak = 15 * 1;  // 15 dakika uzun mola
        private int currentTime;          // Şu anki geri sayım süresi
        private int pomodoroCount = 0;    // Kaç pomodoro tamamlandığını takip et
        private bool isWorking = true;    // Çalışma veya mola durumunu belirle

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;

            #region Buton Özelleştirmeleri
            this.BackColor = ColorTranslator.FromHtml("#1E1E1E"); // Arka planı koyu yap


            // Başlat Butonu
            btnStart.BackColor = ColorTranslator.FromHtml("#4CAF50"); // Yeşil
            btnStart.ForeColor = Color.White;
            btnStart.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 2;
            btnStart.FlatAppearance.BorderColor = Color.Black;
            btnStart.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#66BB6A");
            btnStart.FlatAppearance.MouseDownBackColor = ColorTranslator.FromHtml("#388E3C");

            

            // Duraklat Butonu
            btnPause.BackColor = ColorTranslator.FromHtml("#FFCC00"); // Sarı
            btnPause.ForeColor = Color.Black;
            btnPause.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnPause.FlatStyle = FlatStyle.Flat;
            btnPause.FlatAppearance.BorderSize = 2;
            btnPause.FlatAppearance.BorderColor = Color.Black;
            btnPause.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#FFD54F");
            btnPause.FlatAppearance.MouseDownBackColor = ColorTranslator.FromHtml("#FFA000");

            // Sıfırla Butonu
            btnReset.BackColor = ColorTranslator.FromHtml("#2196F3"); // Mavi
            btnReset.ForeColor = Color.White;
            btnReset.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.FlatAppearance.BorderSize = 2;
            btnReset.FlatAppearance.BorderColor = Color.Black;
            btnReset.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#64B5F6");
            btnReset.FlatAppearance.MouseDownBackColor = ColorTranslator.FromHtml("#1976D2");

            // Çıkış Butonu
            btnExit.BackColor = ColorTranslator.FromHtml("#F44336"); // Kırmızı
            btnExit.ForeColor = Color.White;
            btnExit.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 2;
            btnExit.FlatAppearance.BorderColor = Color.Black;
            btnExit.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#E57373");
            btnExit.FlatAppearance.MouseDownBackColor = ColorTranslator.FromHtml("#D32F2F");
            #endregion

            #region Çıkış Butonu Özelleştirmeleri

            btnExit.BackColor = ColorTranslator.FromHtml("#DC143C"); 
            btnExit.ForeColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 2;
            btnExit.FlatAppearance.BorderColor = Color.Black;

            btnExit.MouseEnter += (s, e) => btnExit.BackColor = ColorTranslator.FromHtml("#8B0000"); 
            btnExit.MouseLeave += (s, e) => btnExit.BackColor = ColorTranslator.FromHtml("#DC143C");


            #endregion

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000; // 1 saniye aralıklarla çalışacak
            currentTime = workTime; // Başlangıçta çalışma süresi
            progressBar1.Maximum = workTime; // ProgressBar'ın maksimum değeri çalışma süresi
            progressBar1.Value = currentTime; // Başlangıçta progressBar'ın değeri de çalışma süresiyle aynı
            progressBar1.Style = ProgressBarStyle.Continuous;
            UpdateTimerDisplay();   // Zamanı ekrana yazdır
            lblMessage.Text = "Şimdi odaklanma zamanı!"; // Başlangıç mesajı
            lblTimer.BackColor = Color.Transparent; // Label arka planını şeffaf yapar
            lblTimer.ForeColor = Color.White;
            lblMessage.BackColor = Color.Transparent; // Label arka planını şeffaf yapar
            lblMessage.ForeColor = Color.White;
            BackgroundImage = Image.FromFile(@"D:\FirstProjectTest\Yeni klasör\PomodoroApp\PomodoroApp\Resources\work.jpg");

        }


        private void PlayAlertSound(string soundType)
        {
            string soundPath = string.Empty;

            // Ses türüne göre doğru ses dosyasını seç
            switch (soundType)
            {
                case "work_end":
                    soundPath = @"D:\FirstProjectTest\Yeni klasör\PomodoroApp\PomodoroApp\Resources\work_end.wav"; // Çalışma bitti sesi
                    break;
                case "short_break_end":
                    soundPath = @"D:\FirstProjectTest\Yeni klasör\PomodoroApp\PomodoroApp\Resources\short_break_end.wav"; // Kısa mola bitti sesi
                    break;
                case "long_break_end":
                    soundPath = @"D:\FirstProjectTest\Yeni klasör\PomodoroApp\PomodoroApp\Resources\long_break_end.wav"; // Uzun mola bitti sesi
                    break;
                default:
                    return;
            }

            // Ses dosyasını çal
            SoundPlayer player = new SoundPlayer(soundPath);
            player.Play();
        }


        private void UpdateTimerDisplay()
        {
            int minutes = currentTime / 60;
            int seconds = currentTime % 60;
            lblTimer.Text = $"{minutes:D2}:{seconds:D2}"; // MM:SS formatı
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            pomodoroCount = 0;
            isWorking = true;
            currentTime = workTime;
            UpdateTimerDisplay();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (currentTime > 0)
            {
                currentTime--;
                progressBar1.Value = Math.Min(currentTime, progressBar1.Maximum);
                UpdateTimerDisplay();
            }
            else
            {
                timer1.Stop();

                if (isWorking)
                {
                    pomodoroCount++;
                    if (pomodoroCount % 4 == 0)
                    {
                        currentTime = longBreak; // 4 Pomodoro tamamlandıysa uzun mola
                        lblMessage.Text = "Harika! Uzun mola zamanı!";
                        PlayAlertSound("long_break_end"); // Uzun mola bitti sesini çal
                        BackgroundImage = Image.FromFile(@"D:\FirstProjectTest\Yeni klasör\PomodoroApp\PomodoroApp\Resources\break1.jpg");
                    }
                    else
                    {
                        currentTime = shortBreak; // Normal mola süresi
                        lblMessage.Text = "Tebrikler! Kısa bir mola ver.";
                        PlayAlertSound("short_break_end"); // Kısa mola bitti sesini çal
                        BackgroundImage = Image.FromFile(@"D:\FirstProjectTest\Yeni klasör\PomodoroApp\PomodoroApp\Resources\break2.jpg");
                    }
                }
                else
                {
                    currentTime = workTime;
                    lblMessage.Text = "Şimdi odaklanma zamanı!";
                    PlayAlertSound("work_end"); // Çalışma bitti sesini çal
                    BackgroundImage = Image.FromFile(@"D:\FirstProjectTest\Yeni klasör\PomodoroApp\PomodoroApp\Resources\work.jpg");
                }

                isWorking = !isWorking; // Çalışma <-> mola durumunu değiştir
                UpdateTimerDisplay();
                progressBar1.Value = Math.Min(currentTime, progressBar1.Maximum); // ProgressBar'ın değeri güncelle
                timer1.Start(); // Yeni süreyle zamanlayıcıyı başlat
                if (!isWorking)
                {
                    progressBar1.Value = 0;  // Mola başladığında progress bar'ı sıfırla
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Uygulamayı kapatmak istiyor musunuz?", "Çıkış Onayı",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit(); 
            }
        }
    }
}
