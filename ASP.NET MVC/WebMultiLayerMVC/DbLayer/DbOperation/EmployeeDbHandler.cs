using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbLayer.DbOperation;
using System.Data.Entity;

namespace DbLayer.DbOperation
{
    public class EmployeeDbHandler
    {
        #region Employee Controllers
        public int AddEmployee(Employee employee)
        {
            using (var connection = new EmployeeDBEntities())
            {
                EMPLOYEE emp = new EMPLOYEE()
                {
                    EMP_NAME = employee.EMP_NAME,
                    EMP_EMAIL = employee.EMP_EMAIL,
                    EMP_ADDRESS_ID = employee.EMP_ADDRESS_ID,
                    EMP_CODE = employee.EMP_CODE
                };
                //address code
                if (employee.Address != null)
                {
                    emp.ADDRESS = new ADDRESS()
                    {
                        ADD_DETIALS = employee.Address.ADD_DETIALS,
                        ADD_COUNTRY = employee.Address.ADD_COUNTRY,
                        ADD_STATE = employee.Address.ADD_STATE,
                    };
                }
                connection.EMPLOYEEs.Add(emp);
                connection.SaveChanges();

                return emp.ID;
            }
        }
        //get all records
        public List<Employee> GetAllEmployees()
        {
            using (var connection = new EmployeeDBEntities())
            {
                var results = connection.EMPLOYEEs.Select(model => new Employee()
                {
                    ID = model.ID,
                    EMP_ADDRESS_ID = model.EMP_ADDRESS_ID,
                    EMP_EMAIL = model.EMP_EMAIL,
                    EMP_CODE = model.EMP_CODE,
                    EMP_NAME = model.EMP_NAME,
                    Address = new Address()
                    {
                        AD_ID = model.ADDRESS.AD_ID,
                        ADD_DETIALS = model.ADDRESS.ADD_DETIALS,
                        ADD_COUNTRY = model.ADDRESS.ADD_COUNTRY,
                        ADD_STATE = model.ADDRESS.ADD_STATE
                    }
                }).ToList();
                return results;
            }
        }

        //get single records
        public Employee GetEmployee(int id)
        {
            using (var connection = new EmployeeDBEntities())
            {
                var results = connection.EMPLOYEEs.
                    Where(model => model.ID == id).
                    Select(model => new Employee()
                    {
                        ID = model.ID,
                        EMP_ADDRESS_ID = model.EMP_ADDRESS_ID,
                        EMP_EMAIL = model.EMP_EMAIL,
                        EMP_CODE = model.EMP_CODE,
                        EMP_NAME = model.EMP_NAME,
                        Address = new Address()
                        {
                            AD_ID = model.ADDRESS.AD_ID,
                            ADD_DETIALS = model.ADDRESS.ADD_DETIALS,
                            ADD_COUNTRY = model.ADDRESS.ADD_COUNTRY,
                            ADD_STATE = model.ADDRESS.ADD_STATE
                        }
                    }).FirstOrDefault();
                return results;
            }
        }

        //update a record
        public bool UpdateEmployee(int id, Employee employee)
        {
            using (var connection = new EmployeeDBEntities())
            {
                var empResult = connection.EMPLOYEEs.FirstOrDefault(model => model.ID == id);
                if (empResult != null)
                {
                    empResult.EMP_NAME = employee.EMP_NAME;
                    empResult.EMP_EMAIL = employee.EMP_EMAIL;
                    empResult.EMP_CODE = employee.EMP_CODE;
                }
                connection.SaveChanges();
                return true;
            }
        }

        public bool DeleteEmployee(int id)
        {
            using (var connection = new EmployeeDBEntities())
            {
                var empResult = connection.EMPLOYEEs.FirstOrDefault(model => model.ID == id);
                if (empResult != null)
                {
                    connection.EMPLOYEEs.Remove(empResult);
                    connection.SaveChanges();
                    return true;
                }
                return false;
            }
        }
        #endregion
    }
}
