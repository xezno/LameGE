var consoleWrapper = document.getElementById("console-wrapper");
var consoleInput = document.getElementById("console-input");

function inputKeyDown(e)
{
    if (e.keyCode == 13)
    {
        // submit console input
        logInput(consoleInput.value);
        consoleInput.value = "";
    }
    
    if (consoleInput.value == "")
    {
        document.getElementById("console-suggestions").style.visibility = "hidden";
    }
    else
    {
        document.getElementById("console-suggestions").style.visibility = "visible";
        sendInputInProgress(consoleInput.value);
    }
}

function writeSuggestions(suggestionsList)
{
    document.getElementById("console-suggestions").innerHTML = "";

    if (suggestionsList.length == 0)
    {
        document.getElementById("console-suggestions").innerHTML = `
        <li>
            <span class="console-suggestion-error">No commands found.</span>
        </li>`;
        return;
    }

    for (var suggestion of suggestionsList)
    {
        var template = `
            <li>
                <span class="console-suggestion-command">{{alias}} <span class="console-suggestion-value">{{currentValue}}</span></span>
                <span class="console-suggestion-description">{{description}}</span>
            </li>`;
            
        var templateProcessed = template.replace("{{alias}}", suggestion.aliases[0]).replace("{{description}}", suggestion.description).replace("{{currentValue}}", suggestion.value);

        document.getElementById("console-suggestions").innerHTML += templateProcessed;
    }
}

function writeToConsole(str)
{
    document.getElementById("console").innerHTML += str;
    consoleWrapper.scrollTo(0, consoleWrapper.scrollHeight);
}

function logMessage(timestamp, stackTrace, str, severity)
{
    var template = `
        <div class="console-message {{severity}}">
            <span class="console-timestamp">{{timestamp}}</span>
            <span class="console-message-string">{{str}}</span>
        </div>
    `;

    var templateProcessed = template.replace("{{severity}}", severity).replace("{{timestamp}}", timestamp).replace("{{str}}", str).replace("{{stackTrace}}", stackTrace);
    writeToConsole(templateProcessed);
}

function logInput(str)
{
    var template = `
        <div class="console-input-message">
            <span class="console-input-message-string">{{str}}</span>
        </div>
    `;

    var templateProcessed = template.replace("{{str}}", "> " + str);
    writeToConsole(templateProcessed);
    sendInput(str);
}