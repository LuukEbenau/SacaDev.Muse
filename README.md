# Muse
This project is made with the goal to give (C#) .NET developers an easy way to receive and manage the data retrieved from the Muse.
The main goals&features of the project include:
*  Intuitive usage
*  Supports multiple Muses
*  option to subscribe on certain signals, can differ for each Muse.
*  Follows c# 7.3 coding styles
*  Good test coverage
*  Targeting .NET Standard 2
*  Extensibility
*  Well documented codebase
*  No external dependencies

The packet can soon been found in the Nuget Packet manager, with Packet ID "Muse".

I would like to encourage everyone to contribute to this project if you see room for improvements!
This is an early version of the project, and there's surely still a lot room for improvement.

And if you encounter weird/unintended behaviour, please leave an issue.
You can also give feature requests using the issue system.

##Current backlog:
-Create tests to cover the receiving of data from the muse
-Add package to the Nuget packet manager
-improve the OSC Parser project. atm. this codebase is an stripped down version from the OSCsharp project (see https://github.com/valyard/OSCsharp). However, this project is written in an very early .NET Framework version, and is written in an c++ ish way. So there is room for some refactoring.
