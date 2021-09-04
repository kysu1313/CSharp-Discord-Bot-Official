using DiscBotConsole.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscBotConsole.Models
{
    public class CryptoModelDTO
    {

        private readonly ApplicationDbContext _context;

        public CryptoModelDTO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CryptoModel> GetCoin(string name)
        {
            var coin = await _context.CryptoModels
                                .FirstOrDefaultAsync(x => x.coinName == name)
                                .ConfigureAwait(false);
            return coin;
        }

        public async Task<IEnumerable<CryptoModel>> GetAllCoins()
        {
            var coins = await _context.CryptoModels.ToListAsync();
            return coins;
        }

        public async Task AddCoin(string name, float value)
        {
            CryptoModel coin = new CryptoModel()
            {
                coinName = name,
                price1 = value,
                price2 = value,
                price3 = value,
                price4 = value,
                price5 = value,
                dateUpdated = DateTime.Now
            };

            var coins = await _context.CryptoModels.ToListAsync();

            if (!coins.Select(x => x.coinName).Contains(name.ToUpper()))
            {
                await _context.CryptoModels.AddAsync(coin);
                await _context.SaveChangesAsync();
            }
            else
            {
                await UpdatePrices(name, value);
            }
        }

        public async Task UpdatePrices(string name, float value)
        {
            var coin = await _context.CryptoModels.FirstOrDefaultAsync(x => x.coinName == name.ToUpper());

            if (coin == null)
            {
                await AddCoin(name, value);
            }
            else
            {
                coin.price5 = coin.price4;
                coin.price4 = coin.price3;
                coin.price3 = coin.price2;
                coin.price2 = coin.price1;
                coin.price1 = value;
                coin.dateUpdated = DateTime.Now;

                _context.CryptoModels.Update(coin);
                await _context.SaveChangesAsync();
            }
        }

    }
}
