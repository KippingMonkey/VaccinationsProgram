using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationsProgram
{
    class Person //base class with properties shared among all persons
    {
        public string SSN { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
    class PersonFromDatabase : Person //specific properties for person in the database original table
    {
        public bool worksHealthcare { get; set; }
        public bool isHighRisk { get; set; }
        public bool hadInfektion { get; set; }
    }
    class PersonToDatabase : Person //specific properties for the persons saved in RESULT tables
    {
        public int howManyShots { get; set; }
    }
}
