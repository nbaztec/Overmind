# Overmind

Overmind is client-server based remote support/monitoring application over a TCP connection. It supports complete event logging as well as recording facility for archival purposes.

* Features a small footprint of 40 bits (5 bytes) to encode key/mouse event information along with modifier keys.
* Supports multiple client manager using a tabbed approach.
* Uses an underlying TCP layer implementation which can be extended for additional functionality.
* On the fly image quantisation and quality adjustment.
* Facility to record session.
* Low level event logging and remote control.
* Shell/CMD access

## Required Framework

The project is dependent upon the following:

* .NET Framework 3.5
* WinApi Dlls *(user32.dll, gdi32.dll and kernel32.dll)*
* 7z executable *(7za.exe)*

## Installation

You will need to compile the project in Microsoft Visual Studio 2010.

## License

Overmind is licensed under GNU LGPL v3 (http://www.gnu.org/licenses/lgpl-3.0.html).

Overmind is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Overmind is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Overmind.  If not, see <http://www.gnu.org/licenses/>.

## Running Overmind

1. Start a server on the host machine.
2. Connect the remote machine via the client by specifying the host address.
3. The server now has access to the remote client.