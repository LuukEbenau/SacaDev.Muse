[![Build Status](https://sacazioto.visualstudio.com/Muse/_apis/build/status/Muse-CI?branchName=master)](https://sacazioto.visualstudio.com/Muse/_build/latest?definitionId=6?branchName=master)

# SacaDev.Muse
This project is made with the goal to give (C#) .NET developers an easy way to receive and manage the data retrieved from the ChooseMuse Muse EEG Headband.
The main goals&features of the project include:
*  Intuitive usage
*  Supports multiple Muses
*  option to subscribe on certain signals, can differ between each Muse.
*  Follows c# 7.1 coding styles
*  Good test coverage
*  Targeting .NET Standard 2
*  Extensibility
*  Well documented codebase
*  No external dependencies

The packet can been found in the Nuget Packet manager at https://www.nuget.org/packages/SacaDev.Muse/.

I would like to encourage everyone to contribute to this project if you see room for improvements!
This is an early version of the project, and there's surely still a lot room for improvement. I also have limited time, so I will probably not have enough time to pick up all issues myself, so help yourself where possible!

If you encounter weird/unintended behaviour, please leave an issue.
You can also give feature requests using the issue system.

## Getting started
To start using the project, you only need to be familiar with a single class: the MuseManager.
Basic usage to start listening to a muse can be done as following:
```	
void manager_MusePacketReceived(object sender, MusePacket e)
{
  Console.WriteLine($"{e.Name} send an packet with address '{e.Address}', containing:");
  foreach (var val in e.Values)
    Console.Write($"{val}, ");
  Console.WriteLine();
}

var manager = new MuseManager();
manager.Connect("jantje", 7000);
manager.MusePacketReceived += manager_MusePacketReceived;
```
First you have to create a new instance of the MuseManager class, no magic is happening here.
After that you can use the Connect(alias,port) command to start listening to a muse at the given muse. The alias is purely given to let you distinguish between multiple connected muses.
after that you can subscribe to the 'MusePacketReceived' event to start getting notified whenever a packet comes in.
### Subscriptions
If you are only interested in some specific types of packets, you can subscribe to only those 'signaladresses'.
The MuseManager has Three handy methods for managing subscriptions.
They all have to option to specify the alias of an specific muse, or no alias (apply to all muses).
The methods are as following:
```
//sets the subscriptions to only the given adresses
manager.SetSubscriptions(SignalAddress.Beta_Rel | SignalAddress.MuseStatus | SignalAddress.Blink);

//add a subscription to beta score and gyro to only the muse with alias "jantje".
manager.AddSubscriptions("alias", SignalAddress.Beta_Session_Score | SignalAddress.Gyro);

//remove subscription to Beta_Rel for all muses
manager.RemoveSubscriptions(SignalAddress.Beta_Rel);
```

### Closing connections
Closing connections to connected muses is really simple. You have three options of doing so:
```
//close connection with the muse by alias
manager.CloseConnection("alias");

//close connection to all connected muses
manager.CloseConnections();

//using the IDisposable interface, like so:
manager.Dispose();
//or:
using(var manager = new MuseManager()){
    //code...
}
```

## Current backlog:
See Issues: https://github.com/LuukEbenau/Muse/issues
