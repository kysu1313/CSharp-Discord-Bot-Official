using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.Helpers;
using ClassLibrary.Models;
using System.Text.RegularExpressions;
using ClassLibrary.Helpers;
using ClassLibrary.Models.ContextModels;

namespace DiscBotConsole.Modules
{
    public class BankCommands : ModuleBase
    {

        private readonly ApplicationDbContext _context;
        private readonly Helper _helper;
        private double _defaultExpGain = 3.0;
        private readonly string _diamond = "💎";
        private readonly string _fire = "🔥";
        private readonly string _cherry = "🍒";
        private readonly string _grape = "🍇";
        private readonly string _heart = "❤️";

        // percent would be: <number> / 100
        private int _diamondPercent = 93;
        private int _firePercent = 80;
        private int _cherryPercent = 60;
        private int _grapePercent = 50;
        private int _heartPercent = 0;

        // Prices, rarity is highest (top) -> lowest (bottom)
        private int _diamondPrice = 60;
        private int _firePrice = 20;
        private int _cherryPrice = 15;
        private int _grapePrice = 10;
        private int _heartPrice = 5;
        private int _quadDiamond = 1000;
        private int _tripleDiamond = 500;
        private int _quadFire = 500;
        private int _tripleFire = 250;
        private int _tripleCherry = 100;
        private int _tripleGrape = 30;
        private int _tripleHeart = 15;
        private int _oneOfEach = 500;

        private string[] _emojis = {
            "💎",
            "🔥",
            "🍒",
            "🍇",
            "❤️",
        };
        private int _slotsCost = 100;

        public BankCommands(ApplicationDbContext context, IServiceProvider services)
        {
            _context = context;
            _helper = new Helper(_context, services);
        }

        private string GetRandomEmoji()
        {
            Random rand = new Random();
            var nextRand = rand.Next(0, 100);
            string result =
                nextRand >= _diamondPercent ? "💎" :
                nextRand >= _firePercent ? "🔥" :
                nextRand >= _cherryPercent ? "🍒" :
                nextRand >= _grapePercent ? "🍇" :
                nextRand >= _heartPercent ? "❤️" : "";

            return result;
        }

        [Command("bank")]
        [Summary(
            "View current user or username's bank and wallet amounts.\n" +
            " !bank <?username>")]
        [RequireContext(ContextType.Guild)]
        public async Task ViewBank(string userName = "")
        {
            var sb = new StringBuilder();

            string bankAmount = "";
            string walletAmount = "";

            var user = Context.User;
            var guild = Context.Guild;

            if (userName != "")
            {

                var findUser = await Context.Guild.GetUsersAsync().ConfigureAwait(false);
                if (userName.Contains("@"))
                {
                    string uid = await _helper.CleanUserAtString(userName);
                    user = findUser.FirstOrDefault(x => x.Id == (ulong.TryParse(uid, out ulong id) == true ? id : 0));
                }
                else
                {
                    user = findUser.FirstOrDefault(x => x.Username == userName);
                }

                //return;
            }

            var userExists = new UserExperience();
            var users = await _helper.getAllUserInServer(guild.Id);
            if (users.Count > 0)
            {
                userExists = users.FirstOrDefault(x => x.userId == user.Id && x.serverId == guild.Id);
                if (userExists == null)
                {
                    await _helper.AddNewUser(user, guild).ConfigureAwait(false);
                    userExists = await _helper.getUserExperienceInServer(user.Id, guild.Id).ConfigureAwait(false);
                }
            }
            else
            {
                await _helper.AddNewUser(user, guild).ConfigureAwait(false);
                userExists = await _helper.getUserExperienceInServer(user.Id, guild.Id).ConfigureAwait(false);
            }

            bankAmount = userExists.bank.ToString();
            walletAmount = userExists.wallet.ToString();

            sb.AppendLine($"{userExists.userName} --> Bank: ${bankAmount}, Wallet: ${walletAmount}");

            // send simple string reply
            await ReplyAsync(sb.ToString());

        }

