using System.Collections.Generic;
using Mogade.WindowsPhone;

namespace PokemonMadjong
{
   public enum Leaderboards
   {
      Main = 1,
   }
   public class MogadeHelper
   {
       //Your game key and game secret
       private const string _gameKey = "4fa70f1a563d8a123a000011";
       private const string _secret = "80UvwIAIEeq`a>Lou4saACh48wKH2x";
       private static readonly IDictionary<Leaderboards, string> _leaderboardLookup = new Dictionary<Leaderboards, string>
                                                                                           {
                                                                                              {Leaderboards.Main, "4fa70f4c563d8a02f0000178"}
                                                                                           };

       public static string LeaderboardId(Leaderboards leaderboard)
       {
           return _leaderboardLookup[leaderboard];
       }

       public static IMogadeClient CreateInstance()
       {
           //MogadeConfiguration.Configuration(c => c.UsingUniqueIdStrategy(UniqueIdStrategy.UserId));
           return MogadeClient.Initialize(_gameKey, _secret);
       }
   }
}