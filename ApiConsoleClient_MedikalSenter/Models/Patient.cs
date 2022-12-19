using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace ApiConsoleClient_MedikalSenter.Models
{
    public class Patient
    {
        //pasted from api file, un-decorated or un-annotated..
        //we dont have database works here
        public int ID { get; set; }

        public string FullName
        {
            get
            {
                return FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? " " :
                    (" " + (char?)MiddleName[0] + ". ").ToUpper())
                    + LastName;
            }
        }
        public string OHIP { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; }
        public byte ExpYrVisits { get; set; }
        public Byte[] RowVersion { get; set; }
        public int DoctorID { get; set; }
        public Doctor Doctor { get; set; }
    }
}
