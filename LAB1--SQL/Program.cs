using System.Data.SqlClient;

namespace LAB1__SQL

{
    internal class Program
    {

        static void Main()
        {


            while (true)
            {
                Console.WriteLine("Welcome to the school's administration application!");
                Console.WriteLine();
                Console.WriteLine("Choose one of the following options:");
                Console.WriteLine("1. View all students");
                Console.WriteLine("2. View all students in a certain class");
                Console.WriteLine("3. Add new staff");
                Console.WriteLine("4. View all staff");
                Console.WriteLine("5. View all grades set in the last month");
                Console.WriteLine("6. Average grade per course");
                Console.WriteLine("7. Add new students");
                Console.WriteLine("0. Exit");

                Console.WriteLine();

                Console.Write("Choose your option by entering the number: ");
                int option = int.Parse(Console.ReadLine());

                //Switch-case for menuoptions
                switch (option) 
                {
                    case 1:
                        ViewAllStudents();
                        break;
                    case 2:
                        ViewAllStudentsInClass();
                        break;
                    case 3:
                        AddNewStaff();
                        break;
                    case 4:
                        ViewAllStaff();
                        break;
                    case 5:
                        ViewGradesLastMonth();
                        break;
                    case 6:
                        ViewAverageGradesPerCourse();
                        break;
                    case 7:
                        AddNewStudent();
                        break;
                    case 0:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Press Enter to return to the main menu.");
                Console.ReadLine();
                Console.Clear();
            }
            //method for view all students
            static void ViewAllStudents()
            {
                string connectionString = "Data Source=(localdb)\\.;Initial Catalog=Lab1SQL;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //Asking the user to choose sorting order
                    Console.Clear();
                    Console.WriteLine("Choose sorting order:");
                    Console.WriteLine("1. Sort by First Name (Ascending)");
                    Console.WriteLine("2. Sort by First Name (Descending)");
                    Console.WriteLine("3. Sort by Last Name (Ascending)");
                    Console.WriteLine("4. Sort by Last Name (Descending)");

                    Console.Write("Enter your choice: ");
                    int sortChoice = int.Parse(Console.ReadLine());

                    string sortBy = "";
                    string sortOrder = "";

                    //Switch-case for choosen sorting order
                    switch (sortChoice)
                    {
                        case 1:
                            sortBy = "FirstName";
                            sortOrder = "ASC";
                            break;
                        case 2:
                            sortBy = "FirstName";
                            sortOrder = "DESC";
                            break;
                        case 3:
                            sortBy = "LastName";
                            sortOrder = "ASC";
                            break;
                        case 4:
                            sortBy = "LastName";
                            sortOrder = "DESC";
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Defaulting to sort by First Name (Ascending).");
                            sortBy = "FirstName";
                            sortOrder = "ASC";
                            break;
                    }

                    //Query that gets all students from database and sorts it by the choice the user did
                    string studentQuery = $"SELECT * FROM Students ORDER BY {sortBy} {sortOrder}";

                    using (SqlCommand command = new SqlCommand(studentQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.Clear();
                            Console.WriteLine("List of all students:");
                            Console.WriteLine();
                            while (reader.Read())
                            {
                                //Lists all the students 
                                Console.WriteLine($"{reader["StudentID"]}, {reader["FirstName"]}, {reader["LastName"]}");
                            }
                        }
                    }
                }
            }

            //Method that views all students in a specific class
            static void ViewAllStudentsInClass()
            {
                string connectionString = "Data Source=(localdb)\\.;Initial Catalog=Lab1SQL;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Console.Clear();
                    Console.WriteLine("List of classes:");
                    string classQuery = "SELECT * FROM Classes"; //Query that gets all classes from classes

                    using (SqlCommand classCommand = new SqlCommand(classQuery, connection))
                    {
                        using (SqlDataReader classReader = classCommand.ExecuteReader())
                        {
                            while (classReader.Read())
                            {   //displays all classes by id and name
                                Console.WriteLine($"{classReader["ClassID"]}. {classReader["ClassName"]}");
                            }
                        }
                    }


                    Console.Write("Enter the ClassID to see all students in the class: ");
                    string classID = Console.ReadLine();

                    //query that gets all information about students and thier classes from database
                    string studentQuery = "SELECT FirstName, LastName, Classes.ClassName FROM Students " +
                                          "JOIN Classes ON Classes.ClassID = Students.ClassID " +
                                          "WHERE Classes.ClassID = @ClassID"; 
                                          

                    using (SqlCommand studentCommand = new SqlCommand(studentQuery, connection))
                    {
                        studentCommand.Parameters.AddWithValue("@ClassID", classID);

                        using (SqlDataReader studentReader = studentCommand.ExecuteReader())
                        {
                            Console.WriteLine();
                            //Displays all students in the specific class
                            Console.WriteLine($"List of students in Class {classID}:");
                            while (studentReader.Read())
                            {
                               
                                Console.WriteLine($"{studentReader["FirstName"]}, {studentReader["LastName"]}");
                            }
                        }
                    }
                }
            }
            //Method that adds new staff member 
            static void AddNewStaff()
            {
                Console.Clear ();
                Console.WriteLine("Enter details for the new staff member:");
                //Since you always call the teacher mister, misses or miss before their lastname in school(except schools in sweden), i asks the user to write the titele before lastname. 
                Console.Write("(please enter MS, MRS or MR before the lastname) Lastname:");
                string staffLastName = Console.ReadLine();

                Console.Write("Employment(ex Teacher, Administrator, Principal): ");
                string staffEmplyment = Console.ReadLine();

                string connectionString = "Data Source=(localdb)\\.;Initial Catalog=Lab1SQL;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //Query that adds the information about the new staff member to database
                    string insertQuery = "INSERT INTO SchoolStaff (LastName, Employment) VALUES (@LastName, @Employment)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@LastName", staffLastName);
                        command.Parameters.AddWithValue("@Employment", staffEmplyment);

