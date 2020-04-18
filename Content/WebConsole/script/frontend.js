var consoleWrapper = document.getElementById("console-wrapper");
var consoleInput = document.getElementById("console-input");
function inputKeyDown(e)
{
    if (e.keyCode == 13)
    {
        // submit console input
        logInput("> " + consoleInput.value);
        consoleInput.value = "";
    }
}

function writeToConsole(str)
{
    document.getElementById("console").innerHTML += str;
    consoleWrapper.scrollTo(0, consoleWrapper.scrollHeight);
}

function logMessage(str, severity)
{
    var template = `
        <div class="console-message {{severity}}">
            <span class="console-timestamp">{{timestamp}}</span>
            <span class="console-message-string">{{str}}</span>
        </div>
    `;

    var templateProcessed = template.replace("{{severity}}", severity).replace("{{timestamp}}", "00:00:00").replace("{{str}}", str);
    writeToConsole(templateProcessed);
}

function logInput(str)
{
    var template = `
        <div class="console-input-message">
            <span class="console-input-message-string">{{str}}</span>
        </div>
    `;

    var templateProcessed = template.replace("{{str}}", str);
    writeToConsole(templateProcessed);
}