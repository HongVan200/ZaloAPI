using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ZaloMiniAppAPI;

namespace ZaloMiniAppAPI
{
    public class MYSERVICE 
    {
        private readonly ProductStore _dbContext;

        public MYSERVICE(ProductStore dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Products> GetProductList()
        {
            return _dbContext.Products!.ToList();
        }
        public void CheckDatabaseConnection()
        {
            bool isConnected = _dbContext.IsDatabaseConnected();

            if (isConnected)
            {
                Console.WriteLine("Database connection is successful.");
            }
            else
            {
                Console.WriteLine("Failed to connect to the database.");
            }
        }

    }
}
