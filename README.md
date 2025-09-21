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

4. Basic steps to run the application
4.1 - Application uses In-Memory datastore to store data of player, submission and to build leaderboard 
Navigate to the root folder
run command: 'dotnet build' to build the projects' dlls.
run command: 'dotnet test' to verify if all UnitTest cases are successful
cd to \src\Leaderboard.Api
run command: 'dotnet run' to run the api
Open this url in browser: http://localhost:5199/swagger/index.html

4.2 - Application uses In-Memory datastore to store data of player, submission and leverage the StackExchange.Redis library to accelerate the performance of ranking and building leaderboard
Open appsettings.json in src\Leaderboard.Api, update "ConnectionString": "localhost:6379" (Please make sure that your machine runs a redis server already.)
Navigate to the root folder
run command: 'dotnet build' to build the projects' dlls.
run command: 'dotnet test' to verify if all UnitTest cases are successful
cd to \src\Leaderboard.Api
run command: 'dotnet run' to run the api
Open this url in browser: http://localhost:5199/swagger/index.html

The following data is seeded everytime we run the application:
PlayerId: 0a111111-1111-1111-1111-111111111111; PlayerName: "Alice", Score: 10000
// 6 duplicates (score = 9000)
PlayerId: 0a222222-2222-2222-2222-222222222222; PlayerName: "Bob"), Score: 9000
PlayerId: 0a333333-3333-3333-3333-333333333333; PlayerName: "Carol"), Score: 9000
PlayerId: 0a444444-4444-4444-4444-444444444444; PlayerName: "Dave"), Score: 7000
PlayerId: 0a555555-5555-5555-5555-555555555555; PlayerName: "Grace"), Score: 6000
// 6 duplicates (score = 5000)
PlayerId: 0a666666-6666-6666-6666-666666666666; PlayerName: "Frank"), Score: 5000
PlayerId: 0a777777-7777-7777-7777-777777777777; PlayerName: "Eve"), Score: 5000
PlayerId: 0a888888-8888-8888-8888-888888888888; PlayerName: "Heidi"), Score: 5000
PlayerId: 0a999999-9999-9999-9999-999999999999; PlayerName: "Ivan"), Score: 5000
PlayerId: 0b111111-1111-1111-1111-111111111111; PlayerName: "Judy"), Score: 5000
PlayerId: 0b222222-2222-2222-2222-222222222222; PlayerName: "Karl"), Score: 5000

// remaining players
PlayerId: 0b333333-3333-3333-3333-333333333333; PlayerName: "Leo"), Score: 3500
PlayerId: 0b444444-4444-4444-4444-444444444444; PlayerName: "Mallory"), Score: 3000
PlayerId: 0b555555-5555-5555-5555-555555555555; PlayerName: "Neil"), Score: 2500
PlayerId: 0b666666-6666-6666-6666-666666666666; PlayerName: "Olivia"), Score: 2000
PlayerId: 0b777777-7777-7777-7777-777777777777; PlayerName: "Peggy"), Score: 1500
PlayerId: 0b888888-8888-8888-8888-888888888888; PlayerName: "Quentin"), Score: 1000
PlayerId: 0b999999-9999-9999-9999-999999999999; PlayerName: "Rupert"), Score: 900
PlayerId: 0c111111-1111-1111-1111-111111111111; PlayerName: "Sybil"), Score: 800
PlayerId: 0c222222-2222-2222-2222-222222222222; PlayerName: "Trent"), Score: 700

Now, let's try /api/leaderboard with playerId: 0b222222-2222-2222-2222-222222222222
Response includes: 
{
	"playerRank": 5,
	"playerScore": 5000,
	"topPlayers": [...],   // Top 10 players does not include the playerId: 0b222222-2222-2222-2222-222222222222
	"nearbyPlayers": [...] // 4 nearby players
}

Try /api/submit with the body:
{
  "playerId": "0b222222-2222-2222-2222-222222222222",
  "score": 1000
}
Now, Response is changed as:
{
  "playerRank": 4,    // from 5->4
  "playerScore": 6000, // 5000 + 1000 -> 6000
  "topPlayers": [...],   // Top 10 players now includes the playerId: 0b222222-2222-2222-2222-222222222222
  "nearbyPlayers": [...] // 4 NEW nearby players
}

Try /api/Reset without any required parameter
Then again try /api/leaderboard with playerId: 0b222222-2222-2222-2222-222222222222
Now, we just give none players
{
  "playerRank": 0,
  "playerScore": 0,
  "topPlayers": [],
  "nearbyPlayers": []
}


