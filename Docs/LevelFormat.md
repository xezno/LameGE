# Level Format

## Header

| Item          | Type          | Length        | Description                                                                       |
|---------------|---------------|---------------|-----------------------------------------------------------------------------------|
| Magic Number  | UInt32        | 4 bytes       | Contains the bytes "LEVL", and is used to identify the file as a level file.      |
| Version       | UInt32        | 4 bytes       | The version of the file (see "versions" below.)                                   |

### Versions

## Chunks

### Format

| Item          | Type          | Length        | Description                                                                       |
|---------------|---------------|---------------|-----------------------------------------------------------------------------------|
| Chunk Length  | UInt32        | 4 bytes       | Contains the total length of the chunk as a 32-bit unsigned integer.              |
| Chunk Type    | ChunkType     | 4 bytes       | The type of the chunk, with the value taken from the ChunkType enum.              |
| Chunk Data    | Mixed         | n bytes       | The chunk's data; the length of this is determined by the "chunk length" field.   |

### Chunk Types

#### Vertices
#### Faces
#### Texture Data

## Extra Information
