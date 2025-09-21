using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaderboard.Domain
{
    public class PlayerEntry
    {
        public int Score { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
