using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
namespace AdoWinFormsAsyncDZ9
{
    public partial class Form1 : Form
    {
        string connectionString;
        DataTable mainTable = new DataTable();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["defConn"].ConnectionString;
        }

        private async Task<DataTable> getDataAsync(string commandText)
        {
            SqlDataReader reader = null;
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                await connection.OpenAsync();
                reader = await command.ExecuteReaderAsync();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    table.Columns.Add(reader.GetName(i));
                }

                while (await reader.ReadAsync())
                {
                    DataRow row = table.NewRow();
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        row[i] = await reader.GetFieldValueAsync<object>(i);
                    }
                    table.Rows.Add(row);
                }
            }
            //return await Task<DataTable>.Run(() =>
            //{
                return table;
            //});
        }

        private async Task setDataAsync(string commandText)
        {
            int count;       
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(commandText, connection);
                await connection.OpenAsync();
                count = await command.ExecuteNonQueryAsync();
            }
            MessageBox.Show($"обработано строк: {count}");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = await getDataAsync("select * from Users where isDebtor=0");
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = await getDataAsync("select Authors.id,Authors.Firstname,Authors.Surname,BooksAuthors.bookID from Authors join BooksAuthors on BooksAuthors.AuthorID=Authors.id where BooksAuthors.bookID=1");
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = await getDataAsync("select Books.id,Books.Title,Books.price,Books.pages from Books join BookUsers on BookUsers.bookID=Books.id join Users on BookUsers.userID=Users.id");
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = await getDataAsync("select Books.id,Books.Title,Books.price,Books.pages from Books join BookUsers on BookUsers.bookID=Books.id where BookUsers.userID=2");
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await setDataAsync("update [Users] set isDebtor=1");
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            await setDataAsync("update [Users] set isDebtor=0");

        }
    }
}
