using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClassesManagerReborn;
using RootCards.Cards;

namespace RootCards.Util
{
    public class Contract : ClassHandler
    {
        public override IEnumerator Init()
        {
            while (!(LilithsDeal.card && ContractOfSouls.card && TheDarkQueen.card)) yield return null;
            ClassesRegistry.Register(LilithsDeal.card, CardType.Entry | CardType.NonClassCard);
            ClassesRegistry.Register(ContractOfSouls.card, CardType.Gate | CardType.NonClassCard, LilithsDeal.card);
            ClassesRegistry.Register(TheDarkQueen.card, CardType.Card | CardType.NonClassCard, ContractOfSouls.card);
            yield break;
        }
    }
}
