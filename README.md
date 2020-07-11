# Project Overview

This project is the chathub for my accompanying chat client app [HenrysChatApp](https://github.com/hchan41567sf/HenrysChatApp). It is created using Visual Studio 2019 and ASP.NET CORE. When a client wants to send a chat message
to another client, it first contacts the Chathub. It tells the Chathub that it wants to send a message to another client. The Chathub then sends the message to this client.
Basically, it makes communication between the chat clients possible. It also has the responsibility of storing and retreiving messages that are sent to users while they are offline.

# How to set up the Chathub 

### 1) Create a free Microsoft Azure Account and an SQL database for your Chathub <br><br>
Sign up for a free [Microsoft Azure Account](https://azure.microsoft.com/en-us/free/). After signing up for an account, open the Azure portal to access the azure services. 
Now select **SQL databases** under **Azure Services**: <br><br>

![Screenshot](https://i.imgur.com/OtpWcxo.png) <br><br>


Now click **Add** to add a new database. In the screen after that, fill out database information such as database name and then create the database: <br><br>

![Screenshot](https://i.imgur.com/mdWUgiE.png) <br><br>

### 2) Copy the "connection string" for the SQL database you just created and paste it in the *appsettings.json* file <br><br>

To get the connection string for the database, first click the database you just created: <br><br>

![Screenshot](https://i.imgur.com/BNFo3iU.png) <br><br>

Now click **Connection strings** and copy the connection string that displays: <br><br>

![Screenshot](https://i.imgur.com/avFXPTd.png) <br><br>

Next open up the project **AS ADMINISTRATOR** with Visual Studio 2019 by clicking **ChatHubApp2.sln**. Look in the **solution** window and find the file **appsettings.json**. Open the file 
and look for the line *"SqlConnection": "your-sql-connection-string-goes-here"*. Replace *your-sql-connection-string-goes-here* with the connection string you just copied.
The connection string should look something like this:
```
Server=tcp:chat-hub-db-server.database.windows.net,1433;Initial Catalog=chat-hub-db;Persist Security Info=False;User ID=chathub2;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```
Replace *{your_password}* (including the brackets) with the password that you created for this sql server.
Now your Chathub knows which SQL server it will use to store data. <br><br>

### 3) Start the program on your local machine or publish it to Microsoft Azure
To build and start the program on your local machine, with your project open in Visual Studio, make sure the web server is set to IIS Express (check the dropdown next to the green play button but it should be set to that by default). 
Then click the green play button to build
and start the program. Now close the program (square stop button in VS). This created a file called **applicationhost.config** in the **.vs/ChatHub2/config** folder of the project. Open the file with Visual Studio. We
are going to add a line to the file so that clients can connect to the Chathub using your computer's local ip address. Press **ctrl+f** and search "binding". Look for this block of code
```
<site name="ChatHubApp2" id="2">
   <application path="/" applicationPool="ChatHubApp2 AppPool">
      <virtualDirectory path="/" physicalPath="C:\Users\Henry\GitProjects\ChatHubApp2\ChatHubApp2" />
   </application>
   <bindings>
      <binding protocol="https" bindingInformation="*:44395:localhost" />
      <binding protocol="http" bindingInformation="*:62188:localhost" />
      <binding protocol="http" bindingInformation="*:5001:192.168.42.81" />    //Add this line but replace the ip address with your computer's (the computer hosting the ChatHub)
                                                                               //local ip address. Keep the port at 5001                                                                               
   </bindings>
</site>
```
Add the line mentioned in the comment above. The line above needs to be added in order for your chat client to connect to the Chathub since it will use the ip address to connect to it. 
Now start the Chathub again in Visual Studio. With the Chathub started on your local machine, chat clients in the same local area network as your computer can now use 
the Chathub to chat with other clients that are also connected to the same local area network<br>

In order for your chat clients to connect to this Chathub, make sure they are set to connect to a 
Chathub on a local machine, and the local ip address is set to the one you added above. (instructions for this are given in my Github project for the chat client).

To publish this project on Microsoft Azure so that your chat clients can connect to a remote Chathub hosted on a  Microsoft server(using a URL like https://mychathub.azurewebsites.net/chatHub), 
and be able to chat with people that are not on the same local area network as you, right click 
your project in Visual Studio and select **publish**. Click **start**, select **App Service** (this project is an app service), make sure **Create New** is selected, then 
click **Create Profile**. Fill in the remaining information and click **Create**. Eventually you will be given a URL like https://mychathub.azurewebsites.net/chatHub that you can 
use in your chat client to connect to this remote Chathub with. Remember the URL. We will paste it into a file in the project for the chat client.<br><br>

### 4) Download the source code for the chat client and set it up
[HenrysChatApp Github Project](https://github.com/hchan41567sf/HenrysChatApp)
