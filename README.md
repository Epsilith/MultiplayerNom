MultiplayerNom
==============

A framework for creating multiplayer games!  
Based on [ByteNom](https://github.com/Yonom/ByteNom).  

## Introduction
MultiplayerNom makes it easy to create all sorts of multiplayer games or chat applications. It uses the ByteNom protocol and is compatible with all ByteNom clients!  

## Getting started
MultiplayerNom is a serverside library, clients only need the ByteNom library to be able to connect.  

To begin, you need to create a `User` and a `Room` class:  

```csharp
public class MyUser : User
{
	// Add your variables here!
	
	// Here is an example:
	public string Name { get; set; }
}
	 
public class MyRoom : Room<MyUser>
{
	protected override void OnCreate()
	{
	    // A room was created
	}
	
	protected override void OnDestroy()
	{
		// A room was closed.
	}
	
	protected override bool AllowUserJoin(MyUser user)
	{
	    // If you don't want a player to join, use "return false;"
	    return true;
	}
	
	protected override void OnJoin(MyUser user)
	{
	    // A player joined
	}
	
	protected override void OnMessage(MyUser user, Message message)
	{
	    // A player sent a message
	}
	
	protected override void OnLeave(MyUser user)
	{
	    // A player left
	}
}
```
**Note:** You can remove any of the `OnSomething` handlers you do not need.  

Let's take a closer look at this line:  

```csharp
public class MyRoom : Room<MyUser>
```
By providing your own `MyUser` class, you are now capable of using all of its variables, functions, etc. in your `AllowUserJoin`, `OnJoin`, `OnMessage`, `OnLeave` callbacks. This makes it really easy to associate data with your connections.

## Connections
To start the server, use the following code:  
```csharp
var server = new MultiplayerServer<Lobby<MyRoom>>(port);
server.Start();
```
And to connect to a MultiplayerNom server, use this in your client:  
```csharp
var client = new Client(host, port);
client.MessageReceived += (sender, message) =>
{
    // TODO: Handle incoming messages
};
client.Connect();

// We first spawn in the lobby, lets join a test room.
client.Send("join", "testroom");

// Send initial messages to the server here
client.Send("test");
```

## Lobbies
Each `MultiplayerServer<TLobby>` can contain many rooms and one lobby room where people spawn when they first connect to your server. The lobby room can then forward players to other rooms when necessary.  
**Note:** There is no limit to how many players can be in the same room.  

```csharp
new MultiplayerServer<Lobby<MyRoom>>(port);
```
In this example. we are creating a new `MultiplayerServer<TLobby>` with a lobby type of `Lobby<MyRoom>`. `Lobby<TRoom>` is a very basic lobby room which only accepts a "join" message.  
```csharp
new Message("join", roomId);
```
When a client sends this message to the `Lobby<TRoom>` room, it will move that player to the room with the given room identifier. If no room is found, it creates a new `TRoom`.  
If the join is rejected in `AllowUserJoin`, the user is disconnected from the server.  

To create your own lobby rooms (to allow listing all available rooms, for example), you need to inherit from the `LobbyBase<TUser>`. You might also want to take a look at the [original lobby class implementation](https://github.com/Yonom/MultiplayerNom/blob/master/MultiplayerNom/Lobby.cs).  

## Rooms
As you've already seen in the examples above, rooms provide a clean way to work with multiple connections. All rooms are created, initalized and managed by a `MultiplayerServer<>` object. 
**Note:** You can have different types of rooms in the same server.  

Each room is associated with a unique `RoomId` and has it's own list of users.  

### Room creation
Lobby rooms are created on server creation. All other rooms will be created once a user tries to join them or by using the `MultiplayerServer.AddRoom<TRoom>(roomId)` method.  
**Note:** The lobby room has the room id "Lobby".  

Once a room is created:  
- The room is added to the server's list
- The room's properties such as `Server` and `RoomId` are set up.
- `OnCreate` is called.

### Room destruction
Rooms can be closed by calling the `Close()` method in the room. Rooms automatically close once the last user leaves them.  
This behaviour can be altered by overriding `OnLastUserLeave()` method:  
```csharp
protected override bool OnLastUserLeave()
{
    // Use "return false;" to avoid closing the room
    return true;
}
```
**Note:** It is illegal to call `Close()` on Lobby rooms! Lobby rooms do not close when the lastest player leaves.  

When a room is closed:  
- `Room.Closed` is set to `true`.
- The room starts blocking any new joins.
- All players are disconnected.
- The room is removed from the server's room list.
- `OnDestroy` is called.

**Note:** Once a room is closed, it is impossible to start the same instance again. However, the room id becomes free to use.  

### Player join
When a new player joins a room:  
- A new `TUser` object is created and initialized. 
- `AllowUserJoin` is called on the target room.
- If `AllowUserJoin` fails, the user receives an "joinDenied" message. If this is the first room the user is joining, the user is disconnected.
- If `AllowUserJoin` succeeds, the user is added to the `Users` array in the room. `OnJoin` is called.
**Note:** While in `AllowUserJoin`, the user has not been added to the users array and will not receive any broadcast messages.  

### Player leave
When a player leaves the room:  
- The user is removed from the Users list.
- `OnLeave` is called.
- If the user was the last user in the room:
	- `OnLastUserLeave` is called.
	- `Close()` is called if the result of `OnLastUserLeave` was `true`.

### Messages
Room allows you to send a message to all users using the `Broadcast` method.  
Whenever a message is received, the `OnMessage` method is called.  
**Note:** the user must be inside the room for `OnMessage` to be called for that user.  

## Users
Once a player leaves a room, all variables associated with it (except `UserId`) are lost. In other words, a new user object is created every time a user joins a room.  

Users have the following members by default:  
- **Id:** The identifier unique to this user on the server.
- **EndPoint:** The endpoint used to connect to this server (ip and port).
- **Send:** Sends a message to the user.
- **Disconnect:** Disconnects the user.
- **MoveToRoom:** Moves this user to another room.

### Moving players across rooms
A player can be moved from one room to another using the `MoveToRoom` method.  
```csharp
bool success = user.MoveToRoom(this.Server.Get<IRoom>(roomId))
```
**Note:** players will not be notified of a room change by MultiplayerNom.  

When MoveToRoom is called:  
- `AllowPlayerJoin` is called in the target room.
- If it fails, `MoveToRoom` returns `false`, the user remains in the old room.
- `OnJoin` is called in the target room.
- New messages will be directed to the new room from now on.
- `OnLeave` is called in the old room.

## Server
You can access the server inside a room from `Room.Server`.  
The server has the following members:
- **Rooms:** Gets a list of room ids available.
- **Lobby:** Gets the lobby room for this server.
- **AddRoom:** Adds a room.
- **Get:** Gets the room with the specified room id.
- **TryGet:** Gets the room with the specified room id or null if not found.- 
- **Contains:** Checks if the room with the specified room id exists.
