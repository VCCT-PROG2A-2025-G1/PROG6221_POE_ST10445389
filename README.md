# PROG2A_Part2-ST10445389
Part two of the PROG POE by Kallan Jones - ST10445389

# Cybersecurity Awareness Bot for South African Citizens

## Overview
This console application serves as a "Cybersecurity Awareness Assistant" 
for South African citizens. It simulates real-life scenarios where users might encounter cyber threats and provides guidance on avoiding common traps.

## Features
- Voice greeting on startup
- Interactive text-based interface
- Responses to questions about password safety, phishing, and safe browsing
- Input validation and error handling
- Enhanced console UI with visual elements

## Requirements
- .NET 8.0 or later
- Windows operating system (for voice playback functionality)

## How to Run
1. Clone this repository
2. Ensure the greeting.wav file is in the output directory
3. Open the solution in Visual Studio
4. Build and run the application
5. If and only if line 127 or "using (SoundPlayer player = new SoundPlayer("greeting.wav"))" is throwing an error, 
download "System.Windows.Extensions" from the NuGet Packet Manager.

## Usage
- Type questions about cybersecurity topics
- Use "help" to see available commands and questions you can ask
- Type "exit" or "quit" to end the conversation