        [Command("pay")]
        [RequireContext(ContextType.Guild)]
        [Summary("Pay a user some amount.\n !pay <username> <amount to pay>")]
        public async Task PayMember(string userName, int amount = 0)
        {
            var sb = new StringBuilder();

            string bankAmount = "";
            string walletAmount = "";

            var payer = Context.User;
            var payee = Context.User;
            var guild = Context.Guild;

            if (userName != "")
            {
                var findUser = await Context.Guild.GetUsersAsync().ConfigureAwait(false);
                if (userName.Contains("@"))
                {
                    string uid = await _helper.CleanUserAtString(userName);
                    payee = findUser.FirstOrDefault(x => x.Id == (ulong.TryParse(uid, out ulong id) == true ? id : 0));
                }
                else
                {
                    payee = findUser.FirstOrDefault(x => x.Username == userName);
                }
            }
            else
            {
                sb.AppendLine($"Must add username of member you are paying: !pay <username> <amount>");

                await ReplyAsync(sb.ToString());
                return;
            }

            if (amount == 0 || amount < 0)
            {
                sb.AppendLine($"Must add positive amount to pay: !pay <username> <amount>");

                await ReplyAsync(sb.ToString());
                return;
            }

            var userExists = new UserExperience();
            var users = await _helper.getAllUserInServer(guild.Id);
            if (users.Count > 0)
            {
                userExists = users.FirstOrDefault(x => x.userId == payee.Id);
                if (userExists == null)
                {
                    await _helper.AddNewUser(payee, guild).ConfigureAwait(false);
                    userExists = users.FirstOrDefault(x => x.userId == payee.Id);
                }
            }
            else
            {
                await _helper.AddNewUser(payee, guild).ConfigureAwait(false);
                userExists = await _helper.getUserExperienceInServer(payee.Id, guild.Id).ConfigureAwait(false);
            }

            if (await _helper.CheckValidAmount(payer, guild, amount).ConfigureAwait(false))
            {
                await _helper.UpdateMoney(payer.Id, guild, -amount);
                await _helper.UpdateMoney(payee.Id, guild, amount);
            }
            else
            {
                sb.AppendLine($"Sorry but you are super poor.");

                await ReplyAsync(sb.ToString());
                return;
            }
            //var updatedUser = _helper.GetUserMoney(pay)
            bankAmount = userExists.bank.ToString();
            walletAmount = userExists.wallet.ToString();

            sb.AppendLine($"{payer.Username} paid {payee.Username} ${amount.ToString()}");

            // send simple string reply
            await ReplyAsync(sb.ToString());

        }

        [Command("adjust")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary(
            "Adjust the bank / wallet of a member. MOD ONLY.\n" +
            " !adjust <username> <amount to set>")]
        public async Task AdjustMoney(string userName = "", int amount = 0)
        {
            var sb = new StringBuilder();

            int bankAmount = 0;
            int walletAmount = 0;

            var user = Context.User;
            var guild = Context.Guild;

            if (userName != "")
            {
                var findUser = await Context.Guild.GetUsersAsync().ConfigureAwait(false);
                if (userName.Contains("@"))
                {
                    string uid = await _helper.CleanUserAtString(userName);
                    user = findUser.FirstOrDefault(x => x.Id == (ulong.TryParse(uid, out ulong id) == true ? id : 0));
                }
                else
                {
                    user = findUser.FirstOrDefault(x => x.Username == userName);
                }
            }
            else if (amount == 0)
            {
                sb.AppendLine($"You must add an amount: !adjust <username> <amount>");
                await ReplyAsync(sb.ToString());
                return;
            }
            else
            {
                sb.AppendLine($"You must add name of member to adjust: !adjust <username> <amount>");
                await ReplyAsync(sb.ToString());
                return;
            }

            await _helper.UpdateMoney(user.Id, guild, amount);
            (bankAmount, walletAmount) = await _helper.GetUserMoney(user, guild);

            sb.AppendLine($"{userName} --> Bank: ${bankAmount}, Wallet: ${walletAmount}");
            await ReplyAsync(sb.ToString());

        }

        [Command("slots")]
        [Summary(
            "Simple slots game.\n" +
            " !slots")]
        [RequireContext(ContextType.Guild)]
        public async Task PlaySlots()
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            int bankAmount = 0;
            int walletAmount = 0;

