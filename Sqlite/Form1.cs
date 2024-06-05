using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLitePCL;

namespace Sqlite
{
    public partial class Form1 : Form
    {
        private string connectionString = "Data Source=MyDatabase.sqlite;Version=3;New=True;Compress=True;";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            InitializeDatabase();
            LoadData();
        }

        private void InitializeDatabase()
        {
            string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyDatabase.sqlite");
            connectionString = $"Data Source={databasePath};Version=3;";

            if (!File.Exists(databasePath))
            {
                File.Create(databasePath).Close();
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string tableQuery = "CREATE TABLE IF NOT EXISTS People (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)";
                SQLiteCommand command = new SQLiteCommand(tableQuery, connection);
                command.ExecuteNonQuery();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO People (Name) VALUES (@name)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@name", txtName.Text);
                command.ExecuteNonQuery();
            }
            LoadData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (listBoxData.SelectedItem == null) return;

            var selectedItem = listBoxData.SelectedItem.ToString();
            var id = selectedItem.Split(':')[0];

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE People SET Name = @name WHERE Id = @id";
                SQLiteCommand command = new SQLiteCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@name", txtName.Text);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
            LoadData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBoxData.SelectedItem == null) return;

            var selectedItem = listBoxData.SelectedItem.ToString();
            var id = selectedItem.Split(':')[0];

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM People WHERE Id = @id";
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
            LoadData();
        }

        private void LoadData()
        {
            listBoxData.Items.Clear();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM People";
                SQLiteCommand command = new SQLiteCommand(selectQuery, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listBoxData.Items.Add($"{reader["Id"]}: {reader["Name"]}");
                }
            }
        }
    }
}
