# Kafka Demo

My sample code from playing with Apache Kafka.

## Starting Server

Requires `docker-compose`.

`docker-compose up -d`

## Building

Example command for building as a windows stand alone executable.

~~~
dotnet publish -c release -r win10-x64 -p:PublishSingleFile=true -p:IncludeSymbolsInSingleFile=true -p:PublishTrimmed=true -o .
~~~

## Clients
 
### Demo

Run `kafka.exe` without arguments to start a demo.

### Consumer

run `kafka.exe consumer` to start a CLI consumer.

### Producer

Run `kafka.exe producer` to start a CLI producer.
