using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;
using SoftUniContext = SoftUni.Data.SoftUniContext;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext dbContext = new SoftUniContext();
            string result = AddNewAddressToEmployee(dbContext);
            Console.WriteLine(result);
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.OrderBy(e => e.EmployeeId).ToArray();
            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        { 
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new {e.FirstName, e.LastName, DepartmentName = e.Department, e.Salary })
                .OrderBy(e => e.Salary).ThenByDescending(e => e.FirstName)
                .ToArray();

            StringBuilder result = new StringBuilder();

            foreach (var employee in employees)
            {
                result.AppendLine($"{employee.FirstName} {employee.LastName} from Research and Development - ${employee.Salary:F2}");
            }

            return result.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            Employee? employee = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");
            employee.Address = newAddress;

            context.SaveChanges();

            var employeeAddresses = context.Employees.OrderBy(e => e.AddressId).Take(10).Select(e => e.Address!.AddressText).ToArray();
            return String.Join(Environment.NewLine, employeeAddresses);
        }

    }
}
