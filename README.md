# Bricks Dryer Control - Dryer controller server
## Description
Main program responsible for:
- Reading current values of temperature and humidity from chambers sensors
- Control chamber actuators by PLC program
- Providing manual and automation control
- Persisting sets and sensor values
- Showing on and reading from front program
## Technologies
- [dotNET](https://dotnet.microsoft.com/)
- [SQLite](https://www.sqlite.org/)
- [NModbus](https://github.com/NModbus/NModbus)
- [NLog](https://nlog-project.org/)
## Modules
- Dryer OldProgram Importer - used for importing automatic control settings from recent dryer controller program
- Dryer Server Core - used for connecting rest of modules and business logic
- Dryer Server Interfaces - common classes and interfaces for modules
- Dryer Sqlite Persistance - used for every persistance
- Dryer Webapi Service - used for communication with frontend program
- Serial Modbus Agent - used for communication with PLC and listening sensors values
