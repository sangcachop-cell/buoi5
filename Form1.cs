using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace buoi5
{
    public partial class Form1 : Form
    {
        private string currentFilePath = null;
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;

            toolStripComboBox1.SelectedIndexChanged += toolStripComboBox1_SelectedIndexChanged;
            toolStripComboBox2.SelectedIndexChanged += toolStripComboBox2_SelectedIndexChanged;
            toolStripButton1.Click += toolStripButton1_Click;
            toolStripButton2.Click += toolStripButton2_Click;
            toolStripButton3.Click += toolStripButton3_Click;
            toolStripButton4.Click += toolStripButton4_Click;
            toolStripButton5.Click += toolStripButton5_Click;

            mnuNew.Click += mnuNew_Click;
            mnuOpen.Click += mnuOpen_Click;
            mnuSave.Click += mnuSave_Click;
            mnuExit.Click += mnuExit_Click;
            mnuFont.Click += mnuFont_Click;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAs(); 
            }
            else
            {
                SaveTo(currentFilePath);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            currentFilePath = null;
            SetDefaultFont();
            UpdateWordCount();
        }

        private void ToggleStyle(FontStyle styleToToggle)
        {
            Font sel = richTextBox1.SelectionFont ?? richTextBox1.Font;
            FontStyle newStyle = sel.Style ^ styleToToggle;
            richTextBox1.SelectionFont = new Font(sel.FontFamily, sel.Size, newStyle);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ToggleStyle(FontStyle.Underline);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ToggleStyle(FontStyle.Italic);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ToggleStyle(FontStyle.Bold);
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string txt = toolStripComboBox2.SelectedItem?.ToString() ?? toolStripComboBox2.Text;
            if (float.TryParse(txt, out float newSize))
            {
                Font baseFont = richTextBox1.SelectionFont ?? richTextBox1.Font;
                richTextBox1.SelectionFont = new Font(baseFont.FontFamily, newSize, baseFont.Style);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fontName = toolStripComboBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(fontName)) return;
            Font baseFont = richTextBox1.SelectionFont ?? richTextBox1.Font;
            float size = baseFont.Size;
            FontStyle style = baseFont.Style;
            richTextBox1.SelectionFont = new Font(fontName, size, style);
        }

        private void mnuFont_Click(object sender, EventArgs e)
        {
            using (FontDialog fontDlg = new FontDialog())
            {
                fontDlg.ShowColor = true;
                fontDlg.ShowEffects = true;
                fontDlg.Font = richTextBox1.SelectionFont ?? richTextBox1.Font;
                fontDlg.Color = richTextBox1.SelectionColor;

                if (fontDlg.ShowDialog() == DialogResult.OK)
                {
                    richTextBox1.SelectionFont = fontDlg.Font;
                    richTextBox1.SelectionColor = fontDlg.Color;
                }
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
                SaveAs();
            else
                SaveTo(currentFilePath);
        }
        private void SaveAs()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Rich Text Format (*.rtf)|*.rtf|Text File (*.txt)|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveTo(sfd.FileName);
                    currentFilePath = sfd.FileName;
                }
            }
        }

        private void SaveTo(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();
            if (ext == ".rtf")
                richTextBox1.SaveFile(filename, RichTextBoxStreamType.RichText);
            else
                richTextBox1.SaveFile(filename, RichTextBoxStreamType.PlainText);

            MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Rich Text Format (*.rtf)|*.rtf|Text File (*.txt)|*.txt|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string ext = Path.GetExtension(ofd.FileName).ToLower();
                    if (ext == ".rtf")
                        richTextBox1.LoadFile(ofd.FileName, RichTextBoxStreamType.RichText);
                    else
                        richTextBox1.LoadFile(ofd.FileName, RichTextBoxStreamType.PlainText);

                    currentFilePath = ofd.FileName;
                    UpdateWordCount();
                }
            }
        }

        private void PopulateFonts()
        {
            toolStripComboBox1.Items.Clear();
            InstalledFontCollection installed = new InstalledFontCollection();
            foreach (var ff in installed.Families)
            {
                toolStripComboBox1.Items.Add(ff.Name);
            }
        }

        private void PopulateSizes()
        {
            toolStripComboBox2.Items.Clear();
            int[] sizes = new int[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            foreach (int s in sizes) toolStripComboBox2.Items.Add(s.ToString());
        }

        private void SetDefaultFont()
        {
            richTextBox1.Font = new Font("Tahoma", 14);
            toolStripComboBox1.Text = "Tahoma";
            toolStripComboBox2.Text = "14";
        }
        private void UpdateWordCount()
        {
            string txt = richTextBox1.Text;
            int count = 0;
            if (!string.IsNullOrWhiteSpace(txt))
            {
                var words = Regex.Split(txt.Trim(), @"\s+");
                count = words.Count(w => !string.IsNullOrWhiteSpace(w));
            }
            toolStripStatusLabel1.Text = $"Số từ: {count}";
        }

        private void mnuNew_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            currentFilePath = null;
            SetDefaultFont();
            UpdateWordCount();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateWordCount();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateFonts();
            PopulateSizes();
            SetDefaultFont();
            UpdateWordCount();
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            Font sel = richTextBox1.SelectionFont ?? richTextBox1.Font;
            toolStripComboBox1.Text = sel.FontFamily.Name;
            toolStripComboBox2.Text = ((int)sel.Size).ToString();

            toolStripButton3.Checked = sel.Style.HasFlag(FontStyle.Bold);
            toolStripButton4.Checked = sel.Style.HasFlag(FontStyle.Italic);
            toolStripButton5.Checked = sel.Style.HasFlag(FontStyle.Underline);
        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {

        }
    }
}
