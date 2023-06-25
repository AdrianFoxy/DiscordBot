using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LysDicordBot.ExternalClasses
{
    internal class CardBuilder
    {
        public int[] cardNumber = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        public string[] cardSuits = { "Clubs", "Spades", "Diamonds", "Hearts" };

        public int SelectedNumber { get; internal set; }
        public string SelectedCard { get; internal set; }

        public CardBuilder()
        {
            var Random = new Random();
            int indexNumbers = Random.Next(0, this.cardNumber.Length - 1);
            int indexSuit = Random.Next(0, this.cardSuits.Length - 1);

            this.SelectedNumber = this.cardNumber.ElementAt(indexNumbers);
            this.SelectedCard = this.cardNumber.ElementAt(indexNumbers) + " of " + this.cardSuits.ElementAt(indexSuit);
        }
    }
}