            var user = Context.User;
            var guild = Context.Guild;

            var findUser = await Context.Guild.GetUsersAsync().ConfigureAwait(false);
            user = findUser.FirstOrDefault(x => x.Id == user.Id);

            if (!(await _helper.CheckValidAmount(user, guild, _slotsCost)))
            {
                sb.AppendLine("You are soooo poor!");
                await ReplyAsync(sb.ToString());
                return;
            }

            Random rand = new Random();

            string l1r1 = GetRandomEmoji();
            string l1r2 = GetRandomEmoji();
            string l1r3 = GetRandomEmoji();
            string l2r1 = GetRandomEmoji();
            string l2r2 = GetRandomEmoji();
            string l2r3 = GetRandomEmoji();
            string l3r1 = GetRandomEmoji();
            string l3r2 = GetRandomEmoji();
            string l3r3 = GetRandomEmoji();

            string[] roll = {l1r1,l1r2,l1r3,l2r1,l2r2,l2r3,l3r1,l3r2,l3r3};
            int totalReward = 0;
            int diamonds = 0, fires = 0, cherries = 0, grapes = 0, hearts = 0;

            foreach (var ele in roll)
            {
                switch (ele)
                {
                    case "💎":
                        totalReward += _diamondPrice;
                        diamonds += 1;
                        break;
                    case "🔥":
                        totalReward += _firePrice;
                        fires += 1;
                        break;
                    case "🍒":
                        totalReward += _cherryPrice;
                        cherries += 1;
                        break;
                    case "🍇":
                        totalReward += _grapePrice;
                        grapes += 1;
                        break;
                    case "❤️":
                        totalReward += _heartPrice;
                        hearts += 1;
                        break;
                }
            }

            if (diamonds == 3 && fires == 3 && cherries == 3)
            {
                totalReward += 15000;
                sb.AppendLine("JACKPOT HIT !!!");
            }

            totalReward += diamonds == 3 ? _tripleDiamond : 0;
            totalReward += fires == 3 ? _tripleFire : 0;
            totalReward += cherries == 3 ? _tripleCherry : 0;
            totalReward += grapes == 4 ? _tripleGrape : 0;
            totalReward += diamonds >= 1 && fires >= 2 && cherries >= 3 ? 250 : 0;
            totalReward += 
                diamonds >= 1 && 
                fires >= 1 && 
                cherries >= 1 &&
                grapes >= 1 &&
                hearts >= 1 ? _oneOfEach : 0;

            sb.AppendLine(
                l1r1 + " " +
                l1r2 + " " +
                l1r3
            );
            sb.AppendLine(
                l2r1 + " " +
                l2r2 + " " +
                l2r3
            );
            sb.AppendLine(
                l3r1 + " " +
                l3r2 + " " +
                l3r3
            );

            embed.WithColor(new Color(0, 255, 0));
            embed.Title = "⭐️ SLOTS! ⭐️";

            totalReward = totalReward - _slotsCost < 0 ? 0 : totalReward - _slotsCost;
            await _helper.UpdateMoney(user.Id, guild, totalReward);
            (bankAmount, walletAmount) = await _helper.GetUserMoney(user, guild);

            sb.AppendLine("");
            sb.AppendLine($"Cost: ${_slotsCost}");
            sb.AppendLine($"Total Winnings: ${totalReward}");

            embed.Description = sb.ToString();

            await ReplyAsync(null, false, embed.Build());

        }

