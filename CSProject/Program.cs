using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSProject
{
    public class Staff
    {
        private float hourlyRate;
        private int hWorked;

        public float TotalPay { protected set; get; }
        public float BasicPay { private set; get; }
        public string NameOfStaff { private set; get; }

        public int HoursWorked
        {
            get { return hWorked; }

            set
            {
                if (value > 0)
                    hWorked = value;
                else
                    hWorked = 0;
            }
        }

        public Staff(string name, float rate)
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating pay ...");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;

        }

        public override string ToString()
        {
            return "Name of the staff is: " + NameOfStaff + " and their Basic Pay is: " + BasicPay;

        }

    }

    public class Manager : Staff
    {
        private const float managerHourlyRate = 50f;

        public int Allowance { private set; get; }
        public Manager(string name):base(name, managerHourlyRate)
        {

        }
        public override void CalculatePay()
        {
            base.CalculatePay();
            Allowance = 1000;
            if (HoursWorked > 160)
            {
                TotalPay += Allowance;
            }
        }

        public override string ToString()
        {
            return "Manager Name: " + NameOfStaff + " and the total salary is: " + TotalPay ;
        }

    }

    class Admin : Staff
    {
        private const float overTimeRate=15.5f , adminHourlyRate=30f;

        public float OverTime
        {
            get;
            private set;
        }

        public Admin(string name) : base(name, adminHourlyRate)
        {

        }

        public override void CalculatePay()
        {
            base.CalculatePay();

            if (HoursWorked > 160)
            {
                OverTime = overTimeRate * (HoursWorked - 160);
                TotalPay += OverTime;
            }

        }

        public override string ToString()
        {
            return "Admin name: " + NameOfStaff + " and the salary is: " + TotalPay;
        }

    }

    class FileReader
    {
        public List<Staff> ReadFile()
        {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.txt";
            string[] separator = { ", " };

            if (File.Exists(path))
            {
                using(StreamReader sr = File.OpenText(path))
                {
                    while(sr.Peek() >= 0) // Check if needs to be changed.
                    {
                        Staff obj;
                        string str;
                        str = sr.ReadLine();
                        result = str.Split(separator, StringSplitOptions.RemoveEmptyEntries); // Check if remove empty or None 
                        if (result[1] == "Manager")
                        {
                            obj = new Manager(result[0]);
                        }
                        else
                        {
                            obj = new Admin(result[0]);
                        }
                        myStaff.Add(obj);

                    }

                    sr.Close();
                }

            }
            else
            {
                Console.WriteLine("File does not exist in the current directory!");
            }

            return myStaff;
        }
    }

    class PaySlip
    {
        private int month, year;

        enum MonthOfYear { JAN = 1, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC}

        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        public void GeneratePaySlip(List<Staff> myStaff)
        {
            foreach(Staff f in myStaff)
            {
                string path = f.NameOfStaff + ".txt";
                using(StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("PAY SLIP FOR {0} {1}", (MonthOfYear)month, year);
                    sw.WriteLine("===============================");
                    sw.WriteLine("Name of Staff: {0}", f.NameOfStaff);
                    sw.WriteLine("Hours Worked:{0}\n", f.HoursWorked);
                    sw.WriteLine("Basic Pay:{0:C}", f.BasicPay);
                    if(f.GetType() == typeof(Manager))
                    {
                        sw.WriteLine("Allowance: {0:C}\n", ((Manager)f).Allowance);
                    }
                    else
                    {
                        sw.WriteLine("Overtime: {0:C}\n", ((Admin)f).OverTime);
                    }
                    sw.WriteLine("===============================");
                    sw.WriteLine("Total pay: {0:C}", f.TotalPay);
                    sw.WriteLine("===============================");
                    sw.Close();
                }
            }
        }
        public void GenerateSummary(List<Staff> mystaff)
        {
            var result = from f in mystaff
                         where f.HoursWorked < 10
                         orderby f.NameOfStaff ascending
                         select new { f.NameOfStaff, f.HoursWorked };
            string path = "summary.txt";
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Staff with less than 10 hours of work\n");
                foreach (var f in result)
                    sw.WriteLine("Name of Staff: {0}, Hours Worked:{1}", f.NameOfStaff, f.HoursWorked);

                sw.Close();
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0, year = 0;
            while(year == 0)
            {
                Console.WriteLine("\nPlease enter the year: ");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message + " Please try again,");
                }
            }

            while(month == 0)
            {
                Console.WriteLine("\nPlease enter the month: ");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());
                    if(month >12 || month < 1)
                    {
                        Console.WriteLine("Invalid Month, please try again");
                        month = 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " Please try again,");

                }
            }
            myStaff = fr.ReadFile();

            for(int i =0; i< myStaff.Count; i++)
            {
                try
                {
                    Console.Write("\nEnter hours worked for {0}: ", myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    i--;
                }
            }
            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);

            Console.ReadLine();

        }
    }
}
