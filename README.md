## MMA events web api

https://mma-events-api.herokuapp.com/swagger

The web api has entry points for getting information about MMA events.
Also, the web api periodically updates information about events and sends data about changes in them to the telegram bot.

Implementation used:
* Clean architecture
* MongoDB
* Web api for entry points
* AutoMapper
* Pattern proxy for caching
* Parser of events for periodically receive up-to-date information about MMA events
* gRpc client to send modified data to the telegram bot
