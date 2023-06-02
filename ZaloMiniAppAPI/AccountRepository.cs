using static ZaloMiniAppAPI.AccountRepository;

namespace ZaloMiniAppAPI
{
   
    
        public class AccountRepository : IAccountRepository
        {
            private readonly ProductStore _dbContext;

            public AccountRepository(ProductStore dbContext)
            {
                _dbContext = dbContext;
            }

            public void SaveAccount(Acount account)
            {
                _dbContext.Acount.Add(account);
                _dbContext.SaveChanges();
            }
        }
    
}
