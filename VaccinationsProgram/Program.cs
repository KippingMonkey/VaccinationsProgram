using System;
using System.Data.SqlClient;
using static System.Console;
using static VaccinationsProgram.SqlQueries;
using static VaccinationsProgram.Simulation;
using static VaccinationsProgram.SimulationMethods;

/*This program is a school assigment that was made during the summer of 2021 as an extra vaccation challenge. It is a vaccination
 simulator that lets the user try how many of the population can be vaccinated with a given number och vaccinationshots. 
It does not hold any real world value and was mostly made as an opportunity to practice some problem solving and to practice
code using connection to an SQL datbase, working with classes and organizing my code into different files.+

NOTE: All methods should be commented with XML so to get a description of a method, just hover over it with the curson.*/

namespace VaccinationsProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            //Instantiate the connection to the database
            // NOTE: Change source and make sure you have created the the database if you want to try and run it.
            connection = new SqlConnection("Data Source=laptop-9gj2bhv1;Initial Catalog=VaccinationProg;Integrated Security=SSPI");
            #region MAIN MENU
            bool isRunning = true;
            while (isRunning)
            {
                GetTestPopulation();
                Clear();
                WriteLine("***** VACCINATION SIMULATOR *****");
                WriteLine("   What would you like to do?\n");
           
                WriteLine("[1] View testpopulation.");
                WriteLine("[2] Simulate vaccination.");
                WriteLine("[3] Load saved results.");
                WriteLine("[4] Quit.");

                var key = ReadKey(true); //Using key choice to instantly go to the chosen destination
                if (key.Key == ConsoleKey.D1 || key.Key == ConsoleKey.NumPad1)
                {
                    ShowTestPopulation();
                }
                else if (key.Key == ConsoleKey.D2 || key.Key == ConsoleKey.NumPad2)
                {
                    // Move to simulation part of program
                    RunSimulation();
                }
                else if (key.Key == ConsoleKey.D3 || key.Key == ConsoleKey.NumPad3)
                {
                    // Move to seach and load part of program
                    SearchDatabaseResults();
                    
                }
                else if (key.Key == ConsoleKey.D4 || key.Key == ConsoleKey.NumPad4)
                {
                   //Exits program
                    Environment.Exit(0);
                }
                else 
                { 
                    WriteLine("Please try again. Only 1-4 are valid inputs.");
                    PressEnterToContinue();
                }

            }
            #endregion

        }
        /// <summary>
        /// Minifunction to spare some repetetive typing
        /// 
        /// </summary>
        public static void PressEnterToContinue()
        {
            //Yes, this really press ANY key to continue but it will allow users to proceed even if they miss the enter key :)
            WriteLine("Press ENTER to continue.");
            ReadKey();
        }
    }
}
