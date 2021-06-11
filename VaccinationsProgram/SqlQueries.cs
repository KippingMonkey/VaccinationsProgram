using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static VaccinationsProgram.SimulationMethods;
using static VaccinationsProgram.Program;

namespace VaccinationsProgram
{
    class SqlQueries
    {
       //static variables used in all sql query related methods
        public static SqlConnection connection;
        public static SqlCommand command;
        public static SqlDataReader reader = null;
        public static int phase1Quantity;
        public static int phase2Quantity;
        public static int phase3Quantity;
        public static int phase4Quantity;
        public static int under18Quantity;

        /// <summary>
        /// Shows the number of persons in each category. Data collected from the database.
        /// </summary>
        public static void GetTestPopulation()
        {
            try
            {
                connection.Open();

                //Everyone that works healthcare and is over 18 years old
                string phase1Total = @"SELECT COUNT(tp.worksHealthcare) AS WorksHealthCare 
                                   FROM dbo.TEST_POPULATION AS tp
                                   WHERE tp.worksHealthcare = 'true' AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) >= 18";
                //Everyone 65 and over that does not work healtcare
                string phase2Total = @"SELECT COUNT(tp.SSN) AS Above65
                                   FROM dbo.TEST_POPULATION AS tp
                                   WHERE tp.worksHealthcare = 'false'  AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) >= 65";
                //Everyone with high risk between 18 and 65
                string phase3Total = @"SELECT COUNT(tp.SSN) AS OVer18
                                   FROM dbo.TEST_POPULATION AS tp
                                   WHERE tp.isHighRisk = 'true'  AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) < 65 AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) >= 18
                                   AND tp.worksHealthcare = 'false'";
                //Everyone else between 18 and 65
                string phase4Total = @"SELECT COUNT(tp.SSN) AS OVer18
                                   FROM dbo.TEST_POPULATION AS tp
                                   WHERE tp.isHighRisk = 'false'  AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) < 65 AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) >= 18
                                   AND tp.worksHealthcare = 'false'";
                //Everyone below 18
                string under18Total = @"SELECT COUNT(tp.SSN) AS Under18
                                    FROM dbo.TEST_POPULATION AS tp
                                    WHERE Floor(DateDiff(d, SSN, GetDate()) / 365.25) < 18";

                //result from above saved in variables below
                command = new SqlCommand(phase1Total, connection);
                phase1Quantity = (int)command.ExecuteScalar();

                command = new SqlCommand(phase2Total, connection);
                phase2Quantity = (int)command.ExecuteScalar();

                command = new SqlCommand(phase3Total, connection);
                phase3Quantity = (int)command.ExecuteScalar();

                command = new SqlCommand(phase4Total, connection);
                phase4Quantity = (int)command.ExecuteScalar();

                command = new SqlCommand(under18Total, connection);
                under18Quantity = (int)command.ExecuteScalar();
            }
            finally
            {
                {
                    //Close the connection to the database
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
            
        }

        /// <summary>
        /// Collects everyone in phase 1 from the database in an list ordered by age
        /// </summary>
        /// <returns></returns>
        public static List<PersonFromDatabase>GetPhase1People()
        {
            var phase1People = new List<PersonFromDatabase>();
            try
            {
                connection.Open();
                // Everyone that works healthcare and is over 18 years old ordered by age
                // uses a UDF to deliver a "personnummer" with four random last digits
                // NEWID generates a radom value for each row, CHECKSUM turns that into a number, ABS return an absolute number and the number determines the range
                // Source: SQLundercover.com
               
                string phase1Ordered = @"SELECT dbo.fn_FormatSSN(SSN, ABS(CHECKSUM(NEWID()))%9000 + 999 +1), last_name, first_name, hadInfection, worksHealthcare, isHighRisk
                                   FROM dbo.TEST_POPULATION AS tp
                                   WHERE tp.worksHealthcare = 'true' AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) >= 18
                                   ORDER BY SSN";

                command = new SqlCommand(phase1Ordered, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    phase1People.Add(new PersonFromDatabase
                    {
                        SSN = reader[0].ToString(),
                        LastName = reader[1].ToString(),
                        FirstName = reader[2].ToString(),
                        hadInfektion = (bool)reader[3],
                        worksHealthcare = (bool)reader[4],
                        isHighRisk = (bool)reader[5]
                    });
                }
            }
            catch (Exception e)
            {
                WriteLine($"Oops, something went wrong {e}.");
            }
            finally
            {
                {
                    // close the reader
                    if (reader != null)
                    {
                        reader.Close();
                    }

                    //Close the connection to the database
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
            return phase1People;

        }

        /// <summary>
        /// Collects everyone in phase 2 from the database in an list ordered by age
        /// </summary>
        /// <returns></returns>
        public static List<PersonFromDatabase> GetPhase2People()
        {
            var phase2People = new List<PersonFromDatabase>();
            try
            {
                connection.Open();
                //Everyone that  is over 65 and does not work healthcare ordered by age
                string phase1Ordered = @"SELECT dbo.fn_FormatSSN(SSN, ABS(CHECKSUM(NEWID()))%9000 + 999 +1), last_name, first_name, hadInfection, worksHealthcare, isHighRisk
                                   FROM dbo.TEST_POPULATION AS tp
                                   WHERE tp.worksHealthcare = 'false'  AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) >= 65
                                   ORDER BY SSN";

                command = new SqlCommand(phase1Ordered, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    phase2People.Add(new PersonFromDatabase
                    {
                        SSN = reader[0].ToString(),
                        LastName = reader[1].ToString(),
                        FirstName = reader[2].ToString(),
                        hadInfektion = (bool)reader[3],
                        worksHealthcare = (bool)reader[4],
                        isHighRisk = (bool)reader[5]
                    });
                }
            }
            catch (Exception e)
            {
                WriteLine($"Oops, something went wrong {e}.");
            }
            finally
            {
                {
                    // close the reader
                    if (reader != null)
                    {
                        reader.Close();
                    }

                    //Close the connection to the database
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
            return phase2People;

        }

        /// <summary>
        /// Collects everyone in phase 3 from the database in an list ordered by age
        /// </summary>
        /// <returns></returns>
        public static List<PersonFromDatabase> GetPhase3People()
        {
            var phase3People = new List<PersonFromDatabase>();
            try
            {
                connection.Open();
                //Everyone that  is under 65, over 18, does not work healthcare and is in the high risk group. ordered by age
                string phase1Ordered = @"SELECT dbo.fn_FormatSSN(SSN, ABS(CHECKSUM(NEWID()))%9000 + 999 +1), last_name, first_name, hadInfection, worksHealthcare, isHighRisk
                                         FROM dbo.TEST_POPULATION AS tp
                                         WHERE tp.isHighRisk = 'true'  AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) < 65 AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) >= 18
                                         AND tp.worksHealthcare = 'false'
                                         ORDER BY SSN";

                command = new SqlCommand(phase1Ordered, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    phase3People.Add(new PersonFromDatabase
                    {
                        SSN = reader[0].ToString(),
                        LastName = reader[1].ToString(),
                        FirstName = reader[2].ToString(),
                        hadInfektion = (bool)reader[3],
                        worksHealthcare = (bool)reader[4],
                        isHighRisk = (bool)reader[5]
                    });
                }
            }
            catch (Exception e)
            {
                WriteLine($"Oops, something went wrong {e}.");
            }
            finally
            {
                {
                    // close the reader
                    if (reader != null)
                    {
                        reader.Close();
                    }

                    //Close the connection to the database
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
            return phase3People;

        }

        /// <summary>
        /// Collects everyone in phase 4 from the database in an list ordered by age
        /// </summary>
        /// <returns></returns>
        public static List<PersonFromDatabase> GetPhase4People()
        {
            var phase4People = new List<PersonFromDatabase>();
            try
            {
                connection.Open();
                //Everyone that is left between the age of 18 and 65
                string phase1Ordered = @"SELECT dbo.fn_FormatSSN(SSN, ABS(CHECKSUM(NEWID()))%9000 + 999 +1), last_name, first_name, hadInfection, worksHealthcare, isHighRisk
                                         FROM dbo.TEST_POPULATION AS tp
                                         WHERE tp.isHighRisk = 'false'  AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) < 65 AND Floor(DateDiff(d, SSN, GetDate()) / 365.25) >= 18
                                         AND tp.worksHealthcare = 'false'
                                         ORDER BY SSN";

                command = new SqlCommand(phase1Ordered, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    phase4People.Add(new PersonFromDatabase
                    {
                        SSN = reader[0].ToString(),
                        LastName = reader[1].ToString(),
                        FirstName = reader[2].ToString(),
                        hadInfektion = (bool)reader[3],
                        worksHealthcare = (bool)reader[4],
                        isHighRisk = (bool)reader[5]
                    });
                }
            }
            catch (Exception e)
            {
                WriteLine($"Oops, something went wrong {e}.");
            }
            finally
            {
                {
                    // close the reader
                    if (reader != null)
                    {
                        reader.Close();
                    }

                    //Close the connection to the database
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
            return phase4People;

        }

        /// <summary>
        /// Searches the database for Result-Tables
        /// </summary>
        public static void SearchDatabaseResults()
        {
            Clear();
            WriteLine("Searching for previous test results.");
            bool fileExists = true;

            try
            {
                //Open the connection to the database
                connection.Open();

                //the query in a string
                string query = @"IF EXISTS(SELECT TABLE_NAME
                                           FROM INFORMATION_SCHEMA.TABLES
                                           WHERE TABLE_NAME LIKE '%RESULT%')           
                                           SELECT 'Files found' AS Search_result 
                                           ELSE SELECT 'There are no saved results in the database' AS search_result";

                //Pass the connection to a command object (the query string)
                command = new SqlCommand(query, connection);

                // get query result and print it
                string result = (string)command.ExecuteScalar();
                WriteLine(result);
            }
            finally
            {
                //Close the connection to the database
                if (connection != null)
                {
                    connection.Close();
                }
            }
            WriteLine("End of search.");
            PressEnterToContinue();

            //if files are found move to the next step
            if (fileExists) { LoadDatabaseResults(); }
        }

       /// <summary>
       /// Lists all Result-tables (if they exist)
       /// </summary>
       public static void LoadDatabaseResults()

        {
            Clear();
            
            int choice = 0;
            var dataTables = new List<string>();

            WriteLine("\nTo return to the main menu, press enter.");

            try
            {
                connection.Open();
                reader = null;

                int counter = 1;

                //get a list of RESULT tables
                string query = @"SELECT TABLE_NAME
                             FROM INFORMATION_SCHEMA.TABLES
                             WHERE TABLE_NAME LIKE '%RESULT%'";


                command = new SqlCommand(query, connection);

                reader = command.ExecuteReader();

                //save the names of the tables to a list
                while (reader.Read())
                {
                    WriteLine($"[{counter}] {reader[0]} ");
                    dataTables.Add(reader[0].ToString());
                    counter++;
                }
            }
            finally
            {
                //Close the connection to the database
                if (connection != null)
                {
                    connection.Close();
                }
            }

            //loop for entering file number. Enter exits to main menu
            while (true)
            {
                Write("Which result would you like to view? Enter filenumber: ");
                string input = ReadLine();
                
                if (input == "")
                {
                    return;
                }
                else 
                {
                    bool isInt = int.TryParse(input, out choice);
                    choice--;
                    if (isInt)
                    {
                        if (choice <= dataTables.Count())
                        {
                            //if a valid choice is entered, move to the next step
                            ShowDatabaseResult(dataTables[choice]);
                        }
                    }
                    else { WriteLine("Only integers are valid for input. Please try again."); }
                }
            }
        }

       /// <summary>
       /// Loads chosen table to a List and prints the result
       /// </summary>
       /// <param name="tableName"></param>
       public static void ShowDatabaseResult(string tableName)
        {
            var result = new List<PersonToDatabase>();
            try
            {
                connection.Open();
                // Everyone that works healthcare and is over 18 years old ordered by age

                string query = @$"SELECT SSN, LastName, FirstName, howManyShots
                                   FROM dbo.{tableName}
                                   ORDER BY SSN";

                command = new SqlCommand(query, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new PersonToDatabase
                    {
                        SSN = reader[0].ToString(),
                        LastName = reader[1].ToString(),
                        FirstName = reader[2].ToString(),
                        howManyShots = (int)reader[3]
                    });
                }
            }
            catch (Exception e)
            {
                WriteLine($"Oops, something went wrong {e}.");
            }
            finally
            {
                {
                    // close the reader
                    if (reader != null)
                    {
                        reader.Close();
                    }

                    //Close the connection to the database
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }
            }
            PrintPersonToDatabaseList(result);
            WriteLine("\nYou can see the result above. Scroll if neccessary.");

            PressEnterToContinue();
            return;

        }

        /// <summary>
        /// Creates a new table in the database where the result will be saved 
        /// </summary>
        public static void CreateNewTableInDatabase()
        {
            try
            {
                connection.Open();
                
                reader = null;

                //Create a table for the result
                string query = @$"CREATE TABLE {newDatabaseTable} (
	                             id INT IDENTITY(1,1) PRIMARY KEY,
	                             SSN VARCHAR(50),
	                             last_name VARCHAR(50),
	                             first_name VARCHAR(50),
	                             howManyShots INT
                                 )";

                command = new SqlCommand(query, connection);
                

                command.ExecuteNonQuery();
            }
            finally
            {
                //Close the connection to the database
                if (connection != null)
                {
                    connection.Close();
                }
            }

        }

      /// <summary>
      /// Saves list of vaccinated people to the new table in the database.
      /// </summary>
      /// <param name="result"></param>
      public static void SaveResultToNewTable(List<PersonToDatabase> result)
        {
            try
            {
                connection.Open();

                reader = null;

                string query = "";

                //Insert all persons from sortedVaccinatedPeople in the new table.
                foreach (PersonToDatabase p in sortedVaccinatedPeople)
                {
                    query = @$"INSERT INTO {newDatabaseTable} 
                                (SSN, last_name, first_name, howManyShots)
	                             VALUES ( '{p.SSN}', '{p.LastName}', '{p.FirstName}', '{p.howManyShots}')";

                    command = new SqlCommand(query, connection);

                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                //Close the connection to the database
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
    }
}
