using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace FRscound
{
    public partial class Form1 : Form
    {
        private Color selectedColor = Color.Red;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();

       
            try
            {
                discordLogo.BackgroundImage = CreateDiscordLogo();
                discordLogo.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { }

  
            textBox1.Text = "Type your message here...";
            textBox1.ForeColor = Color.Silver;
            textBox1.Enter += TextBox1_Enter;
            textBox1.Leave += TextBox1_Leave;

      
            colorIndicator.BackColor = selectedColor;

         
            ApplyRoundedCorners(messagePanel, 8);
            ApplyRoundedCorners(colorIndicator, 5);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
       
        }

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Type your message here...")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.White;
            }
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Type your message here...";
                textBox1.ForeColor = Color.Silver;
            }
        }

        private void titleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void titleLabel_MouseDown(object sender, MouseEventArgs e)
        {
            titleBar_MouseDown(sender, e);
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonPickColor_Click(object sender, EventArgs e)
        {
          
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.FullOpen = true;
                colorDialog.AnyColor = true;
                colorDialog.Color = selectedColor;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedColor = colorDialog.Color;
                    colorIndicator.BackColor = selectedColor;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://discord.com/api/webhooks/1346845854329213009/7_f0eKVzQk4oiznVEuZ5JigkrcbAgqOCaI7cmFeajZjX_m80zicUhkcOo-MqbhdLhsxc";


            if (string.IsNullOrWhiteSpace(textBox1.Text) || textBox1.Text == "Type your message here...")
            {
                ShowStatus("Please enter a message", Color.Orange);
                return;
            }

            string message = textBox1.Text.Replace("\"", "\\\"");
            int discordColor = (selectedColor.R << 16) | (selectedColor.G << 8) | selectedColor.B;
            string jsonpayload = "{ \"embeds\": [ { \"description\": \"" + message + "\", \"color\": " + discordColor + " } ] }";

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Encoding = Encoding.UTF8;
                    client.UploadString(url, "POST", jsonpayload);
                }

                ShowStatus("Message sent successfully!", Color.LimeGreen);

                textBox1.Text = "Type your message here...";
                textBox1.ForeColor = Color.Silver;
            }
            catch (Exception ex)
            {
                ShowStatus("Error: " + ex.Message, Color.Red);
            }
        }

     
        private void ShowStatus(string message, Color color)
        {
            statusLabel.Text = message;
            statusLabel.ForeColor = color;
            statusLabel.Visible = true;

          
            Timer fadeTimer = new Timer { Interval = 3000 };
            fadeTimer.Tick += (s, e) =>
            {
                statusLabel.Visible = false;
                fadeTimer.Stop();
                fadeTimer.Dispose();
            };
            fadeTimer.Start();
        }

  
        private void ApplyRoundedCorners(Control control, int radius)
        {
            Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Width - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Width - radius * 2, rect.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseAllFigures();
            control.Region = new Region(path);
        }

        private Bitmap CreateDiscordLogo()
        {
            Bitmap logo = new Bitmap(25, 25);
            using (Graphics g = Graphics.FromImage(logo))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

            
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(brush, 0, 0, 25, 25);
                }

            
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(114, 137, 218)))
                {
                    g.FillEllipse(brush, 5, 5, 15, 15);
                }
            }
            return logo;
        }
    }
}