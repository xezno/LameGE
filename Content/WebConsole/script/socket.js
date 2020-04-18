var port = 42069;
var socket = new WebSocket("ws://127.0.0.1:" + port, [ "ulaidRcon" ]);
var password = prompt("Enter password");

function sendObject(obj)
{
    socket.send(JSON.stringify(obj));
}

function sendHandshake()
{
    var packet = {
        origin: 0,
        type: 0,
        data: {
            password: password
        }
    }
    sendObject(packet);
    console.log("Sent handshake");
}

function sendInput(input)
{
    var packet = {
        origin: 0,
        type: 1,
        data: {
            input: input
        }
    }
    sendObject(packet);
}

function sendInputInProgress(input)
{
    var packet = {
        origin: 0,
        type: 3,
        data: {
            input: input
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
            case 0x02: // Response
            case 0x05: // Response
                writeLogString(packet);
                break;
            case 0x04: // Suggestions
                if (packet.data.suggestions == null)
                {
                    writeSuggestions([]);
                }
                else
                {
                    writeSuggestions(JSON.parse(packet.data.suggestions));
                }
                break;
            case 0xFF: // Error
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

socket.addEventListener("message", handleMessage);
socket.addEventListener("open", () => sendHandshake());
socket.addEventListener("error", function(e) {
    console.log("Websocket fucked it (again): ", event);
    alert("oops! we did a fucky wucky - check the browser console owo");
});