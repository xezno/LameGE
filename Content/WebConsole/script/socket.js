var port = 42069;
var socket;

var packetTypes = {
    Handshake: 0,
    Input: 1,
    Response: 2,
    InputInProgress: 3,
    Suggestions: 4,
    LogHistory: 5,
    RequestAuth: 6,
    Authenticate: 7,

    Error: 255
};

function sendObject(obj)
{
    socket.send(JSON.stringify(obj));
}

function sendHandshake()
{
    var packet = {
        origin: 0,
        type: packetTypes.Handshake
    }
    sendObject(packet);
}

function sendInput(input)
{
    var packet = {
        origin: 0,
        type: packetTypes.Input,
        data: {
            input: input
        }
    }
    sendObject(packet);
}

function sendInputInProgress(input)
{
    if (input.indexOf(" ") >= 0)
        input = input.substring(0, input.indexOf(" "));
    
    var packet = {
        origin: 0,
        type: packetTypes.InputInProgress,
        data: {
            input: input
        }
    }
    sendObject(packet);
}

function sendAuthentication()
{
    var password = prompt("Enter password");
    var packet = {
        origin: 0,
        type: packetTypes.Authenticate,
        data: {
            password: password
        }
    }
    sendObject(packet);
}

function handleMessage(e)
{
    e.data.text().then((res) => {
        var packet = JSON.parse(res);
        switch (packet.type)
        {
            case packetTypes.RequestAuth:
                sendAuthentication();
                break;
            case packetTypes.Response: // Response
            case packetTypes.LogHistory: // Response
                writeLogString(packet);
                break;
            case packetTypes.Suggestions: // Suggestions
                if (packet.data.suggestions == null)
                {
                    writeSuggestions([]);
                }
                else
                {
                    writeSuggestions(JSON.parse(packet.data.suggestions));
                }
                break;
            case packetTypes.Error: // Error
                alert("Error: " + packet.data.errorMessage);
                break;
            default:
                console.log("Unhandled ", res);
                break;
        }
    });
}

function writeLogString(packet)
{
    logMessage(packet.data.timestamp, packet.data.stackTrace, packet.data.str, packet.data.severity);
}

function tryConnect()
{
    console.log("Trying to connect...");
    socket = new WebSocket("ws://127.0.0.1:" + port, [ "ulaidRcon" ]);
    socket.addEventListener("message", handleMessage);
    socket.addEventListener("open", () => sendHandshake());
    socket.addEventListener("close", () => console.log("Connection closed"));
    socket.addEventListener("error", function(e) {
        console.log("Websocket fucked it (again): ", event);

        // Retry connection
        tryConnect();
    });
}

tryConnect();