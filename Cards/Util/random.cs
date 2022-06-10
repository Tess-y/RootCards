using System;
using System.Collections.Generic;
using System.Linq;
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
                cardcount += p.data.currentCards.Select(card =>
                {
                    int n = 0; 
                    foreach (char c in card.name)
                    {
                        n += ((int)c) * p.data.currentCards.IndexOf(card);
                    }
                    return n;
                }).Sum();
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
