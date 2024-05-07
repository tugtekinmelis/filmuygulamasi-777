using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;

namespace Film_Uygulama
{
    public partial class Form1 : Form
    {
        string baglanti = "Server=localhost;Database=film_arsiv;Uid=root;Pwd=;";
        string yeniAd;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ilk sıra
            string klasorYolu = @"poster";
            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
            }

            DgwDoldur();
            CmdDoldur();
        }


        void DgwDoldur()
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "SELECT * FROM filmler;";

                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                dgwFilmler.DataSource = dt;

               // dgwFilmler.Columns["poster"].Visible = false;
              
            }
        }


        void CmdDoldur()
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "SELECT DISTINCT tur FROM filmler;";

                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                cmbTur.DataSource = dt;

                cmbTur.DisplayMember = "tur";
                cmbTur.ValueMember = "tur";

            }

        }

        private void dgwFilmler_SelectionChanged(object sender, EventArgs e)
        {
            if(dgwFilmler.SelectedRows.Count > 0)
            {
                txtFilmAd.Text = dgwFilmler.SelectedRows[0].Cells["film_ad"].Value.ToString();
                txtYonetmen.Text = dgwFilmler.SelectedRows[0].Cells["yonetmen"].Value.ToString();
                txtYil.Text =   dgwFilmler.SelectedRows[0].Cells["yil"].Value.ToString();
                txtSure.Text = dgwFilmler.SelectedRows[0].Cells["sure"].Value.ToString();
                txtPuan.Text = dgwFilmler.SelectedRows[0].Cells["imdb_puan"].Value.ToString();
                cbOdul.Checked = Convert.ToBoolean(dgwFilmler.SelectedRows[0].Cells["film_odul"].Value);

               // string dosyaYolu = Environment.CurrentDirectory+"\\poster\\"+ dgwFilmler.SelectedRows[0].Cells["poster"].Value.ToString();
                cmbTur.SelectedValue = dgwFilmler.SelectedRows[0].Cells["tur"].Value.ToString();

                string dosyaYolu = Path.Combine(Environment.CurrentDirectory, "poster", dgwFilmler.SelectedRows[0].Cells["poster"].Value.ToString());

                pbResim.Image = null; // önce resmi temizle 

                if(File.Exists(dosyaYolu)) //varsa resmi göster 
                {
                    pbResim.Image = Image.FromFile(dosyaYolu);
                    pbResim.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (dgwFilmler.SelectedRows.Count > 0)
            {
                string sorgu = "UPDATE filmler SET film_ad=@filmad, yonetmen=@yonetmen, yil=@yil," +
                    "tur=@tur, sure=@sure, poster=@poster, imdb_puan=@puan, film_odul=@odul " +
                    "WHERE film_id=@satirid";
                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    baglan.Open();
                    MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                    cmd.Parameters.AddWithValue("filmad", txtFilmAd.Text);
                    cmd.Parameters.AddWithValue("yonetmen", txtYonetmen.Text);
                    cmd.Parameters.AddWithValue("yil", txtYil.Text);
                    cmd.Parameters.AddWithValue("tur", cmbTur.SelectedValue);
                    cmd.Parameters.AddWithValue("sure", txtSure.Text);
                    cmd.Parameters.AddWithValue("puan",Convert.ToDouble(txtSure.Text));
                    cmd.Parameters.AddWithValue("odul",cbOdul.Checked);
                    int film_id = Convert.ToInt32(dgwFilmler.SelectedRows[0].Cells["film_id"].Value);
                    cmd.Parameters.AddWithValue("@satirid",film_id);

                    cmd.Parameters.AddWithValue("@poster", yeniAd);

                    cmd.ExecuteNonQuery();

                    DgwDoldur();
                }
            }
        }

        private void pbResim_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "poster", yeniAd);

            File.Copy(kaynakDosya, hedefDosya);
            pbResim.Image = null; // önce resmi temizle 

            if (File.Exists(hedefDosya)) //varsa resmi göster 
            {
                pbResim.Image = Image.FromFile(hedefDosya);
                pbResim.SizeMode = PictureBoxSizeMode.StretchImage;
            }

        }
    }
}
