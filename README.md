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
 
## Built With

* [EntityFramework](https://docs.microsoft.com/en-us/ef/) - Framework to access to the DB
* [Pusher](https://pusher.com/) - Realtime library
* [RabbitMQ](https://www.rabbitmq.com/) - Queue Manager
* [NUnit](https://nunit.org/) - Used to create the unit tests
* [CsvHelper](https://joshclose.github.io/CsvHelper/) - Used to read csv files

## Authors

* **[Diego Navarro** - hhttps://github.com/difer300

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
