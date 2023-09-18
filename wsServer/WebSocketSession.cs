using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using wsServer.Models;

namespace wsServer
{
    public class WebSocketSession : WebSocketBehavior
    {
        private static readonly ConcurrentDictionary<string, WebSocketSession> ClientSessions = new ConcurrentDictionary<string, WebSocketSession>();
        
        // PlayerId - SessionId format
        private static readonly ConcurrentDictionary<string, string> ClientIds_Player_Session = new ConcurrentDictionary<string, string>();
        
        // PlayerId - SessionId format
        private static readonly ConcurrentDictionary<string, string> ClientIds_Session_Player = new ConcurrentDictionary<string, string>();
        
        
        private static readonly ConcurrentDictionary<Guid, Room> Rooms = new ConcurrentDictionary<Guid, Room>();

        protected override void OnOpen()
        {
            ClientSessions.TryAdd(ID, this);
            ClientIds_Session_Player.TryAdd(ID, default);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            ClientSessions.TryRemove(ID, out _);

            var possibleMatch = Rooms.FirstOrDefault(f => f.Value.FirstPlayerId == ID);
            
            if (!possibleMatch.Equals(default))
                Rooms.TryRemove(possibleMatch.Key, out _);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var clientMessage = JsonSerializer.Deserialize<ClientRequest>(e.Data);

            switch (clientMessage.RequestType)
            {
                case "StartGameRequest":
                    HandleStart();
                    break;
                case "SetMyIdRequest":
                    HandleSetMyId(clientMessage.SetMyIdRequest);
                    break;
                case "MakeMoveRequest":
                    HandleMakeMoveRequest(clientMessage.MakeMoveRequest);
                    break;
                case "LostOnTimeRequest":
                    HandleLostOnTime(clientMessage.LostOnTimeRequest);
                    break;
            }
        }

        private void HandleLostOnTime(LostOnTimeRequest request)
        {
            var loserId = request.UserId;
            
            var roomId = request.GameId;
            var room = Rooms[roomId];

            string winnerId, winnerSessionId;
            if (room.FirstPlayerId == loserId)
            {
                winnerSessionId = room.SecondPlayerSessionId;
                winnerId = room.SecondPlayerId;
            }
            else
            {
                winnerSessionId = room.FirstPlayerSessionId;
                winnerId = room.FirstPlayerId;
            }

            var winnerPlayerSession = ClientSessions[winnerSessionId];

            var serializedResponse = JsonSerializer.Serialize(new ClientResponse
            {
                ResponseType = "GameFinishedResponse",
                GameFinishedResponse = new GameFinishedResponse
                {
                    Reason = "Timeout",
                    WinnerId = winnerId
                }
            });
            
            winnerPlayerSession.Send(serializedResponse);
            Send(serializedResponse);
        }

        private void HandleMakeMoveRequest(MakeMoveRequest request)
        {
            if(!Rooms.TryGetValue(request.GameId, out var room))
                throw new InvalidOperationException($"could not find room with 'gameId':'{request.GameId}'");

            if (!ClientSessions.TryGetValue(room.FirstPlayerSessionId, out var firstPlayerSession) ||
                !ClientSessions.TryGetValue(room.SecondPlayerSessionId, out var secondPlayerSession))
                throw new InvalidOperationException("could not get established connection for one or more players");
            
            var isGameFinished = room.MakeMove(request.CellIndex, request.Move, out var winnerId, out var winPattern);

            if (isGameFinished)
            {
                WebSocketSession winnerSession, loserSession;
                if (winnerId == room.FirstPlayerId)
                {
                    winnerSession = firstPlayerSession;
                    loserSession = secondPlayerSession;
                }
                else
                {
                    winnerSession = secondPlayerSession;
                    loserSession = firstPlayerSession;
                }

                winnerSession.Send(JsonSerializer.Serialize(new ClientResponse
                {
                    ResponseType = "GameFinishedResponse",
                    GameFinishedResponse = new GameFinishedResponse
                    {
                        Message = "You won !",
                        PreviouslyPlayedMove = request.Move,
                        PreviouslyFilledCellIndex = request.CellIndex,
                        WinPattern = winPattern,
                        Reason = "WonByRules"
                    }
                }));
                
                loserSession.Send(JsonSerializer.Serialize(new ClientResponse
                {
                    ResponseType = "GameFinishedResponse",
                    GameFinishedResponse = new GameFinishedResponse
                    {
                        Message = "You lost !",
                        PreviouslyPlayedMove = request.Move,
                        PreviouslyFilledCellIndex = request.CellIndex,
                        WinPattern = winPattern,
                        Reason = "WonByRules"
                    }
                }));
            }
            else
            {
                var currentMove = request.Move == "x" ? "o" : "x";

                var clientResponse = new ClientResponse
                {
                    ResponseType = "MakeMoveResponse",
                    MakeMoveResponse = new MakeMoveResponse
                    {
                        CurrentMove = currentMove,
                        PreviouslyPlayedMove = request.Move,
                        PreviouslyFilledCellIndex = request.CellIndex
                    }
                };
                
                firstPlayerSession.Send(JsonSerializer.Serialize(clientResponse));
                secondPlayerSession.Send(JsonSerializer.Serialize(clientResponse));
            }
        }

        private void StartGame(Room room)
        {
            if(!ClientSessions.TryGetValue(room.FirstPlayerSessionId, out var firstPlayerSession) || !ClientSessions.TryGetValue(room.SecondPlayerSessionId, out var secondPlayerSession))
                throw new InvalidOperationException("invalid player id");
            
            firstPlayerSession.Send(JsonSerializer.Serialize(new ClientResponse
            {
                ResponseType = "StartGameResponse",
                StartGameResponse = new StartGameResponse
                {
                    GameId = room.Id,
                    YourMove = room.FirstPlayerMove
                }
            }));
            
            secondPlayerSession.Send(JsonSerializer.Serialize(new ClientResponse
            {
                ResponseType = "StartGameResponse",
                StartGameResponse = new StartGameResponse
                {
                    GameId = room.Id,
                    YourMove = room.SecondPlayerMove
                }
            }));
        }

        private void HandleStart()
        {
            var roomId = Rooms
                .FirstOrDefault(f => string.IsNullOrEmpty(f.Value.SecondPlayerId))
                .Key;

            if (roomId.Equals(default))
            {
                var newRoomId = Guid.NewGuid();
                var firstPlayerId = ClientIds_Session_Player[ID];
                Rooms.TryAdd(newRoomId, new Room
                {
                    Id = newRoomId,
                    FirstPlayerId = firstPlayerId,
                    FirstPlayerSessionId = ID
                });
            }
            else
            {
                var room = Rooms[roomId];
                var secondPlayerId = ClientIds_Session_Player[ID];
                room.SecondPlayerId = secondPlayerId;
                room.SecondPlayerSessionId = ID;

                Rooms[roomId].RandomizePlayers();
                StartGame(Rooms[roomId]);
            }
        }

        private void HandleSetMyId(SetMyIdRequest request)
        {
            ClientIds_Player_Session.TryAdd(request.Id, ID);
            ClientIds_Session_Player[ID] = request.Id;
        }
    }
}