                        try //try-catch statement to make sure their is no error accuring when adding info to database
                        {
                            int rowsAffected = command.ExecuteNonQuery(); //Saves the new staffmember to database

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("New staffmember added successfully!");
                            }
                            else
                            {
                                Console.WriteLine("Failed to add a new staff member. Please try again.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                }

            }

            //Method that dispalys all staff members
            static void ViewAllStaff()
            {
                string connectionString = "Data Source=(localdb)\\.;Initial Catalog=Lab1SQL;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //Asking the user to view all staff members or just the staff with a specific employment
                    Console.Clear();
                    Console.WriteLine("1. View all staff members");
                    Console.WriteLine("2. View staff members by employment");

                    Console.Write("Enter your choice: ");
                    int choice = int.Parse(Console.ReadLine());
                    Console.WriteLine( );

                    if (choice == 1)
                    {
                        ViewAllStaffWithoutFilter(connection); //if the user chooses 1, this method runs and displays all staff.
                    }
                    else if (choice == 2) 
                    {
                        Console.WriteLine("Teacher");
                        Console.WriteLine("Principal");
                        Console.WriteLine("Administrator");
                        Console.WriteLine("Janitor");
                        Console.Write("Enter employment (or leave blank to view all): ");                       
                        string selectedEmployment = Console.ReadLine();

                        ViewStaffByEmployment(connection, selectedEmployment); //if the user chooses 2 and enter the specific employment, this method runs
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice.");
                    }
                }

                static void ViewAllStaffWithoutFilter(SqlConnection connection)
                {
                    string query = "SELECT * FROM SchoolStaff"; //Query that selects all staff from schoolstaff in database

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("List of all staff members on school:");
                            while (reader.Read())
                            {
                                Console.WriteLine();
                                Console.WriteLine($"{reader["StaffID"]}, {reader["LastName"]}, {reader["Employment"]}");
                            }
                        }
                    }
                }

