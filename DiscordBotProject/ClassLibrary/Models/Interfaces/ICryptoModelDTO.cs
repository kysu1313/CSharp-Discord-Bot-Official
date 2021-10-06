using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models.ContextModels;

namespace ClassLibrary.ModelDTOs
{
    public interface ICryptoModelDTO
    {
        Task<CryptoModel> GetCoin(string name);
        Task<IEnumerable<CryptoModel>> GetAllCoins();
        Task AddCoin(string name, float value);
        Task UpdatePrices(string name, float value);
    }
}