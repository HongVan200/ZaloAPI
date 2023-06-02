using static ZaloMiniAppAPI.Controllers.ZaloAcountController;

namespace ZaloMiniAppAPI
{
    public class ZaloService 
    {
        private readonly IAccountRepository _accountRepository;

        public ZaloService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public bool LinkAccountWithZalo(string zaloId)
        {
            // Tạo đối tượng Account với Zalo ID và lưu vào cơ sở dữ liệu
            Acount account = new Acount { ZaloId = zaloId };
            _accountRepository.SaveAccount(account);

            // Trả về true nếu lưu thành công
            return true;
        }
    }
}
