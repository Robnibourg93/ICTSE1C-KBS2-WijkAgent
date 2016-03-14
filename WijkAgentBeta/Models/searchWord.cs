using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgentBeta.Models
{
    public class searchWord
    {
        /*
            Searchword.cs
            Author: Simon Brink.
            Modified by: Simon Brink.
            Discription:
            Searchword class. Whenever an searchword is active the twitterstream will automatically search for tweets that contains that word.
             
        */
        public string word { get; set; }
        public int id { get; set; }

        public searchWord(int id, string word)
        {
            this.word = word;
            this.id = id;
        }

        public override string ToString()
        {
            return word;
        }
    }
}