                static void ViewStaffByEmployment(SqlConnection connection, string selectedEmployment)
                {
                    Console.Clear();
                    //query that gets all staff members with the specific employment the user entered. 
                    string query = "SELECT * FROM SchoolStaff WHERE Employment = @Employment";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Employment", selectedEmployment);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine($"List of staff members with employment {selectedEmployment}:");
                            while (reader.Read())
                            {
                                Console.WriteLine();
                                Console.WriteLine($"{reader["StaffID"]}. {reader["LastName"]}, {reader["Employment"]}");
                            }
                        }
                    }
                }
            }

            static void ViewGradesLastMonth()
            {
                string connectionString = "Data Source=(localdb)\\.;Initial Catalog=Lab1SQL;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DateTime lastMonth = DateTime.Now.AddMonths(-1);

                    string query = $"SELECT Students.FirstName + Students.LastName AS StudentName, Courses.CourseName, Grades.Grade " +
                                   $"FROM Grades " +
                                   $"INNER JOIN Students ON Grades.StudentID = Students.StudentID " +
                                   $"INNER JOIN Courses ON Grades.CourseID = Courses.CourseID " +
                                   $"WHERE Grades.GradeDate >= @LastMonth";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LastMonth", lastMonth);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.Clear();
                            Console.WriteLine($"List of grades assigned to students in the last month:");
                            Console.WriteLine();
                            Console.WriteLine("Student Name | Course Name | Grade");
                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["StudentName"]}, {reader["CourseName"]}, {reader["Grade"]}");
                            }
                        }
                    }
                }
            }
            //method that displays average, highest and lowest grade per course. 
            static void ViewAverageGradesPerCourse()
            {
                Console.Clear();
                string connectionString = "Data Source=(localdb)\\.;Initial Catalog=Lab1SQL;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Query to get the average, highest, and lowest grades per course from database
                    string query = "SELECT " +
                                   "Courses.CourseID, " +
                                   "Courses.CourseName, " +
                                   "AVG(CAST(Grades.Grade AS FLOAT)) AS AverageGrade, " +
                                   "MAX(Grades.Grade) AS HighestGrade, " +
                                   "MIN(Grades.Grade) AS LowestGrade " +
                                   "FROM Courses " +
                                   "LEFT JOIN Grades ON Courses.CourseID = Grades.CourseID " +   //The LEFT JOIN ensures that all rows from the "Courses" table are included in the result set, even if there are no corresponding matches in the "Grades" table.
                                   "GROUP BY Courses.CourseID, Courses.CourseName";  // If there are no grades for one of the courses, the columns from the "Grades" table will contain NULL values.

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("CourseID | CourseName | AverageGrade | HighestGrade | LowestGrade");
                            while (reader.Read())
                            {
                                //displays the information about the grades 
                                Console.WriteLine();
                                Console.WriteLine($"{reader["CourseID"]} | {reader["CourseName"]} | {reader["AverageGrade"]} | {reader["HighestGrade"]} | {reader["LowestGrade"]}");
                            }
                        }
                    }
                }
            }

            //Method that adds a new student 
            static void AddNewStudent()
            {
                Console.Clear(); //asking the user to enter all the asking details about the student
                Console.WriteLine("Please enter following details to add new student: ");

                Console.Write("Firstname: ");
                string studentFirstName = Console.ReadLine();

                Console.Write("Lastname: ");
                string studentLastName= Console.ReadLine();

                Console.Write("Age: ");
                int studentAge = int.Parse(Console.ReadLine());

                Console.Write("class ID: ");
                int classID = int.Parse(Console.ReadLine());

                string connectionString = "Data Source=(localdb)\\.;Initial Catalog=Lab1SQL;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //Query to insert the information in the database
                    string insertQuery = "INSERT INTO Students (FirstName, LastName, Age, ClassID) " +
                                         "VALUES (@FirstName, @LastName, @Age, @ClassID)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        //adding the parameters with the values/details to database
                        command.Parameters.AddWithValue("@FirstName", studentFirstName);
                        command.Parameters.AddWithValue("@LastName", studentLastName);
                        command.Parameters.AddWithValue("@Age", studentAge);                      
                        command.Parameters.AddWithValue("@ClassID", classID);

                        try //try-catch statement to make sure no errors accur when saving info to databse.
                        {
                            int rowsAffected = command.ExecuteNonQuery(); //saves the information to database

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("New student added successfully!");
                            }
                            else
                            {
                                Console.WriteLine("Failed to add a new student. Please try again.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}