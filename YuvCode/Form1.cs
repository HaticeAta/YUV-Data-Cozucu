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

namespace YuvCode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        String dosyayolu;
        private void button2_Click(object sender, EventArgs e)  // Dosya Açma Butonu
        {
            OpenFileDialog dosyasec = new OpenFileDialog();
            dosyasec.Title = "YUV Dosyası Seç..";
            dosyasec.Filter = "YUV files (*.yuv)|*.yuv";
            if (dosyasec.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dosyayolu = dosyasec.FileName;
                String dosyaadi = dosyasec.SafeFileName;
                textBox2.Text = dosyaadi;
            }
            else
            {
                MessageBox.Show("Dosya Açılamadı", "Error",0, MessageBoxIcon.Error);
            }
           
        }

        int en, boy;
        private void button1_Click(object sender, EventArgs e)  // Görüntü Okuma Butonu
        {
            goruntuframe = 0;
            String format;
            String dosyatext;

            dosyatext = textBox2.Text;
            if(String.Compare(dosyatext, "") != 0)
            {
                format = comboBox1.Text;
                if (String.Compare(format, "") == 0)
                {
                    MessageBox.Show(" Format Bilgisi Seçiniz ... ", "Error", 0, MessageBoxIcon.Error);
                }
                else
                {
                   
                    if (String.Compare(textBoxEn.Text, "")!=0 && String.Compare(textBoxBoy.Text, "") != 0)
                    {
                         en = Convert.ToInt32(textBoxEn.Text);   // Genislik
                         boy = Convert.ToInt32(textBoxBoy.Text); // Yukseklik
                         if (String.Compare(format, "YUV444") == 0)
                         {
                            format_444();
                         }
                         if (String.Compare(format, "YUV422") == 0)
                         {
                            format_422();
                         }
                         if (String.Compare(format, "YUV420") == 0)
                         {
                             format_420();
                         }
                    }
                    else
                    {
                          MessageBox.Show(" Yükseklik ve Genişlik Bilgisi Boş Olamaz ... ", "Error", 0, MessageBoxIcon.Error);
                    }     
                }
            }
            else
            {
                MessageBox.Show(" Dosya Seçiniz ... ", "Error", 0, MessageBoxIcon.Error);
            }
            
        }

        int pikselsayisi;
        int framesayisi;
        int bytesayisi;
        int[,] renk_r;
        int[,] renk_g;
        int[,] renk_b;
        Byte[] tumbaytlar;
        Byte[,] y;
        Byte[,] u;
        Byte[,] v;

        private void format_444()
        {
            FileStream fs = new FileStream(dosyayolu, FileMode.Open);   

            bytesayisi = Convert.ToInt32(fs.Length);    // Dosyadaki toplam byte sayisi
            pikselsayisi = en * boy;
            framesayisi = bytesayisi /(pikselsayisi*3);   

            tumbaytlar = new byte[bytesayisi];

            int a = 0;
            for (int bayt = 0; bayt < bytesayisi; bayt++)
            {
                a = fs.ReadByte();
                tumbaytlar[bayt] = (Byte)a;
            }

            fs.Close();

            y = new byte[framesayisi, pikselsayisi];
            u = new byte[framesayisi, pikselsayisi];
            v = new byte[framesayisi, pikselsayisi];

            for (int ff = 0; ff < framesayisi; ff++)
            {
                for (int yi = 0; yi < pikselsayisi; yi++)
                {
                   y[ff, yi] = tumbaytlar[(pikselsayisi * ff*3) + yi];
                }
                for (int ui = pikselsayisi; ui < pikselsayisi*2; ui++)
                {
                  u[ff, ui-pikselsayisi] = tumbaytlar[(pikselsayisi * ff*3) +ui];
                }
                for (int vi = pikselsayisi * 2; vi < pikselsayisi * 3; vi++)
                {
                   v[ff, vi - pikselsayisi * 2] = tumbaytlar[(pikselsayisi * ff*3) + vi];
                }
            }
            
            renk_r = new int[framesayisi, pikselsayisi];
            renk_g = new int[framesayisi, pikselsayisi];
            renk_b = new int[framesayisi, pikselsayisi];
            
            for (int i = 0; i < framesayisi; i++)
            {
                for(int j = 0; j < pikselsayisi; j++)
                {
                    renk_r[i, j] = RGB_R(y[i, j], v[i, j]);
                    renk_g[i, j] = RGB_G(y[i, j], u[i,j],v[i, j]);
                    renk_b[i, j] = RGB_B(y[i, j], u[i, j]);
                }
            }

            ResimGoster(0);
            
        }


        private void format_422()
        {
            FileStream fs = new FileStream(dosyayolu, FileMode.Open);

            bytesayisi = Convert.ToInt32(fs.Length);
            pikselsayisi = en * boy;
            framesayisi = bytesayisi / (pikselsayisi * 2);
            tumbaytlar = new byte[bytesayisi];

            int a = 0;
            for (int bayt = 0; bayt < bytesayisi; bayt++)
            {
                a = fs.ReadByte();
                tumbaytlar[bayt] = (Byte)a;
            }

            fs.Close();

            y = new byte[framesayisi, pikselsayisi];
            u = new byte[framesayisi, pikselsayisi/2];
            v = new byte[framesayisi, pikselsayisi/2];
           
            for (int ff = 0; ff < framesayisi; ff++)
            {
                for (int yi = 0; yi < pikselsayisi; yi++)
                {
                    y[ff, yi] = tumbaytlar[(pikselsayisi * ff * 2) + yi];
                }
                for (int ui = pikselsayisi; ui < (pikselsayisi * 3)/2; ui++)
                {
                    u[ff, ui - pikselsayisi] = tumbaytlar[(pikselsayisi * ff * 2) + ui];
                }
                for (int vi = (pikselsayisi * 3)/2; vi < pikselsayisi * 2; vi++)
                {
                    v[ff, vi - ((pikselsayisi * 3)/2)] = tumbaytlar[(pikselsayisi * ff * 2) + vi];
                }
            }
            
            renk_r = new int[framesayisi, pikselsayisi];
            renk_g = new int[framesayisi, pikselsayisi];
            renk_b = new int[framesayisi, pikselsayisi];
                        
            for (int i = 0; i < framesayisi; i++)
            {
                for (int j = 0; j < pikselsayisi; j=j+2)
                {
                    renk_r[i, j] = RGB_R(y[i, j], v[i, j / 2]);
                    renk_r[i, j+1] = RGB_R(y[i, j+1], v[i, j / 2]);

                    renk_g[i, j]     = RGB_G(y[i, j], u[i,j/2]  ,v[i, j / 2]);
                    renk_g[i, j + 1] = RGB_G(y[i, j + 1], u[i, j / 2], v[i, j / 2]);

                    renk_b[i, j] = RGB_B(y[i, j], u[i, j / 2]);
                    renk_b[i, j + 1] = RGB_B(y[i, j + 1], u[i, j / 2]);
                }
            }

            ResimGoster(0);

        }

        private void format_420()
        {
            FileStream fs = new FileStream(dosyayolu, FileMode.Open);

            bytesayisi = Convert.ToInt32(fs.Length);
            pikselsayisi = en * boy;
            framesayisi = (int)((float)bytesayisi / ((float)pikselsayisi * 1.5));

            tumbaytlar = new byte[bytesayisi];

            int a = 0;
            for (int bayt = 0; bayt < bytesayisi; bayt++)
            {
                a = fs.ReadByte();
                tumbaytlar[bayt] = (Byte)a;
            }

            fs.Close();

            y = new byte[framesayisi, pikselsayisi];
            u = new byte[framesayisi, pikselsayisi / 4];
            v = new byte[framesayisi, pikselsayisi / 4];


            for (int ff = 0; ff < framesayisi; ff++)
            {
                for (int yi = 0; yi < pikselsayisi; yi++)
                {
                    y[ff, yi] = tumbaytlar[((pikselsayisi * ff * 3)/2) + yi];
                }
                for (int ui = pikselsayisi; ui < pikselsayisi+(pikselsayisi/4); ui++)
                {
                    u[ff, ui-pikselsayisi] = tumbaytlar[((pikselsayisi * ff * 3) / 2) + ui];
                }
                for (int vi = pikselsayisi + (pikselsayisi / 4); vi < pikselsayisi+(pikselsayisi/2); vi++)
                {
                    v[ff, vi-( pikselsayisi + (pikselsayisi / 4))] = tumbaytlar[((pikselsayisi * ff * 3) / 2) + vi];
                }
            }

            renk_r = new int[framesayisi, pikselsayisi];
            renk_g = new int[framesayisi, pikselsayisi];
            renk_b = new int[framesayisi, pikselsayisi];
            
            int s = 0;
            for(int ff = 0; ff < framesayisi; ff++)
            {
                for (int i = 0; i < boy; i = i + 2)
                {
                    for (int j = 0; j < en; j = j + 2)
                    {
                        renk_r[ff, (i * en) + j] = RGB_R(y[ff, (i * en) + j], v[ff, s]);
                        renk_r[ff, (i * en) + j + 1] = RGB_R(y[ff, (i * en) + j + 1], v[ff, s]);
                        renk_r[ff, (i * en) +j+ en] = RGB_R(y[ff, (i * en) + j + en], v[ff, s]);
                        renk_r[ff, (i * en) + j+en + 1] = RGB_R(y[ff, (i * en) + j + en + 1], v[ff, s]);

                        renk_g[ff, (i * en) + j] = RGB_G(y[ff, (i * en) + j], u[ff, s], v[ff, s]);
                        renk_g[ff, (i * en) + j + 1] = RGB_G(y[ff, (i * en) + j + 1], u[ff, s], v[ff, s]);
                        renk_g[ff, (i * en) + j+en] = RGB_G(y[ff, (i * en) + j + en], u[ff, s], v[ff, s]);
                        renk_g[ff, (i * en) + j+en + 1] = RGB_G(y[ff, (i * en) + j + en + 1], u[ff, s], v[ff, s]);

                        renk_b[ff, (i * en) + j] = RGB_B(y[ff, (i * en) + j], u[ff, s]);
                        renk_b[ff, (i * en) + j + 1] = RGB_B(y[ff, (i * en) + j + 1], u[ff, s]);
                        renk_b[ff, (i * en) + j+en] = RGB_B(y[ff, (i * en) + j + en], u[ff, s]);
                        renk_b[ff, (i * en) + j+en + 1] = RGB_B(y[ff, (i * en) + j + en + 1], u[ff, s]);
                        s++;
                    }
                }
                s = 0;
            }
           
            ResimGoster(0);

        }


        /*             
           R = Y + 1.4075 * (V - 128)
           G = Y - 0.3455 * (U - 128) - (0.7169 * (V - 128))
           B = Y + 1,7790 * (U - 128)
       */

        private int RGB_R(int y,int v)
        {
            int renk=0;
            renk = y + (int)(1.4075 * ((float)(v - 128)));
            if (renk > 255) renk = 255;
            if (renk < 0) renk = 0;
            return renk;
        }
        private int RGB_G(int y, int u, int v)
        {
            int renk = 0;
            renk = y- (int)(0.3455 * ((float)(u - 128))) - (int)(0.7169 * ((float)(v - 128)));
            if (renk > 255) renk = 255;
            if (renk < 0) renk = 0;
            return renk;
        }
        private int RGB_B(int y, int u)
        {
            int renk = 0;
            renk = y + (int)(1.7790 * ((float)(u - 128)));
            if (renk > 255) renk = 255;
            if (renk < 0) renk = 0;
            return renk;
        }
        
 
        private void ResimGoster(int frame) // Görüntü Oynatırken Resimleri Oluşturma
        {
            int deger_r, deger_g, deger_b;
            int say = 0;
            Bitmap resim = new Bitmap(en, boy);
            for (int i = 0; i < boy ; i++)
            {
                for (int j = 0; j < en ; j++)
                {

                    deger_r = y[frame, say];            
                    deger_g = y[frame, say];
                    deger_b = y[frame, say];
  
                    // Yorum Satırında Bulunan Kod Görüntüyü RGB Formatında Gösterir
                 
                    // deger_r = renk_r[frame, say];
                    // deger_g = renk_g[frame, say];
                    // deger_b = renk_b[frame, say];
                 
                    say++;
                    Color renk;
                    renk = Color.FromArgb(255,deger_r, deger_g, deger_b);
                    resim.SetPixel(j, i, renk);
                }
            }
            pictureBox1.Image = resim;
            textBoxFrame.Text = Convert.ToString(frame + ". Frame ");

        }


        private Bitmap ResimKaydet(int kaydedilecekframe)  
        {
            int deger_r, deger_g, deger_b;
            int say = 0;
            Bitmap resim = new Bitmap(en, boy);
            for (int i = 0; i < boy ; i++)
            {
                for (int j = 0; j < en ; j++)
                {
                                   
                    deger_r = renk_r[kaydedilecekframe, say];      
                    deger_g = renk_g[kaydedilecekframe, say];       
                    deger_b = renk_b[kaydedilecekframe, say];

                    // Yorum Satırında Bulunan Kod Sadece Y Bileşenlerini Kaydeder

                    // deger_r = y[kaydedilecekframe, say];            
                    // deger_g = y[kaydedilecekframe, say];
                    // deger_b = y[kaydedilecekframe, say];

                    say++;
                    Color renk;
                    renk = Color.FromArgb(255,deger_r, deger_g, deger_b);
                    resim.SetPixel(j, i, renk);
                }
            }
            return resim;
        }
        
        private void button5_Click(object sender, EventArgs e)  //Tüm Pencereleri Kaydetme Butonu
        {
            FolderBrowserDialog Klasor = new FolderBrowserDialog();
            Klasor.ShowDialog();

            string KlasorYolu;
            KlasorYolu = Klasor.SelectedPath;
            for(int kay = 0; kay < framesayisi; kay++)
            {
                Image resim = ResimKaydet(kay);
                Bitmap bm = new Bitmap(resim, en, boy);
                String kaydedilecekisim = textBox2.Text;
                bm.Save(KlasorYolu + "\\" + kaydedilecekisim+"_"+kay + ".bmp");
            }
            
        }


        int goruntuframe = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (goruntuframe < framesayisi-1)
            {
                ResimGoster(goruntuframe);
                textBoxFrame.Text = Convert.ToString(goruntuframe + ". Frame ");
                goruntuframe++;
            }
            else if(goruntuframe == framesayisi-1)   // Son frame'i kaydederken hata almamak için gereken koşul
            {
                ResimGoster(goruntuframe);
                textBoxFrame.Text = Convert.ToString(goruntuframe + ". Frame ");
            }
        }

        private void button3_Click(object sender, EventArgs e)  // Görüntü Oynat Butonu
        {
           
            if (goruntuframe == 0 || goruntuframe==framesayisi-1) // Son görüntüden sonra tekrar ilk görüntüye döner
            {
                goruntuframe = 0;
                timer1.Interval = 50;
                timer1.Enabled = true;
                timer1.Start();
            }
            else{                                               // Durdurulan yerden devam eder
                timer1.Interval = 50;
                timer1.Enabled = true;
                timer1.Start();
            }

        }

        private void button6_Click(object sender, EventArgs e)  // Ekrandaki Görüntüyü Kaydetme Butonu
        {
            FolderBrowserDialog Klasor = new FolderBrowserDialog();
            Klasor.ShowDialog();

            string KlasorYolu;
            KlasorYolu = Klasor.SelectedPath;
            
            Image resim = ResimKaydet(goruntuframe);
            Bitmap bm = new Bitmap(resim, en, boy);
            String kaydedilecekisim = textBox2.Text+"_frame_"+goruntuframe;
            bm.Save(KlasorYolu + "\\" + kaydedilecekisim + ".bmp");
            
        }

        private void button4_Click(object sender, EventArgs e)  // Görüntü Oynatmayı Durduran Buton
        {
            timer1.Enabled = false;
            timer1.Stop();
        }
    }
}
