using System.Text;

namespace FlatOut2LocalizationEditor
{
    public partial class MainTable : Form
    {
        public MainTable()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
                return;

            dataGridView1.Rows.Clear();

            string filename = openFileDialog1.FileName;

            byte[] data = File.ReadAllBytes(filename);

            int count = BitConverter.ToInt32(data, 0);

            for (int i = 0; i < count; i++)
            {
                var pos = BitConverter.ToInt32(data, (i * 2 * 4) + 4);
                var len = BitConverter.ToInt32(data, (i * 2 * 4) + 8);
                string text = Encoding.Unicode.GetString(data, pos, len * 2);
                dataGridView1.Rows.Add(i, text);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) == DialogResult.Cancel)
                return;

            string filename = saveFileDialog1.FileName;

            List<byte> data = [];

            data.AddRange(BitConverter.GetBytes(dataGridView1.Rows.Count));

            List<byte> offsets = [];
            List<byte> strings = [];

            int offset = (dataGridView1.Rows.Count * 4 * 2) + 4;

            int len = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                byte[] str = [.. Encoding.Unicode.GetBytes(row.Cells[1].Value.ToString() ?? " "),.. BitConverter.GetBytes((short)0)];
                offsets.AddRange([.. BitConverter.GetBytes(len + offset), .. BitConverter.GetBytes(str.Length/2 - 1)]);
                strings.AddRange(str);
                len += str.Length;
            }

            data.AddRange([..offsets,.. strings]);

            File.WriteAllBytes(filename, [.. data]);
        }
    }
}
