Task:

Condition: Create a solution that calculates statistical parameters for quotes from the "exchange" as quickly as possible. To implement this, two console applications need to be created.

1st application-server:
infinitely generates random numbers in a range (to simulate the subject area - a stream of quotes from the exchange), sends them via UDP multicast without delays.
The range and multicast group are configured through a separate XML config.

2nd application-client:
Receives data via UDP multicast, calculates for all received: mean, standard deviation, mode, and median. The total number of received quotes can be in the trillions and above.
The calculated values are output to the console on demand (pressing Enter).
The application should monitor the receipt of all quotes, the number of lost quotes (those that did not reach the client for any reason: network problems, the client did not have time to read, etc.) should be displayed along with the statistics.
Receiving packets and calculating statistical parameters should be implemented in separate threads with minimal delays.
The reception multicast group should be configured through a separate XML config (not in app.config).
Important requirement: The application must be maximally optimized for speed considering the volume of received data and provide a solution as quickly as possible (within milliseconds) - every microsecond matters for the exchange.
The application should work for a long time (week-month) without crashes due to internal errors, as well as in case of network errors.
