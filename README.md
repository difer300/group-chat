# Financial Group Chat
## Tasks
The objetive of this project is to challenge the coding abilities of the developer.

## Scenario

Level 1 
Create a browser based chat with rooms:
* Have persistent users with profiles.
* Users can talk inside a chatroom
* Order the messages by their timestamps 
* Show only the last 50 messages 

Level 2 
Users can post commands to the chatroom with the following format /stock=APPL, and it will trigger a bot that will reply the stock quote. 
* This command won’t be saved on the database as a post but it will trigger a RabbitMQ message. 
* A decoupled bot will get the CSV information from the external API at https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv. 
* The response from the external API must be posted back into the chatroom with the following format: “APPL quote is $93.42 per share”. 
* The post owner will be the bot. 

## Solution
The solution include two steps: 
* The first window show the login option. In this part an existing user can join with a username, in the case that the username doesn't exits it will be create automatically 
![Alt text](https://raw.githubusercontent.com/difer300/group-chat/master/Login.JPG?raw=true "Title")

* The second window include the chat option. At the left will be include all the existing groups, to join in any of them just click and the chat will be load. The chat windows will show all the messages of the differnts users organice by created date. 
![Alt text](https://raw.githubusercontent.com/difer300/group-chat/master/Chat.JPG?raw=true "Title")
 
## Built With

* [EntityFramework](https://docs.microsoft.com/en-us/ef/) - Framework to access to the DB
* [Pusher](https://pusher.com/) - Realtime library
* [RabbitMQ](https://www.rabbitmq.com/) - Queue Manager
* [NUnit](https://nunit.org/) - Used to create the unit tests
* [CsvHelper](https://joshclose.github.io/CsvHelper/) - Used to read csv files

## Authors

* **[Diego Navarro** - https://github.com/difer300

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
