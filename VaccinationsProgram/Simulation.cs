using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static VaccinationsProgram.SimulationMethods;
using static VaccinationsProgram.Program;


namespace VaccinationsProgram
{
    class Simulation
    {
        public static void RunSimulation()
        {
            int doseQuantity = 0;
            bool isRunning = true;
            while (isRunning)
            {
                Clear();
                WriteLine("***** VACCINATION SIMULATOR RUNNING *****");
                WriteLine(        "What would you like to do?\n");

                WriteLine("[1] Enter amount of vaccine doses.");
                WriteLine("[2] Start vaccination.");
                WriteLine("[3] View and Save result.");
                WriteLine("[4] Return to main menu.");

                WriteLine("\n NOTE: Testdata = 1000 random persons");

                var key = ReadKey(true);
                if (key.Key == ConsoleKey.D1 || key.Key == ConsoleKey.NumPad1)
                {
                    // Required in order to run 2 and 3
                    doseQuantity = GetDoseQuantity();
                    newDatabaseTable = $"RESULT_{doseQuantity}doses";
                }
                else if (key.Key == ConsoleKey.D2 || key.Key == ConsoleKey.NumPad2)
                {
                    // Run vaccination program
                   StartVaccination(doseQuantity);
                   
                }
                else if (key.Key == ConsoleKey.D3 || key.Key == ConsoleKey.NumPad3)
                {
                    // Shows the result with the set amount of doses. Option given to save or delete result
                    ViewVaccinatedPeople();
                }
                else if (key.Key == ConsoleKey.D4 || key.Key == ConsoleKey.NumPad4)
                {
                    //Exits to main menu
                    isRunning = false;
                }
                else 
                {
                    WriteLine("Please try again. Only 1-4 are valid inputs. Press enter to continue.");
                    PressEnterToContinue();
                } 
            }

        }

        
    }
}
