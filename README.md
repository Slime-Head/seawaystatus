# SeawayChecker
Windows IoT Core background app for checking the status of the Saint-Lambert seaway bike bridge. When the bridge is available, a green LED turns on. When it is not available, a red LED turns on. The status of the bridge comes from the Great Lakes St. Lawrence Seaway System's website (http://www.greatlakes-seaway.com) and is for informational purposes only.

Hardware:
  * Raspberry Pi 2 or 3
  * 1 green LED
  * 1 red LED
  * 2 560 Ohm resistors
  
Setup:
  1. Connect the shorter leg of the green LED to GPIO 3.
  2. Connect the longer leg of the green LED to a resistor and connect the other end of the resistor to a 3.3 V pin.
  3. Connect the shorter leg of the red LED to GPIO 6.
  4. Connect the longer leg of the red LED to a resistor and connect the other end of the resistor to a 3.3 V pin.
