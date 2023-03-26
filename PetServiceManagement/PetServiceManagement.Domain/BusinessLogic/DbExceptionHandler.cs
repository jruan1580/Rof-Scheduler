using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace PetServiceManagement.Domain.BusinessLogic
{
    //TODO: consider moving to shared library
    public static class DbExceptionHandler
    {
        private static int _uniqueConstraintCode = 2627;

        public static void HandleDbUpdateException(DbUpdateException dbUpdateException, string entity)
        {
            if (dbUpdateException.InnerException == null)
            {
                throw new Exception(dbUpdateException.Message);
            }

            if (!ExtractSqlException(dbUpdateException, out var sqlException))
            {
                throw new Exception(dbUpdateException.InnerException.Message);
            }

            CheckIfUniqueConstraintException(sqlException, entity);
        }

        private static bool ExtractSqlException(DbUpdateException dbUpdateException, out SqlException sqlException)
        {
            sqlException = null;
            if (dbUpdateException.InnerException.GetType() != typeof(SqlException))
            {
                return false;
            }

            sqlException = dbUpdateException.InnerException as SqlException;
            return true;
        }

        private static void CheckIfUniqueConstraintException(SqlException sqlException, string entity)
        {
            if (sqlException.Number == _uniqueConstraintCode)
            {
                throw new ArgumentException($"{entity} already exists");
            }

            throw new Exception(sqlException.Message);
        }
    }
}
