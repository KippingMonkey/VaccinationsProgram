using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static VaccinationsProgram.SqlQueries;
using static VaccinationsProgram.Program;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("VaccinationProgramTests")]
//https://stackoverflow.com/questions/15440935/how-to-test-internal-class-library
//Even though this is an internal class the above property allows a specific project or file outside access outside that assembly

namespace VaccinationsProgram
{
    class SimulationMethods
    {
        // static variables that needs to be accessed from other classes
        public static List<PersonToDatabase> vaccinatedPeople = new List<PersonToDatabase>();
        public static List<PersonToDatabase> sortedVaccinatedPeople = new List<PersonToDatabase>();
        public static string newDatabaseTable = "";

       /// <summary>
       /// Shows a summary of the testpopulation and each category
       /// </summary>
       public static void ShowTestPopulation()
        {
            //controll to check all persons are accounted for
            int sumTestPersons = (phase1Quantity + phase2Quantity + phase3Quantity + phase4Quantity + under18Quantity);

            Clear();
            WriteLine($"Testpopulation consists of a {sumTestPersons} random persons divided as shown below.");
            WriteLine($"Phase 1: {phase1Quantity} persons over 18 working within the healthcare system.  \n" +
                      $"Phase 2: {phase2Quantity} persons above the age of 65 not working in healthcare: \n" +
                      $"Phase 3: {phase3Quantity} persons between 18 and 65 within the \"High Risk\" category: \n" +
                      $"Phase 4: {phase4Quantity} remaining persons over the age of 18: \n");
            WriteLine($"There are {under18Quantity} persons under the age of 18 and they will not be vaccinated");
            WriteLine("Press enter to return to the main menu.");

            PressEnterToContinue();
        }

        /// <summary>
        /// Lets the user input a quantity of vaccination doses
        /// </summary>
        /// <returns></returns>
        public static int GetDoseQuantity()
        {
            Clear();
            while (true)
            {
                int quantity;

                Write("Please enter the desired amount of vaccine doses: ");
                bool isInt = int.TryParse(ReadLine(), out quantity);

                if (quantity <= 0)
                {
                    WriteLine("With no vaccine, no one gets vaccinated. Please enter a higher number.");

                }
                else if (quantity > 0)
                {
                    WriteLine($"You have set the quantity to {quantity} doses.");
                    PressEnterToContinue();
                    return quantity;
                }

                else { WriteLine("Only integers are valid for input. Please try again."); }
            }

        }

       /// <summary>
       /// Works through all phases and "vaccinates" utill either all doses are used or everyone is vaccinated.
       /// </summary>
       /// <param name="doseQuantity"></param>
       public static void StartVaccination(int doseQuantity)
        {
            int doseQuantityStart = doseQuantity;
            if (doseQuantity <= 0)
            {
                WriteLine("You must enter a valid amount of vaccination doses before you can start.");
                ReadKey();
                return;
            }

            var phase1People = GetPhase1People();
            doseQuantity = Vaccinate(phase1People, doseQuantity);

            if (doseQuantity > 0)
            {
                var phase2People = GetPhase2People();
                doseQuantity = Vaccinate(phase2People, doseQuantity);
            }
            if (doseQuantity > 0)
            {
                var phase3People = GetPhase3People();
                doseQuantity = Vaccinate(phase3People, doseQuantity);
            }
            if (doseQuantity > 0)
            {
                var phase4People = GetPhase4People();
                doseQuantity = Vaccinate(phase4People, doseQuantity);
            }

            int allPhasesTotal = phase1Quantity + phase2Quantity + phase3Quantity + phase4Quantity;
            int peopleVaccinated = vaccinatedPeople.Count();
            double percentOfPopulation = Math.Round((((double)peopleVaccinated / (double)allPhasesTotal) * 100),2);
            
            WriteLine($"{doseQuantityStart} doses was enough to vaccinate {percentOfPopulation}% of the population ({peopleVaccinated} people out of {allPhasesTotal}).");

            PressEnterToContinue();
        }

        /// <summary>
        /// Decrements doseQuantity based on phase population and adds "vaccinated" people to a list.
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="doseQuantity"></param>
        /// <returns></returns>
        public static int Vaccinate(List<PersonFromDatabase> phase, int doseQuantity)
        {
            foreach (PersonFromDatabase p in phase)
            {
                if (doseQuantity == 0)
                {
                    break;
                }
                else if (p.hadInfektion) //person only needs 1 shot
                {
                    doseQuantity--;
                    vaccinatedPeople.Add(new PersonToDatabase
                    {
                        SSN = p.SSN,
                        LastName = p.LastName,
                        FirstName = p.FirstName,
                        howManyShots = 1
                    });
                }
                else if (!p.hadInfektion) //person needs two shots
                {
                    doseQuantity -= 2;
                    vaccinatedPeople.Add(new PersonToDatabase
                    {
                        SSN = p.SSN,
                        LastName = p.LastName,
                        FirstName = p.FirstName,
                        howManyShots = 2
                    });
                }
            }
            return doseQuantity;
        }

       /// <summary>
       /// If prompted prints a list of all vaccinated persons with an option to save or delete the result
       /// </summary>
       public static void ViewVaccinatedPeople()
        {
            Clear();
            WriteLine("Press ENTER to view result. Press ESC to return to menu.");

            sortedVaccinatedPeople = vaccinatedPeople.OrderBy(p => p.SSN).ToList();

            var key = ReadKey(true); //here the user actually have to press enter or esc to proceed
            if (key.Key == ConsoleKey.Enter)
            {
                if (vaccinatedPeople.Count == 0)
                {
                    WriteLine("There is no result to show. Please run a simulation first.");
                    ReadKey();
                }
                PrintPersonToDatabaseList(sortedVaccinatedPeople);

                while (true)
                {
                    WriteLine("\n Do you wish to save this result? [1]=Yes, [2]=No");
                    var key2 = ReadKey(true);

                    if (key2.Key == ConsoleKey.D1 || key2.Key == ConsoleKey.NumPad1)
                    {
                        WriteLine("Saving result");
                        CreateNewTableInDatabase();
                        SaveResultToNewTable(sortedVaccinatedPeople);
                        WriteLine("Data saved successfullly!");
                        PressEnterToContinue();
                    }
                    else if (key2.Key == ConsoleKey.D2 || key2.Key == ConsoleKey.NumPad2)
                    {
                        WriteLine("Deleting result"); // the result is not really deleted but it could be done by clearing the lists with the result.
                        PressEnterToContinue();
                        return;
                    }
                    else { WriteLine("Press key 1 for yes or key 2 for no. Please try again"); PressEnterToContinue(); } 
                }
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                return;
            }
        }

       /// <summary>
       /// Prints a formated display of given list
       /// </summary>
       /// <param name="listToPrint"></param>
       public static void PrintPersonToDatabaseList(List<PersonToDatabase> listToPrint)
        {
            foreach (PersonToDatabase p in listToPrint) //prints a list of all persons that got vaccinated
            {
                WriteLine(@"{0, -15}{1, -15}{2, -15}{3, -5}"
                , p.SSN,
                 p.LastName,
                 p.FirstName,
                 p.howManyShots);
            }
        }
    }

}
