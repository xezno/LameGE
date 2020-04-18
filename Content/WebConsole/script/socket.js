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
            password: password + "\0"
        }
    }
    sendObject(packet);
    console.log("Sent handshake");
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
            default:
                console.log("Unhandled ", res);
                break;
        }
    });
}

function writeLogString(packet)
{
    logMessage(packet.data.logString, packet.data.severity);
}

socket.addEventListener("message", handleMessage);
socket.addEventListener("open", () => sendHandshake());
socket.addEventListener("error", function(e) {
    console.log("Websocket fucked it (again): ", event);
    alert("oops! we did a fucky wucky - check the browser console owo");
});