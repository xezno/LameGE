# Rcon / WebConsole protocol

## Packet Types

|Type               |ID         |
|-------------------|-----------|
|Handshake          |0          |
|Input              |1          |
|Response           |2          |
|InputInProgress    |3          |
|Suggestions        |4          |
|RequestAuth        |5          |
|Authenticate       |6          |
|RequestLogHistory  |7          |
|LogHistory         |8          |
|Error              |255        |

## Packet Layout

A JSON object serialized as a string sent via WebSockets.

|Name               |Description                        |Type               |
|-------------------|-----------------------------------|-------------------|
|Type               |The type of the packet being sent. |PacketType (int)   |
|Data               |The data for the packet (optional) |Object             |

### Example

```json
{
    "type": 3,
    "data": {
        "input":  "Hello, World!"
    }
}
```

## Packets

TODO

### Handshake

### Input

### Response

### InputInProgress

### Suggestions

### RequestAuth

### Authenticate

### RequestLogHistory

### LogHistory

### Error
