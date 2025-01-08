using System;
using System.Collections.Generic;
using System.Windows;
using System.Data.SqlClient;

namespace StudentProgressTracker
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private Student? selectedStudent;

        public MainWindow()
        {
            InitializeComponent();
            LoadStudents();
        }

        private void LoadStudents()
        {
            lstStudents.ItemsSource = dbHelper.GetStudents();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var student = new Student
            {
                Name = txtName.Text,
                Grade = txtGrade.Text,
                Subject = txtSubject.Text,
                Marks = int.Parse(txtMarks.Text),
                AttendancePercentage = double.Parse(txtAttendance.Text)
            };
            dbHelper.AddStudent(student);
            LoadStudents();
            ClearFields();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStudent != null)
            {
                selectedStudent.Name = txtName.Text;
                selectedStudent.Grade = txtGrade.Text;
                selectedStudent.Subject = txtSubject.Text;
                selectedStudent.Marks = int.Parse(txtMarks.Text);
                selectedStudent.AttendancePercentage = double.Parse(txtAttendance.Text);
                dbHelper.UpdateStudent(selectedStudent);
                LoadStudents();
                ClearFields();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStudent != null)
            {
                dbHelper.DeleteStudent(selectedStudent.Id);
                LoadStudents();
                ClearFields();
            }
        }

        private void lstStudents_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedStudent = (Student)lstStudents.SelectedItem;
            if (selectedStudent != null)
            {
                txtName.Text = selectedStudent.Name;
                txtGrade.Text = selectedStudent.Grade;
                txtSubject.Text = selectedStudent.Subject;
                txtMarks.Text = selectedStudent.Marks.ToString();
                txtAttendance.Text = selectedStudent.AttendancePercentage.ToString();
            }
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtGrade.Clear();
            txtSubject.Clear();
            txtMarks.Clear();
            txtAttendance.Clear();
            selectedStudent = null;
            lstStudents.SelectedItem = null;
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int Marks { get; set; }
        public double AttendancePercentage { get; set; }
    }

    public class DatabaseHelper
    {
        private string connectionString = "Data Source=AUMC-LAB3-04\\SQLEXPRESS;Initial Catalog=LabFinalQ1;Integrated Security=True";

        public void AddStudent(Student student)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var command = new SqlCommand("INSERT INTO Students (Name, Grade, Subject, Marks, AttendancePercentage) VALUES (@Name, @Grade, @Subject, @Marks, @AttendancePercentage)", connection);
            command.Parameters.AddWithValue("@Name", student.Name);
            command.Parameters.AddWithValue("@Grade", student.Grade);
            command.Parameters.AddWithValue("@Subject", student.Subject);
            command.Parameters.AddWithValue("@Marks", student.Marks);
            command.Parameters.AddWithValue("@AttendancePercentage", student.AttendancePercentage);
            command.ExecuteNonQuery();
        }

        public List<Student> GetStudents()
        {
            var students = new List<Student>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Students", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Grade = reader.GetString(2),
                            Subject = reader.GetString(3),
                            Marks = reader.GetInt32(4),
                            AttendancePercentage = reader.GetDouble(5)
                        });
                    }
                }
            }
            return students;
        }

        public void UpdateStudent(Student student)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Students SET Name = @Name, Grade = @Grade, Subject = @Subject, Marks = @Marks, AttendancePercentage = @AttendancePercentage WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", student.Id);
                command.Parameters.AddWithValue("@Name", student.Name);
                command.Parameters.AddWithValue("@Grade", student.Grade);
                command.Parameters.AddWithValue("@Subject", student.Subject);
                command.Parameters.AddWithValue("@Marks", student.Marks);
                command.Parameters.AddWithValue("@AttendancePercentage", student.AttendancePercentage);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteStudent(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Students WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}

