let currentPlayer = {
    id: `${Math.floor(Math.random() * 101)}`,
    move: null,
    gameId: null
};

const webSocketUri = "ws://localhost:8585";
const webSocket = new WebSocket(webSocketUri);
let interval;
let boardIsEnabled;

const clearBoard = function(){
    let cells = document.getElementById('board').children;
    for(let cell of cells){
        cell.style.backgroundColor = 'white';
        cell.textContent = '';
    }
}


const startNewGameButton = document.getElementById('startNewGame');
const enableStartGameButton = function(){
    startNewGameButton.disabled = false;
}

const disableStartGameButton = function(){
    startNewGameButton.disabled = true;
}

const initializeRoom = function(){
    clearBoard();
    document.getElementById("info").style.color = 'black';
    document.getElementById("info").textContent = '';
    document.getElementsByTagName("body")[0].style.backgroundColor = 'white';
    enableStartGameButton();
    boardIsEnabled = false;
}

initializeRoom();

webSocket.onmessage = (e) => {
    const responseData = JSON.parse(e.data);
    console.log(responseData);
    switch (responseData.ResponseType){
        case "StartGameResponse":
            handleStartGameResponse(responseData.StartGameResponse);
            break;
        case "MakeMoveResponse":
            handleMakeMoveResponse(responseData.MakeMoveResponse);
            break;
        case "GameFinishedResponse":
            handleGameFinishedResponse(responseData.GameFinishedResponse);
            break;
    }
}

webSocket.onopen = () => {
    webSocket.send(JSON.stringify({
        RequestType: 'SetMyIdRequest',
        SetMyIdRequest: {
            Id: currentPlayer.id
        }
    }))
}

const handleStartGameResponse = function(data){
    currentPlayer.move = data.YourMove;
    currentPlayer.gameId = data.GameId;
    
    clearBoard();    

    document.getElementsByTagName("body")[0].style.backgroundColor = 'powderblue';
    clearInterval(interval);

    if(currentPlayer.move === 'x'){
        boardIsEnabled = true;
        let timeForNextMove = 10;
        document.getElementById("info").textContent = `You have ${timeForNextMove} seconds to move`;
        interval = setInterval(() => {
            document.getElementById("info").textContent = `You have ${--timeForNextMove} seconds to move`;

            if(timeForNextMove === 0){
                document.getElementsByTagName("body")[0].style.backgroundColor = 'red';
                clearInterval(interval);
                document.getElementById("info").textContent = `You lost the game !`;
                
                webSocket.send(JSON.stringify({
                    RequestType: "LostOnTimeRequest",
                    LostOnTimeRequest: {
                        UserId: currentPlayer.id,
                        GameId: currentPlayer.gameId
                    }
                }))
            }
        }, 1000);
    }
    else{
        document.getElementById("info").textContent = "Opponent's move";
        boardIsEnabled = false;
    }
}

const handleGameFinishedResponse = function(data) {
    enableStartGameButton();
    switch (data.Reason){
        case "Timeout":
            handleTimeoutResponse(data);
            break;
        case "WonByRules":
            handleWonByRulesResponse(data);
            break;
    }
}

const handleTimeoutResponse = function(data){
    clearInterval(interval);
    clearBoard();
    
    if(data.WinnerId === currentPlayer.id){
        document.getElementsByTagName("body")[0].style.backgroundColor = 'limegreen';
        document.getElementById("info").textContent = `You won the game on time !`;
    }
    else{
        document.getElementsByTagName("body")[0].style.backgroundColor = 'red';
        document.getElementById("info").textContent = `You lost the game on time !`;
    }
}

const handleWonByRulesResponse = function(data){
    clearInterval(interval);
    document.getElementById("info").textContent = data.Message;
    document.getElementsByTagName("body")[0].style.backgroundColor = 'white';

    document.getElementById('board').children[data.PreviouslyFilledCellIndex].textContent = data.PreviouslyPlayedMove;

    if(data.Message === 'You won !'){
        document.getElementById("info").style.color = 'limegreen';
        document.getElementById("info").textContent = 'Congratulations, you won !!!';

        data.WinPattern.forEach(cellIndex => {
            document.getElementById('board').children[cellIndex].style.backgroundColor = 'limegreen';
        })
    }
    else{
        document.getElementById("info").style.color = 'red';
        document.getElementById("info").textContent = 'Sorry, you lost !!!';

        data.WinPattern.forEach(cellIndex => {
            document.getElementById('board').children[cellIndex].style.backgroundColor = 'red';
        })
    }
}

const handleMakeMoveResponse = function(data) {
    clearInterval(interval);

    if(currentPlayer.move === data.CurrentMove){
        boardIsEnabled = true;

        document.getElementById('board').children[data.PreviouslyFilledCellIndex].textContent = data.PreviouslyPlayedMove;

        let timeForNextMove = 10;
        document.getElementById("info").textContent = `You have ${timeForNextMove} seconds to move`;
        interval = setInterval(() => {
            document.getElementById("info").textContent = `You have ${--timeForNextMove} seconds to move`;

            if(timeForNextMove === 0){
                document.getElementsByTagName("body")[0].style.backgroundColor = 'red';
                clearInterval(interval);
                document.getElementById("info").textContent = `You lost the game !`;
            }
        }, 1000);
    }
}


function makeMove(cellIndex) {
    if(!boardIsEnabled)
        return;

    document.getElementById('board').children[cellIndex].textContent = currentPlayer.move;
    document.getElementById("info").textContent = "Opponent's move";
    boardIsEnabled = false;

    webSocket.send(JSON.stringify({
        RequestType: 'MakeMoveRequest',
        MakeMoveRequest: {
            GameId: currentPlayer.gameId,
            CellIndex: cellIndex,
            Move: currentPlayer.move
        }
    }));
}

function startGame(){
    webSocket.send(JSON.stringify({RequestType: 'StartGameRequest'}));

    initializeRoom();
    disableStartGameButton();

    let counter = 0;
    interval = setInterval(() => {

        switch (counter % 6){
            case 0:
                document.getElementById('info').textContent = 'Looking for opponent.';
                break;
            case 1:
                document.getElementById('info').textContent = 'Looking for opponent..';
                break;
            case 2:
                document.getElementById('info').textContent = 'Looking for opponent...';
                break;
            case 3:
                document.getElementById('info').textContent = 'Looking for opponent....';
                break;
            case 4:
                document.getElementById('info').textContent = 'Looking for opponent.....';
                break;
            case 5:
                document.getElementById('info').textContent = 'Looking for opponent......';
                break;
        }

        counter ++;
    }, 150)
}
