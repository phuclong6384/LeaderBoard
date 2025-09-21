1. Sequence Diagrams for leaderboard score submission:
<img width="2942" height="1815" alt="image" src="https://github.com/user-attachments/assets/903ab449-0a36-43ff-a806-e46ef43c1c0b" />


2. The configuration as follows 
{ 
  “TopLimit”: 10,            // get Top players then add into the response from Submission and GetLeaderboard query.
  “NearbyRange”: 2           // get Nearby players then add into the response from Submission and GetLeaderboard query. Ex: The value 2 means Nearby players of the current player inlcudes 2 previous and next 2 players in the top list.
  “ResetIntervalHours”: 24   // for every interval hours that each Reset activity executes.
}

3. The data schema for storing the leaderboard data
The leaderboard data is the combination of data aggreated from 2 main entities: Player and Submission and calculation in the Leaderboard builder and Ranking service.
Basically, it contains:
- PlayerRank: The player current rank - integer
- PlayerScore: The player current score - integer
- TopPlayers: The top leaderboard players with their scores - (Guid PlayerId, string PlayerName, long Score, int PlayerRank)
- NearbyPlayers: A list of players above and below the specific player, based on the current rank - (Guid PlayerId, string PlayerName, long Score, int PlayerRank)
