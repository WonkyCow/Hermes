# Hermes

## Overview

This bot is created for the PCHH server to gather data for the creation of Audits.

## Functionality

- **Collect User Info**: Gather the Display Names, Usernames, and IDs of users with a specified role.
- **Collect Message Info**: Gether the number of messages sent, the time/date of the last message sent, and the number of messages over the last 30d from any given user
- **Collect Bot Info**: Gather information collected by other bots including Rep Bot, and modlogs from Dyno

## About this bot

This bot collects data from within joined servers including:
- Username
- Display Name
- User ID
- Message IDs

The bot does not collect
- Your account information (Email, phone no., payment info)
- Contents of your message
- Your bio info
- Your pronoun info


## Current Status/Recent Changes

This bot is currently **Inactive**

**Status**

Audit command functional
- Gathers displayname, username, ID
- Logs Number of messages sent, and messages sent in the last 30 days (does not work retro-actively)
- Does not integrate with other bots
- Stores to a local SQLl database, and displays as an embedded message in discord

- Ping command to check if the bot is responsive

- Pingable Name command to check for users whos names cannot be pinged by typing an @ in the text bar

**Recent Changes**

Added ``/Create Audit (role)`` command
Added ``/Create badnamelist`` command