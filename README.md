# RabbitMqExample
This is an example of the usage RabbitMq in C# with .net 6.

# Prerequisites
The RabbitMQ is installed and running on localhost on the standard port (5672).
[Downloading and installing RabbitMq](https://www.rabbitmq.com/download.html)

The .NET Core SDK 6.0 SDK is installed
[Downloading and installing SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

# Overview
The project is simulating a system for the continuous processing of files.

There are two hosted services:
* _DataCapture_: responsible for listening to the local folder and sending new files to the queue
* _ProcessData_: is listening to the queue and saving new files in a local folder

# Configuration
The configuration is in the _appsettings.json_ file.

There can be found:
* _QueueName_: the name of the queue where files are sent and which is subscribed
* _DataCaptureFolderPath_: path to the folder which will be watched be _DataCapture_ service
* _ProcessDataFolderPath_: path to the folder where files will be saved by _ProcessData_ service
* _FileExtension_: filter used to read files with specified extension only

# How to run
To run the application open the command line, go to the directory with the project and run the command:

`dotnet run`

Next, add a new file to the folder defined in the _DataCaptureFolderPath_ configuration.

The copy of the file should be created in the folder defined in the _ProcessDataFolderPath_ configuration.