        [Command("highlow")]
        [Summary(
            "Bet an amount of money on whether the number will be higher / lower than 5.\n" +
            " !highlow <higher / lower> <amount to bet>")]
        [RequireContext(ContextType.Guild)]
        public async Task HighLow(string betStr = "", int betAmt = 0)
        {
            var sb = new StringBuilder();

            if (betStr == "" || betAmt == 0)
            {
                sb.AppendLine($"Must add positive amount to pay: !pay <username> <amount>");

                await ReplyAsync(sb.ToString());
                return;
            }

            string bankAmount = "";
            string walletAmount = "";
            int totalWinnings = betAmt;

            var user = Context.User;
            var guild = Context.Guild;

            await _helper.AddNewUser(user, guild);
            var userExists = new UserExperience();
            userExists = await _helper.getUserExperienceInServer(user.Id, guild.Id);
            if (userExists != null)
            {
                bankAmount = userExists.bank.ToString();
                walletAmount = userExists.wallet.ToString();
            }

            Random rand = new Random();
            int value = rand.Next(0, 10);

            if (betStr == "higher")
            {
                if (value >= 5)
                {
                    totalWinnings *= 2;
                    sb.AppendLine($"YOU WIN {totalWinnings}");
                    await _helper.UpdateMoney(user.Id, guild, totalWinnings);
                }
                else
                {
                    sb.AppendLine($"You lose {-totalWinnings}");
                    await _helper.UpdateMoney(user.Id, guild, -totalWinnings);
                }
            }
            else if (betStr == "lower")
            {
                if (value < 5)
                {
                    totalWinnings *= 2;
                    sb.AppendLine($"YOU WIN {totalWinnings}");
                    await _helper.UpdateMoney(user.Id, guild, totalWinnings);
                }
                else
                {
                    sb.AppendLine($"You lose {-totalWinnings}");
                    await _helper.UpdateMoney(user.Id, guild, -totalWinnings);
                }
            }

            await ReplyAsync(sb.ToString());

        }

        [Command("steal")]
        [RequireContext(ContextType.Guild)]
        public async Task Steal(string username = "", int amount = 0)
        {
            var sb = new StringBuilder();

            var user = Context.User;
            var guild = Context.Guild;

            if (username == "" || amount <= 0)
            {
                sb.AppendLine($"Must add victim username & positive amount to steal: !pay <username> <amount>");
                await ReplyAsync(sb.ToString());
                return;
            }
            else if (username.Contains("@"))
            {
                var findUser = await Context.Guild.GetUsersAsync().ConfigureAwait(false);
                if (username.Contains("@"))
                {
                    string uid = await _helper.CleanUserAtString(username);
                    username = findUser.FirstOrDefault(x => x.Id == (ulong.TryParse(uid, out ulong id) == true ? id : 0)).Username;
                }
            }
            
            var victim = await _helper.getUserNameExperienceInServer(user, guild);
            
            if (victim == null)
            {
                sb.AppendLine($"Hmm, can't find that user..");
                await ReplyAsync(sb.ToString());
                return;
            }
            
            if (victim.bank + victim.wallet < amount)
            {
                sb.AppendLine($"{username} only has {victim.bank + victim.wallet}. lol, so poor");
                await ReplyAsync(sb.ToString());
                return;
            }

            var userExists = await _helper.getUserExperienceInServer(user.Id, guild.Id);
            int userLuck = userExists.luck == null ||
                           userExists.luck== 0 ? 1 :
                           userExists.luck;
            bool result = GetWeightedGuess(userLuck);

            // TESTING
            //result = true;

            if (result)
            {
                await _helper.UpdateMoney((ulong)victim.id, guild, -amount);
                await _helper.UpdateMoney(user.Id, guild, 2*amount);
                userExists.luck += 1;
                userExists.experience += (float)(userExists.experience * _defaultExpGain);
                await _helper.UpdateUser(guild, user: userExists);
                sb.AppendLine($"SUCCESS!");
                sb.AppendLine($"You stole ${amount} from {victim.userName}");
            }
            else
            {
                int lostAmt = (int)(0.5 * amount);
                await _helper.UpdateMoney((ulong)victim.id, guild, lostAmt);
                await _helper.UpdateMoney(user.Id, guild, 2 * -lostAmt);
                sb.AppendLine($"So bad lol");
                sb.AppendLine($"{victim.userName} took ${lostAmt} from you lol");
            }


            await ReplyAsync(sb.ToString());

        }

        private bool GetWeightedGuess(int luck)
        {
            Random rand = new Random();

            // Random number determined by users luck score.
            int initial = rand.Next(luck, luck + 10);
            int value = rand.Next(0, luck + 13);

            if (initial > value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
