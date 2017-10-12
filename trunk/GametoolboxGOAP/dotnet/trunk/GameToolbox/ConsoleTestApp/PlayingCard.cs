﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTestApp
{
	public enum PlayingCardSuit { Hearts, Spades, Diamonds, Clubs, None };
	public enum PlayingCardRank
	{
		Joker = 0, Ace = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10,
		Jack = 11, Queen = 12, King = 13
	};

	public class PlayingCard
	{
		public PlayingCardRank Rank { get; set; }
		public PlayingCardSuit Suit { get; set; }
	}
}