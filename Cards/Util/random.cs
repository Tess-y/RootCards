using System;
using System.Collections.Generic;
using System.Text;

namespace RootCards.Cards.Util
{
    internal class random
    {
        public static int Seed()
        {
            int seed = 0;
            int cardcount = 0;
            PlayerManager.instance.players.ForEach(p =>
            {
                cardcount += p.data.currentCards.Count;
                foreach (char c in p.name)
                {
                    seed += ((int)c) * p.playerID;
                }
            });
            seed *= cardcount;
            return seed;
        }
    }
}
